using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockObjectClass : EnergyObjectClass
{
    [SerializeField]
    protected InteractionClass connectedLockedObject;

    [SerializeField]
    protected PositionInteractionClass connectedLockPosition;

    [SerializeField]
    LockObjectClass pairedLock;

    [SerializeField]
    protected bool initialLockPosition;

    [SerializeField]
    protected InteractionClass.interactionType lockAgainst;

    //A function to set the energy manager of this object.
    public override void setEnergyManager(GridManager man)
    {
        energyManager = man;

        //When manager is set, set on depending on energyObject type. Default is off.
        /*isOn = initialLockPosition;

        if (isOn)
        {
            forceIsOn(true);
        }*/

        //Set any components that needed to be made.
        controller = GetComponent<InteractionControlClass>();
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
        initialLockPosition = isOn;
        //Ensure the paired lock has the same 'isOn' statement.
        if (pairedLock && pairedLock.getIsOn() != isOn)
        {
            pairedLock.setIsOn(isOn);
            //initialLockPosition = isOn;
            pairedLock.setInitialLock(isOn);
        }
        //If the object is powered and is on, then switch material to the on materials and turn on light.
        //If not, turn them off.
        if (isPowered && !isOn)
        {
            connectedLockedObject.GetComponent<InteractionClass>().addPermission(lockAgainst);
            controller.playInteractionAudio(0);
        }
        else
        {
            //Make absolutely sure that the door is closed first.
            connectedLockedObject.GetComponent<InteractionClass>().removePermission(lockAgainst);
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

    public bool getInitialLock()
    {
        return initialLockPosition;
    }

    public void setInitialLock(bool b)
    {
        initialLockPosition = b;
    }

    public void forceClose()
    {
        if(connectedLockedObject && connectedLockPosition)
        {
            connectedLockPosition.Interact(connectedLockedObject.gameObject);
        }
    }
}
