using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightObjectClass : EnergyObjectClass
{
    [SerializeField]
    Material[] switchMats;

    [SerializeField]
    GameObject litObj;

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
            rend_.material = switchMats[0];
            litObj.SetActive(true);
        } else
        {
            rend_.material = switchMats[1];
            litObj.SetActive(false);
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
    }
}
