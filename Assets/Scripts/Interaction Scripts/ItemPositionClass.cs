using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPositionClass : InteractionClass
{
    [SerializeField]
    string permittedItem;

    [SerializeField]
    Transform holdPosition;

    public bool hasItem = false;

    public override void interact(Transform obj)
    {
        GameObject item = GameObject.Find("Player").GetComponent<FPSController>().getHeldItem();

        //If the added obj is a permitted object, then set new obj to new position.
        if (string.Equals(obj.gameObject.name, permittedItem))
        {
            obj.gameObject.GetComponent<InteractionClass>().interact(holdPosition);
            obj.gameObject.GetComponent<HoldItemClass>().forceAddColliders();

            GameObject.Find("Player").GetComponent<FPSController>().removeHeldItem();

            hasItem = true;
        }
    }
}
