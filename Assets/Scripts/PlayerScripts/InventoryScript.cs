using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    [SerializeField]
    Transform[] inventoryPositions;
    [SerializeField]
    List<inventoryItem> inventoryObjects;
    [SerializeField]
    Transform actualActiveHand;

    public int activeHand = 0;

    //A function that adds a new given object to your inventory.
    //This should be called when a player selects a holdable object.
    public void addObject(HoldInteractionClass newObject)
    {
        int availableSlot = findAvailableSlot();

        //Debug.Log(availableSlot);


        //If lesser than 0, no available slots and so don't complete the 'add'.
        if (availableSlot > -1)
        {
            //Ensure object can go into inventory if it is unable to go to right hand.
            if(availableSlot == 0 || (availableSlot > 0 && newObject.isInteractionType(InteractionClass.interactionType.playerInventory)))
            {
                newObject.Interact(inventoryPositions[availableSlot].transform.GetChild(0).position, inventoryPositions[availableSlot].transform.GetChild(0).rotation, inventoryPositions[availableSlot].transform);
                inventoryObjects.Add(new inventoryItem(newObject, availableSlot));
                return;
            }

            //Set the activeHand to the availableSlot. It is now no longer available but store the last position the object has been moved.
            //activeHand = availableSlot;

            for(int i = 0; i < inventoryObjects.Count; i++)
            {
                Debug.Log("Object " + i + ": " + inventoryObjects[i].item);
            }
        }

        //If it got this far, then you are unable to pick up the item. Debug for prosperity.
        Debug.Log("Unable to pick up item.");
    }

    public void dropObject(Vector3 front)
    {
        int obj = findObjectInSlot(0);

        //Debug.Log("Working? " + obj);

        if(obj > -1)
        {
            inventoryObjects[obj].item.Interact(front, Quaternion.identity, null);
            inventoryObjects[obj].item.removeHeld();
            inventoryObjects.RemoveAt(obj);
        }
    }

    public void placeObject(InteractionClass newParent, int ind)
    {
        newParent.Interact(inventoryObjects[ind].item.gameObject);
        inventoryObjects.RemoveAt(ind);
    }

    public HoldInteractionClass getObjectAtInvent(int ind)
    {
        if(ind >= 0 && ind < inventoryObjects.Count)
        {
            return inventoryObjects[ind].item;
        } else
        {
            return null;
        }
    }

    //Switch current hand with the last active hand. This should start at 1.
    public void switchObject(int offset)
    {
        //Return the item at position 0.
        activeHand += offset;

        //Loop the movement around the positions.
        if(activeHand < 1)
        {
            activeHand = inventoryPositions.Length - 1;
        } else if (activeHand > inventoryPositions.Length - 1)
        {
            activeHand = 1;
        }

        int newInd = findObjectInSlot(activeHand);
        int actInd = findObjectInSlot(0);

        //Make sure the item can be but into inventory before trying.
        if(actInd >= 0 && inventoryObjects[actInd].item.isInteractionType(InteractionClass.interactionType.playerInventory))
        {
            inventoryItem newItem = new inventoryItem(inventoryObjects[actInd].item, activeHand);

            //Store all the activeobject.
            newItem.item.removeHeld();
            newItem.item.Interact(inventoryPositions[activeHand].GetChild(0).position, inventoryPositions[activeHand].GetChild(0).rotation, inventoryPositions[activeHand].transform);

            //Add new struct with the new data.
            inventoryObjects[actInd] = newItem;
        }

        //Make sure active hand has been made empty.
        if (newInd >= 0 && findObjectInSlot(0) < 0)
        {
            inventoryItem newItem = new inventoryItem(inventoryObjects[newInd].item, 0);
            //switch item to main hand.
            newItem.item.removeHeld();
            newItem.item.Interact(inventoryPositions[0].GetChild(0).position, inventoryPositions[0].transform.GetChild(0).rotation, inventoryPositions[0].transform);

            inventoryObjects[newInd] = newItem;
        }
    }

    /*public HoldInteractionClass returnHeldObject()
    {
        return inventoryObjects[activeHand].getCurrentHeldItem();
    }*/

    //A function to return whether the intentory list is currently filled.
    public int findAvailableSlot()
    {
        List<int> positions = new List<int>();


        //Populate positions so that the final positions will match the index.
        for(int i = 0; i < inventoryPositions.Length; i++)
        {
            if(!inventoryObjects.Any(x => x.place == i))
            {
                positions.Add(i);
            }
        }

        //If any more positions left, then assume they are available.
        if(positions.Count > 0)
        {
            if(positions[0] >= 0 && positions[0] < inventoryPositions.Length)
            {
                return positions[0];
            }

            //Failure to match position to an inventory position. Shouldn't be possible.
            return -1;
        }

        //Only gets this far if no available slots.
        return -1;
    }


    //Finds the connected position of a given point.
    public int findObjectInSlot(int ind)
    {
        for(int i = 0; i < inventoryObjects.Count; i++)
        {
            if(inventoryObjects[i].place == ind)
            {
                //Debug.Log("Found object: " + inventoryObjects[i].item.name + " in " + inventoryPositions[ind].name);
                return i;
            }
        }

        return -1;
    }


    //Warning: This assumes the given inventory items can be found when searching for interactables.
    public void updateInventorySlots()
    {
        //All slots would have their items saved in interaction data, so no need to find if more points can be added.

        //Set up the interactables. This should be the only objects in the inventory.
        //inventoryPositions = GetComponentsInChildren<Transform>();

        inventoryPositions = new Transform[transform.childCount + 1];
        inventoryObjects = new List<inventoryItem>();

        if (actualActiveHand)
        {
            inventoryPositions[0] = actualActiveHand;
        } else
        {
            inventoryPositions[0] = transform;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            inventoryPositions[i + 1] = transform.GetChild(i);
        }

        //Now find the objects that are connected to them.
        HoldInteractionClass[] items = GameObject.FindObjectsByType<HoldInteractionClass>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for(int i = 0; i < items.Length; i++)
        {
            for(int v = 0; v < inventoryPositions.Length; v++)
            {
                if(items[i].currentHolder == inventoryPositions[v])
                {
                    inventoryObjects.Add(new inventoryItem(items[i], v));
                }
            }
        }
    }

    //A struct to keep item data.
    public struct inventoryItem
    {
        public HoldInteractionClass item { get; }
        public int place { get; set; }

        public inventoryItem(HoldInteractionClass it, int i)
        {
            item = it;
            place = i;
        }
    }
}
