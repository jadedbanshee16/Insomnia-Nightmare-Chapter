using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This will manage several grids, turning them on or off. Each system will function on it�s own. This is linked to a power port and gets power from a generator.
 */
public class SystemManager : MonoBehaviour
{
    [SerializeField]
    GridManager[] managers;

    bool generatorPowered;

    [SerializeField]
    private float systemID;

    [SerializeField]
    float generatorPower;
    [SerializeField]
    float currentPower;

    [SerializeField]
    bool hasEssential;

    //Update this grid system.
    public void setManager()
    {
        for (int i = 0; i < managers.Length; i++)
        {
            managers[i].updateTheGrid();
        }

        if (generatorPower > 0)
        {
            generatorPowered = true;
        }

        powerSystems();
    }

    //A function that would turn off all 
    public void dePowerGrid(int ind)
    {

        managers[ind].tripGrid();
        managers[ind].setGrid(false);
    }

    //A function that will power each grid until it runs out of energy to give.
    public void powerSystems()
    {
        //If there is generator power, then complete the generator coding.
        if(generatorPower > 0 && generatorPowered)
        {
            currentPower = 0;
            bool tripped = false;

            //To ensure some grids that need to stay on will stay on.
            if (hasEssential)
            {
                for (int i = 0; i < managers.Length; i++)
                {
                    currentPower += managers[i].getPowerUsed();

                    if (currentPower > generatorPower)
                    {
                        tripped = true;

                        dePowerGrid(i);
                    }
                }
            } else
            {
                for (int i = 0; i < managers.Length; i++)
                {
                    currentPower += managers[i].getPowerUsed();

                    if (currentPower > generatorPower)
                    {
                        tripped = true;
                    }
                }

                if (tripped)
                {
                    for(int i = 0; i < managers.Length; i++)
                    {
                        dePowerGrid(i);
                    }

                    turnOffGenerator();
                }
            }

            if(tripped)
            {
                runGeneratorSound(4);
            }
        //If power is 0, then assume generator is turned off or unplugged, which means turn off grids without tripping system.
        } else
        {
            for (int i = 0; i < managers.Length; i++)
            {
                managers[i].setSystem(false);
            }

            currentPower = 0;
        }

    }

    public void setGeneratorPower(float newNumber)
    {
        generatorPower = newNumber;

        //Tell grid that system is powered.
        for (int i = 0; i < managers.Length; i++)
        {
            managers[i].setSystem(true);
        }

        powerSystems();
    }

    public void setGenerator(bool b)
    {
        generatorPowered = b;
    }

    public float getSystemId()
    {
        return systemID;
    }

    public void setSystemId(float i)
    {
        systemID = i;
    }

    private void runGeneratorSound(int v)
    {
        //Find all generators. Find which one is connected to this object, if any.
        GeneratorInteractionClass[] gens = FindObjectsByType<GeneratorInteractionClass>(FindObjectsSortMode.None);

        for(int i = 0; i < gens.Length; i++)
        {
            if(gens[i].getSystemManager() && gens[i].getSystemManager().getSystemId() == getSystemId())
            {
                //Play the given sound.
                gens[i].GetComponent<InteractionControlClass>().playInteractionAudio(v);
            }
        }
    }

    private void turnOffGenerator()
    {
        //Find all generators. Find which one is connected to this object, if any.
        GeneratorInteractionClass[] gens = FindObjectsByType<GeneratorInteractionClass>(FindObjectsSortMode.None);

        for (int i = 0; i < gens.Length; i++)
        {
            if (gens[i].getSystemManager() && gens[i].getSystemManager().getSystemId() == getSystemId())
            {
                //Play the given sound.
                gens[i].Interact(false);
            }
        }
    }

    /*public void setGenerator(GeneratorClass g)
    {
        generatorPower = g;
    }*/
}
