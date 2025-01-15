using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Classes for objects that use the energy system. This can be turned on and off, and completes a certain action when activated.

This is for all objects which require energy to function. This includes electric doors, screens, lights, etc.
*/
[RequireComponent(typeof(InteractionControlClass))]
public class EnergyObjectClass : MonoBehaviour
{
    [SerializeField]
    private float objectId;
    
    [SerializeField]
    protected int energyUsage;

    [SerializeField]
    protected bool isOn;

    [SerializeField]
    protected int amountInteractionsNeeded = 1;
    [SerializeField]
    protected int amountOn;

    [SerializeField]
    protected bool isPowered;

    protected GridManager energyManager;

    protected InteractionControlClass controller;

    //A function that will power the current object.
    public virtual void powerObject(bool b)
    {
        if(energyUsage == 0)
        {
            isPowered = true;
        } else
        {
            isPowered = b;
        }
    }

    //A function that will make the object be used if it is powered on on.
    public virtual void useObject()
    {
        if(isPowered && isOn)
        {
            //This is just an empty template.
        }
    }

    //A function to set the energy manager of this object.
    public virtual void setEnergyManager(GridManager man)
    {
        energyManager = man;

        //When manager is set, set on depending on energyObject type. Default is off.
        isOn = false;

        //Set any components that needed to be made.
        controller = GetComponent<InteractionControlClass>();
    }

    //Return the amount of energy this object uses.
    public virtual int getEnergyAmount()
    {
        return energyUsage;
    }

    //Return if object is powered.
    public virtual bool isObjectPowered()
    {
        return isPowered;
    }

    //return if the object is on.
    public virtual bool getIsOn()
    {
        return isOn;
    }

    public virtual int getAmount()
    {
        return amountOn;
    }

    public void setObjectID(float id)
    {
        objectId = id;
    }

    public float getObjectID()
    {
        return objectId;
    }

    public virtual GridManager getEnergyManager()
    {
        return energyManager;
    }

    //Set the current on state of the object.
    public virtual void setIsOn(bool b)
    {
        //Debug.Log("React2: " + b + " | " + this.gameObject.name);
        if (b)
        {
            amountOn++;
        } else
        {
            amountOn--;
        }

        if(amountOn >= amountInteractionsNeeded)
        {
            isOn = true;
        } else
        {
            isOn = false;
        }
    }

    //A function to force isOn and off.
    //WARNING. When turning on, this would probably make 'amount on' incorrect.
    public virtual void forceIsOn(bool b)
    {
        if (b)
        {
            amountOn = amountInteractionsNeeded;
            isOn = true;
        } else
        {
            amountOn = 0;
            isOn = false;
        }
    }
}
