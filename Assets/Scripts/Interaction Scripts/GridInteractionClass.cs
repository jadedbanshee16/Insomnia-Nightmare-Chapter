using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInteractionClass : InteractionClass
{
    GridManager manager_;

    [SerializeField]
    bool isOn;

    public override void Interact()
    {
        isOn = !isOn;

        //This is to function either with a switch, button or indication.
        //Set the animation of the controller.
        controller.setAnimation("Pressed");

        controller.setAnimation("isOn", isOn);

        controller.setIndicator(isOn);

        controller.playInteractionAudio(0);

        setObject();

    }

    public override void Interact(bool b)
    {
        isOn = b;

        if (!controller)
        {
            setController();
        }

        //This is to function either with a switch, button or indication.
        //Set the animation of the controller.
        controller.setAnimation("Pressed");

        controller.setAnimation("isOn", isOn);

        controller.setIndicator(isOn);

        controller.playInteractionAudio(0);

        setObject();
    }

    //Update power manager with the new system.
    public void setObject()
    {
        manager_.setGrid(isOn);
        manager_.updateGrids();
    }

    //A function that works like the 'Start', only it ensures that things that need to be done before it
    //will finish first before calling this function.
    public void updateGridSwitch()
    {
        setController();
        manager_ = GetComponentInParent<GridManager>();
        manager_.setGrid(isOn);

        controller.setAnimation("isOn", isOn);

        controller.setIndicator(isOn);
    }

    public bool getIsOn()
    {
        return isOn;
    }
}
