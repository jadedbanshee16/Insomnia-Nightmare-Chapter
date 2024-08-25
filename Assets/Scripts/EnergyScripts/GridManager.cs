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

    [SerializeField]
    bool systemPowered;

    SystemManager man_;

    //Update an object to be on or off in the system.
    public void updateObject(EnergyObjectClass obj, bool b)
    {
        for(int i = 0; i < objs.Length; i++)
        {
            if(obj.gameObject == objs[i].gameObject)
            {
                objs[i].setIsOn(b);
                objs[i].useObject();
            }
        }

        updateGrids();
    }

    //A function that will update the on and off state for each object object based on being on or off.
    public void updatePower()
    {
        if (systemPowered)
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
        } else
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
        if (!gridPowered)
        {
            return 0;
        }

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

        //Go through interactions and turn of the object as well as setting the Interaction classes.
        for(int i = 0; i < interactions.Length; i++)
        {
            interactions[i].turnOffObject();
        }

        for(int i = 0; i < objs.Length; i++)
        {
            objs[i].setIsOn(false);
        }

        //Find and turn off this grid interactions as well.
        GetComponentInChildren<GridInteractionClass>().setToOff();


    }

    //Set the grid to being on or off.
    public void setGrid(bool b)
    {
        gridPowered = b;
        updatePower();
    }

    //Set whether the system is gaining power.
    public void setSystem(bool b)
    {
        systemPowered = b;
        updatePower();
    }

    //A function that will return the current powered state of this grid object.
    public bool getIsOn()
    {
        return gridPowered;
    }

    //Update the systems this grid is a part of.
    public void updateGrids()
    {
        man_.powerSystems();
    }

    public void updateTheGrid()
    {
        objs = GetComponentsInChildren<EnergyObjectClass>();

        man_ = GetComponentInParent<SystemManager>();

        for (int i = 0; i < objs.Length; i++)
        {
            objs[i].setEnergyManager(this.GetComponent<GridManager>());
        }

        setGrid(true);

        //Go through all interaction control classes in this system and update them with the basic updates.
        InteractionControlClass[] controllers = GetComponentsInChildren<InteractionControlClass>();
        for(int i = 0; i < controllers.Length; i++)
        {
            controllers[i].updateThisInteraction();
        }

        //Now, find the switch linked to this system, if it has a link (this should be MUST) and set it to whatever position is currently on.
        if (GetComponentInChildren<GridInteractionClass>())
        {
            GetComponentInChildren<GridInteractionClass>().updateGridSwitch();
        }


    }
}
