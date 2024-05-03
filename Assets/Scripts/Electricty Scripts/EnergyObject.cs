using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyObject : MonoBehaviour
{
    public enum objectType
    {
        lights,
        doorMechanisms,
        doors
    }

    [SerializeField]
    private float energyUsage;

    [SerializeField]
    private objectType type;

    protected bool powered;

    //A function to power the 
    public virtual void powerObject()
    {
        //Power itself.
        powered = true;
    }

    //A function to depower the object.
    public virtual void dePowerObject()
    {
        //Remove power of self.
        powered = false;
    }

    //Return the amount of energy this object takes.
    public float getEnergyAmount()
    {
        return energyUsage;
    }

    //Return if currently powered.
    public bool isPowered()
    {
        return powered;
    }

    //Return the objectType
    public objectType getType()
    {
        return type;
    }
}
