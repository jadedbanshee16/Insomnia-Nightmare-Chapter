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
    string uniqueObjectOverride;

    [SerializeField]
    protected GameObject connectedObject;

    //This will keep the interaction with this object. It takes an obj input and sees if it is a permitted object.
    public override void Interact(GameObject obj)
    {
        setCurrentHeldItem(obj.GetComponent<HoldInteractionClass>());
        setObject();
    }

    public virtual void setObject()
    {
        currentHeldItem.Interact(targetPos.position, targetPos.rotation, this.transform);

        //When run, then input connected item into the holdInteraction class.
        currentHeldItem.setSystem(connectedObject);
    }

    public virtual void setCurrentHeldItem(HoldInteractionClass c)
    {
        currentHeldItem = c;
    }

    public HoldInteractionClass getCurrentHeldItem()
    {
        return currentHeldItem;
    }

    public bool canHoldItem(GameObject obj)
    {
        bool canHold = false;

        if(uniqueObjectOverride != null && string.Equals(uniqueObjectOverride, obj.gameObject.name))
        {
            canHold = true;
        } else if (string.Equals(uniqueObjectOverride, ""))
        {
            canHold = hasPermission(obj.GetComponent<HoldInteractionClass>().getType());
        }

        if (currentHeldItem)
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
    private bool hasPermission(interactionType t)
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
    //This should only be activated if current type is autoPosition.
    private void OnTriggerStay(Collider other)
    {
        //Only see if trigger if this object is an autoposition.
        if (hasPermission(interactionType.autoPosition) && canHoldItem(other.gameObject))
        {
            //If met, the interact with object.
            Interact(other.gameObject);
        }
    }
}
