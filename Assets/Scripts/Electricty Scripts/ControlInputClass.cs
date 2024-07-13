using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlInputClass : ControlEnergyObject
{
    [SerializeField]
    string input;

    public override void setObject()
    {

        for(int i = 0; i < interactions_.Length; i++)
        {
            interactions_[i].useObject(input);
        }
    }
}
