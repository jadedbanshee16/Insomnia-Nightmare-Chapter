using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPositionClass : InteractionClass
{
    //Show what item is permitted to be placed onto this position.
    [SerializeField]
    string permittedItem;

    //The current position of this item.
    [SerializeField]
    Transform holdPosition;

    //Something to show if item is on position or not.
    public bool hasItem = false;

    //Interaction.
    /* Summary:
     * Get a held item from player. If permitted item, place into this position.
     */
    public override void interact(Transform holder, Transform obj)
    {

        //If the added obj is a permitted object, then set new obj to new position.
        if (string.Equals(obj.gameObject.name, permittedItem))
        {
            obj.gameObject.GetComponent<InteractionClass>().interact(this.transform, holdPosition);
            obj.gameObject.GetComponent<HoldItemClass>().forceAddColliders();

            hasItem = true;
        }
    }

    public void removeHeldItem()
    {
        hasItem = false;
    }
}
