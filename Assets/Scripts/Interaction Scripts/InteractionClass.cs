using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This will keep interaction control items. This can be input or energy interactions, but this will mostly take inputs and set objects.
 */
[RequireComponent(typeof(InteractionControlClass))]
public class InteractionClass : MonoBehaviour
{
    public enum interactionType
    {
        player,
        generalPosition,
        powerPosition,
        autoPosition,
        secondaryInteraction,
        senserInteraction,
        playerHold,
        unique,
        bookPosition,
        activeObjects
    }

    [SerializeField]
    private float objectID;

    [SerializeField]
    protected interactionType[] permittedInteractions;

    protected InteractionControlClass controller;

    private void Start()
    {
        setController();
    }


    //The main interaction. A player may call this for an interaction. So far, it will just run an animation to begin with.
    public virtual void Interact()
    {
        //Set the animation of the controller.
        controller.setAnimation("Pressed");

        //controller.setAnimation("isOn", true);

        //controller.setIndicator(true);

        controller.triggerEvent();

        controller.playInteractionAudio(0);

    }

    //Another overload for the interaction.
    public virtual void Interact(Vector3 newPos, Quaternion newRot, Transform trans)
    {
        controller.setAnimation("Pressed");
    }

    //An overload for the interaction to take a specific object.
    public virtual void Interact(GameObject obj)
    {
        controller.setAnimation("Pressed");
    }

    public virtual void Interact(bool b)
    {
        controller.setAnimation("Pressed");
    }

    //A function that completes a secondary or minor interaction.
    public virtual void secondaryInteract()
    {
        //This does nothing here.
    }

    //See if the inputted interaction type is on the list of permitted interactions.
    public bool isInteractionType(interactionType type)
    {
        bool isType = false;

        for(int i = 0; i < permittedInteractions.Length; i++)
        {
            if(permittedInteractions[i] == type)
            {
                isType = true;
            }
        }

        return isType;
    }

    //A function to set the controller of this current item.
    protected virtual void setController()
    {
        controller = GetComponent<InteractionControlClass>();

        //Test to see if this function has a grid manager. If not, then update the controller.
        if (!GetComponentInParent<GridManager>())
        {
            controller.updateThisInteraction();
        }
    }

    //Add a new interaction type to the list of permitted interactions of this object.
    public void addPermission(interactionType t)
    {
        bool alreadyHasType = false;

        //Check if new permission is already a listed permission.
        for(int i = 0; i < permittedInteractions.Length; i++)
        {
            if(permittedInteractions[i] == t)
            {
                alreadyHasType = true;
            }
        }

        //If not already in, then add it.
        if (!alreadyHasType)
        {
            interactionType[] newPermissionsList = new interactionType[permittedInteractions.Length + 1];

            for(int i = 0; i < permittedInteractions.Length; i++)
            {
                newPermissionsList[i] = permittedInteractions[i];
            }

            //Add the new permission.
            newPermissionsList[newPermissionsList.Length - 1] = t;

            //Replace the old list.
            permittedInteractions = newPermissionsList;
        }
    }

    //Remove an interaction type from the list of permitted interactions.
    public void removePermission(interactionType t)
    {
        int hasType = -1;

        //Check if new permission is already a listed permission.
        for (int i = 0; i < permittedInteractions.Length; i++)
        {
            if (permittedInteractions[i] == t)
            {
                hasType = i;
            }
        }

        //Create a new array and populate with all elements except for the hasType.
        if(hasType >= 0)
        {
            interactionType[] newPermissionList = new interactionType[permittedInteractions.Length - 1];

            int currentIndex = 0;

            for(int i = 0; i < permittedInteractions.Length; i++)
            {
                if(i != hasType)
                {
                    newPermissionList[currentIndex] = permittedInteractions[i];
                    currentIndex++;
                }
            }

            permittedInteractions = newPermissionList;
        }
    }

    public float getObjectID()
    {
        return objectID;
    }

    public void setObjectID(float ind)
    {
        objectID = ind;
    }
}
