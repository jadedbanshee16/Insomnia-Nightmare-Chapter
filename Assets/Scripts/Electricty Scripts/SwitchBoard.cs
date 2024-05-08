using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBoard : MonoBehaviour
{
    SwitchClass[] switches;
    // Start is called before the first frame update
    void Start()
    {
        switches = GetComponentsInChildren<SwitchClass>();
    }

    //Turn off all switches in the current switchboard.
    public void turnOffSwitches()
    {
        for(int i = 0; i < switches.Length; i++)
        {
            switches[i].turnOff();
        }
    }
}
