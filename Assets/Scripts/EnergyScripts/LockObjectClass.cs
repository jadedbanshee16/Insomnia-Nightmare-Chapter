using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockObjectClass : EnergyObjectClass
{
    [SerializeField]
    InteractionClass connectedLockedObject;

    [SerializeField]
    PositionInteractionClass connectedLockPosition;

    [SerializeField]
    LockObjectClass pairedLock;

    [SerializeField]
    bool initialLockPosition;

    //A function to set the energy manager of this object.
    public override void setEnergyManager(GridManager man)
    {
        energyManager = man;

        //When manager is set, set on depending on energyObject type. Default is off.
        isOn = initialLockPosition;
    }

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
        //Ensure the paired lock has the same 'isOn' statement.
        if (pairedLock && pairedLock.getIsOn() != isOn)
        {
            pairedLock.setIsOn(isOn);
        }
        //If the object is powered and is on, then switch material to the on materials and turn on light.
        //If not, turn them off.
        if (isPowered && !isOn)
        {
            connectedLockedObject.GetComponent<InteractionClass>().addPermission(InteractionClass.interactionType.player);
        }
        else
        {
            connectedLockedObject.GetComponent<InteractionClass>().removePermission(InteractionClass.interactionType.player);
        }
    }

    public bool checkLock()
    {
        if(connectedLockedObject.GetComponent<HoldInteractionClass>() && connectedLockedObject.GetComponent<HoldInteractionClass>().getCurrentHolder() == connectedLockPosition.gameObject)
        {
            return true;
        }

        return false;
    }
}
