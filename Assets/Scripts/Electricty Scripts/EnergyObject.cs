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

    public bool powered;

    [SerializeField]
    protected EnergizerScript powerBox;

    public void setPowerBox(EnergizerScript script)
    {
        powerBox = script;
    }

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

    //A function to use the current energy object.
    public virtual void useObject(bool b)
    {
        //Do nothing as this energy object is unusable.
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

    //Return if the object is currently being used.
    public virtual bool isUsed()
    {
        //If default with nothing, return true.
        return true;
    }
}
