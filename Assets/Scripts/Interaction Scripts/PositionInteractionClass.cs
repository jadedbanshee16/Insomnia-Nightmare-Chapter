using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PositionInteractionClass : InteractionClass
{    
    [SerializeField]
    protected Transform targetPos;

    [SerializeField]
    protected HoldInteractionClass currentHeldItem;

    [SerializeField]
    protected string uniqueObjectOverride;

    [SerializeField]
    protected GameObject connectedObject;

    //This will keep the interaction with this object. It takes an obj input and sees if it is a permitted object.
    public override void Interact(GameObject obj)
    {
        setCurrentHeldItem(obj.GetComponent<HoldInteractionClass>());
        setObject();
    }

    public void setObject()
    {
        if (!controller)
        {
            setController();
        }

        //If this is not a senser, then make it interact.
        if(currentHeldItem.getType() != interactionType.senserInteraction)
        {
            currentHeldItem.Interact(targetPos.position, targetPos.rotation, this.transform);
        }
        if (!currentHeldItem)
        {
            //Debug.Log("Check current held item");
        } else
        {
            //When run, then input connected item into the holdInteraction class.
            currentHeldItem.setSystem(connectedObject);
        }

        controller.playInteractionAudio(0);
    }

    public virtual void setCurrentHeldItem(HoldInteractionClass c)
    {
        currentHeldItem = c;

        //Have an extra test based on some interactions.
        if (!currentHeldItem && connectedObject && connectedObject.GetComponent<InvertedLockObjectClass>())
        {
            //This means that connected object needs to be told thatthat current held item is false.
            connectedObject.GetComponent<InvertedLockObjectClass>().setIsOn(false);
            connectedObject.GetComponent<InvertedLockObjectClass>().useObject();
        }

        //Complete any connected object interactions if item is removed.
        if (connectedObject && connectedObject.GetComponent<EnergyObjectClass>() && !connectedObject.GetComponent<LockObjectClass>() && !connectedObject.GetComponent<EnergySlotObject>() && currentHeldItem == null)
        {
            connectedObject.GetComponent<EnergyObjectClass>().getEnergyManager().updateObject(connectedObject.GetComponent<EnergyObjectClass>(), false);
        } else if (connectedObject && connectedObject.GetComponent<EnergySlotObject>() && currentHeldItem == null)
        {
            //First, compelte normal.
            //connectedObject.GetComponent<EnergyObjectClass>().getEnergyManager().updateObject(connectedObject.GetComponent<EnergyObjectClass>(), false);
            //Then remove the energy slot to remove the object.
            connectedObject.GetComponent<EnergySlotObject>().setConnectedObject(null);
        }
    }

    public HoldInteractionClass getCurrentHeldItem()
    {
        return currentHeldItem;
    }

    //Used to check if position item can hold the item it is interacting with.
    public bool canHoldItem(HoldInteractionClass obj, bool auto)
    {
        bool canHold = false;

        //Test for unique first, if unique and string is not empty, then check if name is correct.
        if (hasPermission(interactionType.unique) && !string.Equals(uniqueObjectOverride, ""))
        {
            //If unique, then check name.
            if (string.Equals(uniqueObjectOverride, obj.gameObject.name))
            {
                canHold = true;
            }
        } else
        {
            //If override is empty or not interaction unique, check for any other interactions.
            canHold = hasPermission(obj.getType());
        }


        if (currentHeldItem)
        {
            canHold = false;
        }

        if(auto && obj.getCurrentHolder() != null)
        {
            canHold = false;
        }

        return canHold;
    }

    public string getUniqueOverride()
    {
        return uniqueObjectOverride;
    }

    //A function which tests to see if this object has permission to hold a certain interaction type.
    protected bool hasPermission(interactionType t)
    {
        bool hasPerm = false;

        for(int i = 0; i < permittedInteractions.Length; i++)
        {
            if(permittedInteractions[i] == t)
            {
                hasPerm = true;
            }
        }

        return hasPerm;
    }

    //A trigger enter for colliding with an interaction.
    //This should only be activated if current type is autoPosition or senser
    private void OnTriggerStay(Collider other)
    {
        //Only see if trigger if this object is an autoposition.
        if (hasPermission(interactionType.autoPosition) && other.GetComponent<HoldInteractionClass>() && canHoldItem(other.GetComponent<HoldInteractionClass>(), true))
        {
            //If met, the interact with object.
            Interact(other.gameObject);

        //If no achievement script, run a sensor interaction as normal.
        //Keep for now but OBSOLETE.
        //NOTE: The lock object is no longer used in this way, but keep for old problems.
        } else if(hasPermission(interactionType.senserInteraction) && other.GetComponent<HoldInteractionClass>() && canHoldItem(other.GetComponent<HoldInteractionClass>(), true) &&
                  !GetComponentInChildren<AchievementScript>())
        {
            //If true, then doesn't need to set any objects. Just set the connected object.
            if (connectedObject.GetComponent<LockObjectClass>())
            {
                connectedObject.GetComponent<EnergyObjectClass>().setIsOn(true);
                connectedObject.GetComponent<EnergyObjectClass>().useObject();
            }

            setCurrentHeldItem(other.GetComponent<HoldInteractionClass>());
        }
    }

    //Play when leaving a given function.
    private void OnTriggerExit(Collider other)
    {
        bool isOther = false;
        if (currentHeldItem)
        {
            isOther = currentHeldItem.gameObject.GetInstanceID() == other.gameObject.GetInstanceID();
        }

        if(isOther && currentHeldItem.GetComponent<HoldInteractionClass>() && currentHeldItem.GetComponent<HoldInteractionClass>().getType() == interactionType.senserInteraction)
        {
            if (connectedObject.GetComponent<LockObjectClass>() && !connectedObject.GetComponent<InvertedLockObjectClass>())
            {
                connectedObject.GetComponent<EnergyObjectClass>().setIsOn(false);
                connectedObject.GetComponent<EnergyObjectClass>().useObject();
            }

            currentHeldItem.removeHeld();
            setCurrentHeldItem(null);
        }
    }

    //A function to return the connected object.
    public GameObject getConnectedObject()
    {
        return connectedObject;
    }
}
