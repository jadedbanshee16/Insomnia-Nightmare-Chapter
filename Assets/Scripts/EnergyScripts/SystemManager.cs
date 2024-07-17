using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This will manage several grids, turning them on or off. Each system will function on it’s own. This is linked to a power port and gets power from a generator.
 */
public class SystemManager : MonoBehaviour
{
    [SerializeField]
    GridManager[] managers;

    [SerializeField]
    float generatorPower;
    [SerializeField]
    float currentPower;

    //Generator gen;
    private void Start()
    {
        for(int i = 0; i < managers.Length; i++)
        {
            managers[i].setGrid(true);
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
        currentPower = 0;
        for(int i = 0; i < managers.Length; i++)
        {
            currentPower += managers[i].getPowerUsed();

            if(currentPower > generatorPower)
            {
                dePowerGrid(i);
            }
        }
    }

    /*public void setGenerator(GeneratorClass g)
    {
        generatorPower = g;
    }*/
}
