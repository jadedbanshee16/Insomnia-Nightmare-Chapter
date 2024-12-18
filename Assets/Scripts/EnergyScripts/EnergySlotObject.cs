using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This behaves as an energy object placeholder for grids.
 * If it doesn't have an energy object, then it shouldn't run anything. then it doesn't use it.
 */
public class EnergySlotObject : EnergyObjectClass
{
    [SerializeField]
    EnergyObjectClass connectedObject;

    [SerializeField]
    EnergyInteractionClass[] switchConnector;

    public void setConnectedObject(EnergyObjectClass c)
    {
        connectedObject = c;
        if (connectedObject)
        {
            connectedObject.setEnergyManager(energyManager);
            connectedObject.powerObject(isPowered);

            for (int i = 0; i < switchConnector.Length; i++)
            {
                if (switchConnector[i].getIsOn())
                {
                    setIsOn(switchConnector[i].getIsOn());
                }
            }

            useObject();
        }
    }

    public override void powerObject(bool b)
    {
        isPowered = b;
        
        if (connectedObject)
        {
            connectedObject.powerObject(isPowered);
        }
    }

    public override void useObject()
    {
        if (connectedObject)
        {
            //Debug.Log("React 3: " + connectedObject.name);
            connectedObject.useObject();
        }
    }

    public override void setEnergyManager(GridManager man)
    {
        energyManager = man;

        if (connectedObject)
        {
            connectedObject.setEnergyManager(man);
        }
    }

    //Return the amount of energy this object uses.
    public override int getEnergyAmount()
    {
        if (connectedObject)
        {
            return connectedObject.getEnergyAmount();
        }
        return 0;
    }

    //Return if object is powered.
    public override bool isObjectPowered()
    {
        if (connectedObject)
        {
            return connectedObject.isObjectPowered();
        }
        return false;
    }

    //return if the object is on.
    public override bool getIsOn()
    {
        if (connectedObject)
        {
            return connectedObject.getIsOn();
        }
        return false;
    }

    public override int getAmount()
    {
        if (connectedObject)
        {
            return connectedObject.getAmount();
        }
        return 0;
    }

    public override GridManager getEnergyManager()
    {
        if (connectedObject)
        {
            return connectedObject.getEnergyManager();
        }
        return energyManager;
    }

    //Set the current on state of the object.
    public override void setIsOn(bool b)
    {
        //Force off.
        base.forceIsOn(false);
        forceIsOn(false);

        //Set the on position of this energy slot.
        //This goes through and checks every switch it is connected to to see if it is on for it's personal 'on'.
        for(int i = 0; i < switchConnector.Length; i++)
        {
            if (switchConnector[i].getIsOn())
            {
                base.setIsOn(switchConnector[i].getIsOn());
            }
        }

        if (connectedObject)
        {
            connectedObject.setIsOn(b);
        }
    }

    //A function to force isOn and off.
    //WARNING. When turning on, this would probably make 'amount on' incorrect.
    public override void forceIsOn(bool b)
    {
        if (connectedObject)
        {
            connectedObject.forceIsOn(b);
        }
    }
}
