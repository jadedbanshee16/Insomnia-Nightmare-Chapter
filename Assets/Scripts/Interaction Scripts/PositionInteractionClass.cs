using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PositionInteractionClass : InteractionClass
{    
    [SerializeField]
    Transform targetPos;

    [SerializeField]
    HoldInteractionClass currentHeldItem;

    [SerializeField]
    string uniqueObjectOverride;

    [SerializeField]
    GameObject connectedObject;

    //This will keep the interaction with this object. It takes an obj input and sees if it is a permitted object.
    public override void Interact(GameObject obj)
    {
        setCurrentHeldItem(obj.GetComponent<HoldInteractionClass>());
        setObject();
    }

    void setObject()
    {
        currentHeldItem.Interact(targetPos.position, targetPos.rotation, this.transform);

        //When run, then input connected item into the holdInteraction class.
        currentHeldItem.setSystem(connectedObject);
    }

    public void setCurrentHeldItem(HoldInteractionClass c)
    {
        currentHeldItem = c;
    }

    public bool canHoldItem(GameObject obj)
    {
        bool canHold = false;

        if(uniqueObjectOverride != null && string.Equals(uniqueObjectOverride, obj.gameObject.name))
        {
            canHold = true;
        } else if (string.Equals(uniqueObjectOverride, ""))
        {
            for (int i = 0; i < permittedInteractions.Length; i++)
            {
                if (permittedInteractions[i] == obj.GetComponent<HoldInteractionClass>().getType())
                {
                    canHold = true;
                }
            }
        }

        if (currentHeldItem)
        {
            canHold = false;
        }

        return canHold;
    }
}
