using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergizerScript : MonoBehaviour
{
    [SerializeField]
    EnergyObject[] toPower;
    public bool[] systems;

    [SerializeField]
    SwitchBoard board_;

    private float energyAmount;
    private float energyUsed;
    private bool energized;



    private void Start()
    {
        energyAmount = 0;
        energized = false;
        //Make open types the same length as the energyobject type enum. Set all to false.
        systems = new bool[System.Enum.GetValues(typeof(EnergyObject.objectType)).Length];
    }

    //Energize every item connected to this power source.
    public void energize()
    {
        energyUsed = energyAmount;
        //Cycle through all power items and check for energyObject component.
        //Add energy.
        for(int i = 0; i < toPower.Length; i++)
        {
            //Check if current system is online.
            if (checkIfTypeIsOn(toPower[i].getType()))
            {
                //If energy output is made less than zero, trip the system.
                if (energyUsed - toPower[i].getEnergyAmount() < 0)
                {
                    deEnergize();
                    //Also remove all switches in current switchboard connected to this.
                    if (board_)
                    {
                        board_.turnOffSwitches();
                    }
                    energyUsed = 0;
                }
                else
                {
                    energyUsed -= toPower[i].getEnergyAmount();
                    toPower[i].powerObject();
                }
            } else
            {
                //If the type is off, depower the object.
                toPower[i].dePowerObject();
            }

        }
    }

    //Remove energy to everything connected to this power source.
    public void deEnergize()
    {
        /*
         * Same as with energize.
         */
        for (int i = 0; i < toPower.Length; i++)
        {
            if (toPower[i])
            {
                toPower[i].dePowerObject();
            }
        }
    }

    //A function to see if the type matches a system and if that system is online.
    public bool checkIfTypeIsOn(EnergyObject.objectType t)
    {
        //Go through the systems and match the type to system.
        //Return whether system is on or not.
        for(int i = 0; i < systems.Length; i++)
        {
            if((int)t == i)
            {
                return systems[i];
            }
        }

        return false;
    }

    //A function to set the powersource of this energizer.
    public void setPowersource(float energy, bool on)
    {
        if (on)
        {
            energyAmount = energy;
            energized = true;
            energize();
        } else
        {
            energyAmount = 0;
            energized = false;
            deEnergize();
        }
    }

    //A function to adjust one of the systems. If already energized, then energize.
    public void setSystem(int ind, bool on)
    {
        systems[ind] = on;

        if (energized)
        {
            energize();
        }
    }

}
