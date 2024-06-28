using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBoard : MonoBehaviour
{
    [SerializeField]
    ControlEnergyObject[] switches;

    // Start is called before the first frame update
    void Start()
    {
        //Find all switches on the grid.
        switches = GetComponentInParent<EnergizerScript>().gameObject.GetComponentsInChildren<ControlEnergyObject>();
    }

    //Turn off all switches in the current switchboard.
    public void turnOffSwitches()
    {
        for(int i = 0; i < switches.Length; i++)
        {

            switches[i].setPower(false);
            SwitchClass swtch = switches[i].gameObject.GetComponent<SwitchClass>();

            if (swtch)
            {
                swtch.turnOff();
            }
        }
    }
}
