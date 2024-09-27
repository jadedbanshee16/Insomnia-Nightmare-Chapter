using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightObjectClass : EnergyObjectClass
{
    MeshRenderer rend_;

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
        //If the object is powered and is on, then switch material to the on materials and turn on light.
        //If not, turn them off.
        if (isPowered && isOn)
        {
            controller.setIndicator(isOn);
            controller.setActive(isOn);
        } else
        {
            controller.setIndicator(false);
            controller.setActive(false);
        }
    }

    //A function to set the energy manager of this object.
    public override void setEnergyManager(GridManager man)
    {
        energyManager = man;

        //When manager is set, set on depending on energyObject type. Default is off.
        isOn = false;

        //Set any components that needed to be made.
        rend_ = GetComponentInChildren<MeshRenderer>();

        controller = GetComponent<InteractionControlClass>();
    }
}
