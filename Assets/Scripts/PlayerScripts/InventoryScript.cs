using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    [SerializeField]
    Transform[] inventoryPositions;
    [SerializeField]
    HoldInteractionClass[] inventoryObjects;

    [SerializeField]
    int inventorySize;

    public int activeHand = 0;

    //A function that adds a new given object to your inventory.
    //This should be called when a player selects a holdable object.
    /*public void addObject(HoldInteractionClass newObject, Vector3 front)
    {
        int availableSlot = findAvailableSlot();


        //If lesser than 0, no available slots and so don't complete the 'add'.
        if (availableSlot > -1)
        {
            //Add the current object in '0' to the given slot, then move the new added object to the old one.
            //If the hold interaction object is NOT able to be moved, then do nothing.
            if(inventoryObjects[0].getCurrentHeldItem() != null && inventoryObjects[0].getCurrentHeldItem().isInteractionType(InteractionClass.interactionType.playerInventory))
            {
                //Add this object to the available slot.
                inventoryObjects[0].getCurrentHeldItem().Interact(inventoryObjects[availableSlot].transform.GetChild(0).position, inventoryObjects[availableSlot].transform.GetChild(0).rotation, inventoryObjects[availableSlot].transform);
            }

            //Considering the above is complete, test if inventory active hand is not null and try to put something in it.
            if(inventoryObjects[0].getCurrentHeldItem() == null)
            {
                newObject.Interact(inventoryObjects[0].transform.GetChild(0).position, inventoryObjects[activeHand].transform.GetChild(0).rotation, inventoryObjects[activeHand].transform);
            }

            //If it got this far, then you are unable to pick up the item. Debug for prosperity.
            Debug.Log("Unable to pick up item.");
        }
    }*/

    /*public void dropObject(Vector3 front)
    {
        if(inventoryObjects[activeHand].getCurrentHeldItem() != null)
        {
            inventoryObjects[activeHand].getCurrentHeldItem().Interact(front, Quaternion.identity, null);
            inventoryObjects[activeHand].getCurrentHeldItem().removeHeld();
        }
    }*/

    //Cycle all currentHeldItems up by removing them 
    /*public void switchObject(int offset)
    {
        //Store all the activeobject.
        HoldInteractionClass activeObj = inventoryObjects[0].getCurrentHeldItem();
        HoldInteractionClass newObj = inventoryObjects[offset].getCurrentHeldItem();

        //Offset is the new position. This can never be zero (active hand).
        //This marks the new position that switches with active hand.
        activeHand = offset;

        //Remove items from current positions.
        if(activeObj)
        {
            activeObj.removeHeld();
        }

        if(newObj)
        {
            
            newObj.removeHeld();
        }

        activeObj.Interact(inventoryObjects[offset].transform.GetChild(0).position, inventoryObjects[offset].transform.GetChild(0).rotation, inventoryObjects[offset].transform);
        newObj.Interact(inventoryObjects[0].transform.GetChild(0).position, inventoryObjects[0].transform.GetChild(0).rotation, inventoryObjects[0].transform);
    }*/

    /*public HoldInteractionClass returnHeldObject()
    {
        return inventoryObjects[activeHand].getCurrentHeldItem();
    }*/

    //A function to return whether the intentory list is currently filled.
    /*public int findAvailableSlot()
    {
        //Find and return the first available slot.
        for(int i = 0; i < inventoryObjects.Length; i++)
        {
            if(inventoryObjects[i].getCurrentHeldItem() == null)
            {
                return i;
            }
        }

        return -1;
    }*/


    //Warning: This assumes the given inventory items can be found when searching for interactables.
    public void updateInventorySlots()
    {
        //All slots would have their items saved in interaction data, so no need to find if more points can be added.

        //Set up the interactables. This should be the only objects in the inventory.
        inventoryPositions = GetComponentsInChildren<Transform>();
    }
}
