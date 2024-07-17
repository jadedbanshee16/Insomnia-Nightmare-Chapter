using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This is the manager of the grid. It keeps all the energy objects and manages interactions to turn them on and off.

This also turns off all switches if too much power is routed through it. WARNING: This will look for all energyInteraction classes currently under it’s system. No others. Need system for generator as well.
*/
public class GridManager : MonoBehaviour
{
    EnergyObjectClass[] objs;

    [SerializeField]
    bool gridPowered;

    SystemManager man_;

    private void Start()
    {
        objs = GetComponentsInChildren<EnergyObjectClass>();

        man_ = GetComponentInParent<SystemManager>();

        for(int i = 0; i < objs.Length; i++)
        {
            objs[i].setEnergyManager(this.GetComponent<GridManager>());
        }
    }

    //Update an object to be on or off in the system.
    public void updateObject(EnergyObjectClass obj, bool b)
    {
        for(int i = 0; i < objs.Length; i++)
        {
            if(obj.gameObject.name == objs[i].gameObject.name)
            {
                objs[i].setIsOn(b);
                objs[i].useObject();
            }
        }

        man_.powerSystems();
    }

    //A function that will update the on and off state for each object object based on being on or off.
    public void updatePower()
    {
        if (gridPowered)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i].powerObject(true);
            }
        }
        else
        {
            for (int i = 0; i < objs.Length; i++)
            {
                objs[i].powerObject(false);
            }
        }
    }

    //Return the amount of power being used.
    public float getPowerUsed()
    {
        float powerUsed = 0;

        for(int i = 0; i < objs.Length; i++)
        {
            if (objs[i].getIsOn())
            {
                powerUsed += objs[i].getEnergyAmount();
            }
        }

        return powerUsed;
    }

    //A function that will trip and turn off all interactions in a given grid system.
    public void tripGrid()
    {

        /*
         * WARNING: Ensure all interactions and nergy objects are children of this grid manager for this to work.
         */
        EnergyInteractionClass[] interactions = GetComponentsInChildren<EnergyInteractionClass>();

        for(int i = 0; i < interactions.Length; i++)
        {
            interactions[i].turnOffObject();
        }

        for(int i = 0; i < objs.Length; i++)
        {
            objs[i].setIsOn(false);
        }
    }

    //Set the grid to being on or off.
    public void setGrid(bool b)
    {
        gridPowered = b;

        updatePower();
    }
}
