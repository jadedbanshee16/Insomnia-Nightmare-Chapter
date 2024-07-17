using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionInteractionClass : InteractionClass
{
    [SerializeField]
    Transform targetPos;

    HoldInteractionClass currentHeldItem;

    [SerializeField]
    HoldInteractionClass.holdType[] permittedTypes;

    [SerializeField]
    string uniqueObjectOverride;

    //This will keep the interaction with this object. It takes an obj input and sees if it is a permitted object.
    public override void Interact(GameObject obj)
    {
        currentHeldItem = obj.GetComponent<HoldInteractionClass>();
        setObject();
    }

    void setObject()
    {
        currentHeldItem.Interact(targetPos.position, targetPos.rotation, this.transform);
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
        } else
        {
            for (int i = 0; i < permittedTypes.Length; i++)
            {
                if (permittedTypes[i] == obj.GetComponent<HoldInteractionClass>().getType())
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
