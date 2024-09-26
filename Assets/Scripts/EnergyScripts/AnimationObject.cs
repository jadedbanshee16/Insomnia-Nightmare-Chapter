using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionControlClass))]
public class AnimationObject : EnergyObjectClass
{
    InteractionControlClass controller;

    bool wasOn = true;

    //A function that will power the current object.
    public override void powerObject(bool b)
    {
        isPowered = b;

        //Maybe will break. Will find out.
        useObject();
    }

    //A function that will make the object be used if it is powered on on.
    public override void useObject()
    {
        //If the object is powered and is on, then use the animation 'isOn' is any given animation.
        if (isPowered && isOn)
        {
            controller.setAnimation("isOn", true);
            controller.playInteractionAudio(0);

            wasOn = true;
        }
        else
        {
            controller.setAnimation("isOn", false);

            if (wasOn)
            {
                controller.playInteractionAudio(1);

                wasOn = false;
            }
        }
    }

    //A function to set the energy manager of this object.
    public override void setEnergyManager(GridManager man)
    {
        energyManager = man;

        //When manager is set, set on depending on energyObject type. Default is off.
        isOn = false;

        //Set any components that needed to be made.
        controller = GetComponent<InteractionControlClass>();
    }
}
