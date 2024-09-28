using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiInteractionEnergyClass : MonoBehaviour
{
    [SerializeField]
    int amountNeeded;

    public int amountOn = 0;

    [SerializeField]
    EnergyObjectClass[] connectedObjects;

    //Set the current on state of the object.
    public void setIsOn(bool b)
    {
        //Add or remove an 'on' state.
        if (b)
        {
            amountOn++;
        } else
        {
            amountOn--;
        }

        if(amountOn >= amountNeeded)
        {
            if (connectedObjects.Length > 0)
            {
                for(int i = 0; i < connectedObjects.Length; i++)
                {
                    connectedObjects[i].getEnergyManager().updateObject(connectedObjects[i], true);
                }
                
            }

        } else
        {
            if (connectedObjects.Length > 0)
            {
                for (int i = 0; i < connectedObjects.Length; i++)
                {
                    connectedObjects[i].getEnergyManager().updateObject(connectedObjects[i], false);
                }
            }
        }
    }
}
