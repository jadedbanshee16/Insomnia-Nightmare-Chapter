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
        powerPosition
    }


    [SerializeField]
    interactionType[] permittedInteractions;

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
    protected void setController()
    {
        controller = GetComponent<InteractionControlClass>();
    }
}
