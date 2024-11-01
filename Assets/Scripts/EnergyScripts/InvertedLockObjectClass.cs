using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertedLockObjectClass : LockObjectClass
{
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

        //If the object is powered and is on, then switch material to the on materials and turn on light.
        //If not, turn them off.
        if (isPowered && !isOn)
        {
            connectedLockedObject.GetComponent<InteractionClass>().addPermission(lockAgainst);
            controller.playInteractionAudio(0);
        }
        else
        {
            //Ensure to force door to close.
            //connectedLockPosition.Interact(connectedLockedObject.gameObject);
            //Make absolutely sure that the door is closed first.
            connectedLockedObject.GetComponent<InteractionClass>().removePermission(lockAgainst);
        }
    }

    public override void setIsOn(bool b)
    {
        //Debug.Log("Worked!: " + this.gameObject.name);
        if (b)
        {
            amountOn++;
        }
        else
        {
            amountOn--;
        }

        if (amountOn < amountInteractionsNeeded)
        {
            isOn = true;
        }
        else
        {
            isOn = false;
        }
    }

    public override void forceIsOn(bool b)
    {
        //This lock use the environment to remember the lock so lock is not enforcable.
        
        /*if (b)
        {
            amountOn = amountInteractionsNeeded;
            isOn = true;
        }
        else
        {
            amountOn = 0;
            isOn = false;
        }*/
    }
}
