using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    [SerializeField]
    List<GameObject> inventoryObjects;

    [SerializeField]
    int maximumInventory;

    //Some special inventory slots.
    public bool torchSlot;
    public GameObject activeHand;

    [SerializeField]
    public int activeObject;


    //A function that adds a new given object to your inventory.
    //This should be called when a player selects a holdable object.
    public void addObject(GameObject newObject, Vector3 front)
    {
        if (!isInventoryFilled())
        {
            inventoryObjects.Add(newObject);

            //Set the new active hand to the new item. First, remove the old on.
            if(activeObject >= 0)
            {
                inventoryObjects[activeObject].SetActive(false);
                inventoryObjects[activeObject].GetComponent<InteractionClass>().Interact(front, Quaternion.identity, null);
                inventoryObjects[activeObject].GetComponent<HoldInteractionClass>().removeHeld();
                activeHand.transform.GetChild(0).transform.position = activeHand.transform.position;
            }

            activeObject = inventoryObjects.Count - 1;
            inventoryObjects[activeObject].GetComponent<HoldInteractionClass>().Interact(activeHand.transform.GetChild(0).position, activeHand.transform.GetChild(0).rotation, activeHand.transform);
            inventoryObjects[activeObject].SetActive(false);
        }
    }

    public void dropObject(Vector3 front)
    {
        if(activeObject >= 0)
        {
            int newActive = 0;
            if(activeObject - 1 >= 0)
            {
                newActive = activeObject - 1;
            }

            inventoryObjects[activeObject].SetActive(false);
            inventoryObjects[activeObject].GetComponent<InteractionClass>().Interact(front, Quaternion.identity, null);
            inventoryObjects[activeObject].GetComponent<HoldInteractionClass>().removeHeld();
            activeHand.transform.GetChild(0).transform.position = activeHand.transform.position;

            inventoryObjects.Remove(inventoryObjects[activeObject]);

            if(inventoryObjects.Count > 0)
            {
                //Switch to new object.
                activeObject = newActive;

                inventoryObjects[activeObject].GetComponent<HoldInteractionClass>().Interact(activeHand.transform.GetChild(0).position, activeHand.transform.GetChild(0).rotation, activeHand.transform);
                inventoryObjects[activeObject].SetActive(true);
            } else
            {
                activeObject = -1;
            }
        }
    }

    public void switchObject(int offset, Vector3 front)
    {
        int newActive = activeObject;

        if(offset > 0)
        {
            if(activeObject + offset > inventoryObjects.Count)
            {
                newActive = 0;
            } else
            {
                newActive = activeObject + offset;
            }
        } else if (offset < 0)
        {
            if(activeObject - offset < 0)
            {
                newActive = inventoryObjects.Count - 1;
            } else
            {
                newActive = activeObject - offset;
            }
        }

        //De activate the current object.
        inventoryObjects[activeObject].SetActive(false);
        inventoryObjects[activeObject].GetComponent<InteractionClass>().Interact(front, Quaternion.identity, null);
        inventoryObjects[activeObject].GetComponent<HoldInteractionClass>().removeHeld();
        activeHand.transform.GetChild(0).transform.position = activeHand.transform.position;

        //Activate the new object.
        activeObject = newActive;
        inventoryObjects[activeObject].GetComponent<HoldInteractionClass>().Interact(activeHand.transform.GetChild(0).position, activeHand.transform.GetChild(0).rotation, activeHand.transform);
        inventoryObjects[activeObject].SetActive(true);
    }

    public HoldInteractionClass returnHeldObject()
    {
        return inventoryObjects[activeObject].GetComponent<HoldInteractionClass>();
    }

    public bool isHoldingObject()
    {
        return activeObject >= 0;
    }

    //A function to return whether the intentory list is currently filled.
    public bool isInventoryFilled()
    {
        return inventoryObjects.Count == maximumInventory;
    }



    //Warning: This assumes the given inventory items can be found when searching for interactables.
    public void updateInventory(List<float> inven, int activeObj, int maximum)
    {
        //Set up the interactables. This should be the only objects in the inventory.
        HoldInteractionClass[] interactables = GameObject.FindObjectsByType<HoldInteractionClass>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        inventoryObjects = new List<GameObject>();

        activeObject = activeObj;
        maximumInventory = maximum;

        //First, update the inventory objects with the given list.
        for (int i = 0; i < inven.Count; i++)
        {
            for(int v = 0; v < interactables.Length; v++)
            {
                if(interactables[v].getObjectID() == inven[i])
                {
                    //Set the object to the list.
                    inventoryObjects.Add(interactables[v].gameObject);

                    //Set active of this object to false so it's not in the world.
                    if(i == activeObject)
                    {
                        inventoryObjects[i].GetComponent<HoldInteractionClass>().Interact(activeHand.transform.GetChild(0).position, activeHand.transform.GetChild(0).rotation, activeHand.transform);
                    } else
                    {
                        inventoryObjects[i].SetActive(false);
                    }
                }
            }
        }
    }


    public List<float> getInventory()
    {
        List<float> newList = new List<float>();

        for(int i = 0; i < inventoryObjects.Count; i++)
        {
            newList.Add(inventoryObjects[i].GetComponent<InteractionClass>().getObjectID());
        }

        return newList;
    }

    public int getCurrentActiveHand()
    {
        return activeObject;
    }

    public int setMaximumInventorySize()
    {
        return maximumInventory;
    }
}
