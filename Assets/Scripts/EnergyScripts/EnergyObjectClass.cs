using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Classes for objects that use the energy system. This can be turned on and off, and completes a certain action when activated.

This is for all objects which require energy to function. This includes electric doors, screens, lights, etc.
*/
public class EnergyObjectClass : MonoBehaviour
{
    [SerializeField]
    int energyUsage;

    [SerializeField]
    bool isOn;

    [SerializeField]
    bool isPowered;

    GridManager energyManager;


    //A function that will power the current object.
    public void powerObject(bool b)
    {
        isPowered = b;
    }

    //A function that will make the object be used if it is powered on on.
    public void useObject()
    {
        if(isPowered && isOn)
        {
            //This is just an empty template.
        }
    }

    //A function to set the energy manager of this object.
    public void setEnergyManager(GridManager man)
    {
        energyManager = man;
    }

    //Return the amount of energy this object uses.
    public int getEnergyAmount()
    {
        return energyUsage;
    }

    //Return if object is powered.
    public bool isObjectPowered()
    {
        return isPowered;
    }

    //return if the object is on.
    public bool getIsOn()
    {
        return isOn;
    }

    //Set the current on state of the object.
    public void setIsOn(bool b)
    {
        isOn = b;
    }
}
