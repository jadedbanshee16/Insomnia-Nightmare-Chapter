using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergizerScript : MonoBehaviour
{
    public EnergyObject[] toPower;
    public bool[] systems;
    public SwitchBoard board_;
    public PowerPortClass plug;

    /*bool powerOn;

    [SerializeField]
    private float energyAmount;*/

    [SerializeField]
    private GeneratorClass currentGenerator;

    private bool usingInternalGenerator;

    [SerializeField]
    private float energyUsed;



    private void Start()
    {
        //Get all energy objects as children.
        toPower = this.GetComponentsInChildren<EnergyObject>();

        //Make open types the same length as the energyobject type enum. Set all to false.
        systems = new bool[System.Enum.GetValues(typeof(EnergyObject.objectType)).Length];

        plug = GetComponentInChildren<PowerPortClass>();

        for(int i = 0; i < toPower.Length; i++)
        {
            toPower[i].setPowerBox(this.GetComponent<EnergizerScript>());
        }

        //Set power on based on if using internal power.
        if (plug)
        {
            usingInternalGenerator = false;
            //Set the power port with this energizer script.
            GetComponentInChildren<PowerPortClass>().setPowerPort(this.GetComponent<EnergizerScript>());
        }
        else
        {
            usingInternalGenerator = true;
        }

        if (GetComponentInChildren<SwitchBoard>())
        {
            board_ = this.GetComponentInChildren<SwitchBoard>();
        }

        //Make open types the same length as the energyobject type enum. Set all to false.
        systems = new bool[System.Enum.GetValues(typeof(EnergyObject.objectType)).Length];

        //Now, go through and find all the switches that affect energy objects. Ensure all of them are linked to this object.
        ControlEnergyObject[] controlObjects = GetComponentsInChildren<ControlEnergyObject>();

        for(int i = 0; i < controlObjects.Length; i++)
        {
            controlObjects[i].setPowerObject(this.GetComponent<EnergizerScript>());
        }
    }

    //Energize every item connected to this power source.
    public void energize()
    {
        energyUsed = 0;

        if (usingInternalGenerator)
        {
            energyUsed = 250;
        } else
        {
            if (currentGenerator && currentGenerator.getPowerOn())
            {
                energyUsed = currentGenerator.getEnergyAmount();
            }
        }

        if(energyUsed > 0)
        {
            //Cycle through all power items and check for energyObject component.
            //Add energy.
            for (int i = 0; i < toPower.Length; i++)
            {
                //Check if current system is online.
                if (checkIfTypeIsOn(toPower[i].getType()))
                {
                    //If energy output is made less than zero, trip the system.
                    if (toPower[i].isUsed() && energyUsed - toPower[i].getEnergyAmount() < 0)
                    {
                        deEnergize(true);
                        energyUsed = 0;
                    }
                    else if (toPower[i].isUsed())
                    {
                        energyUsed -= toPower[i].getEnergyAmount();
                        toPower[i].powerObject();
                    }
                }
                else
                {
                    //If the type is off, depower the object.
                    toPower[i].dePowerObject();
                }

            }
        } else
        {
            for(int i = 0; i < toPower.Length; i++)
            {
                toPower[i].dePowerObject();
            }
        }

    }

    //This will take a float number and check if adding the extra number to energy used will cause a fault.
    public void checkForTooMuchPower(float energy)
    {
        if(energyUsed - energy < 0)
        {
            //If too much power, turn off all objects.
            deEnergize(true);
        } else
        {
            energyUsed -= energy;
        }
    }

    //Remove energy to everything connected to this power source.
    public void deEnergize(bool shorting)
    {
        /*
         * Same as with energize, just for depowering.
         */
        for (int i = 0; i < toPower.Length; i++)
        {
            if (toPower[i])
            {
                toPower[i].dePowerObject();
            }
        }

        //Also remove all switches in current switchboard connected to this.
        if (board_ && shorting)
        {
            board_.turnOffSwitches();
        }

        if (currentGenerator)
        {
            //Now, deenergize the generator.
            currentGenerator.gameObject.GetComponentInChildren<ControlEnergyObject>().setPower(false);
            currentGenerator.gameObject.GetComponentInChildren<SwitchClass>().turnOff();
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
    public void setPowersource(GeneratorClass gen)
    {
        if (gen)
        {
            currentGenerator = gen;
            currentGenerator.setGrid(this.GetComponent<EnergizerScript>());
            energize();
        } else
        {
            currentGenerator = null;
            deEnergize(false);
        }
    }

    //A function to adjust one of the systems. If already energized, then energize.
    public void setSystem(int ind, bool on)
    {
        systems[ind] = on;

        if (currentGenerator)
        {
            energize();
        }
    }

}
