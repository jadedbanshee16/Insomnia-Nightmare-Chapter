using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInteractionClass : InteractionClass
{
    GridManager manager_;

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

    public void setToOff()
    {
        //First, set the object to on so that when the interaction occurs, it will turn off.
        isOn = true;
        Interact();
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
        isOn = manager_.getIsOn();

        controller.setAnimation("isOn", isOn);

        controller.setIndicator(isOn);
    }
}
