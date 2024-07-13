using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBoard : MonoBehaviour
{
    [SerializeField]
    ControlEnergyObject[] switches;

    [SerializeField]
    private ControlEnergyObject.interactionType[] prioritizeSystems;

    // Start is called before the first frame update
    void Start()
    {
        //Find all switches on the grid.
        switches = GetComponentInParent<EnergizerScript>().gameObject.GetComponentsInChildren<ControlEnergyObject>();
    }

    //Turn off all switches in the current switchboard.
    public void turnOffSwitches()
    {
        //Turn off the switches.
        for(int i = 0; i < switches.Length; i++)
        {
            //Go through the prioritized systems and turn off if not on the list.
            bool turnOff = true;

            for(int v = 0; v < prioritizeSystems.Length; v++)
            {
                if(switches[i].getPowerType() == prioritizeSystems[v])
                {
                    turnOff = false;
                }
            }

            //If not on list of prioritized systems, then turn off.
            if (turnOff)
            {
                //switches[i].setPower(false);
                SwitchClass swtch = switches[i].gameObject.GetComponent<SwitchClass>();

                if (swtch)
                {
                    swtch.turnOff();
                }
            }

        }

        //Turn off the generator if a generator is connected.
        GeneratorClass gen = GetComponentInParent<EnergizerScript>().getGenerator();

        if (gen)
        {
            //Now, deenergize the generator.
            //gen.gameObject.GetComponentInChildren<ControlEnergyObject>().setPower(false);
            gen.gameObject.GetComponentInChildren<SwitchClass>().turnOff();
        }
    }
}
