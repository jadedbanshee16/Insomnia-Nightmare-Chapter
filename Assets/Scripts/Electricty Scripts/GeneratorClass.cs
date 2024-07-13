using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorClass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Make sure generator has a connected cable.
        if (GetComponentInChildren<PlugClass>())
        {
            GetComponentInChildren<PlugClass>().setMainObject(this.GetComponent<GeneratorClass>());
        }
    }

    /*public void useObject()
    {
        setPower(b);
    }

    public float getPowerAmount()
    {
        return energyUsage;
    }

    public bool getPowerOn()
    {
        return powered;
    }

    public void setPower(bool b)
    {
        powered = b;
    }

    public void setGrid(EnergizerScript script)
    {
        powerBox = script;

        //Set this in the controller as well.
        GetComponentInChildren<ControlEnergyObject>().setPowerObject(powerBox);
    }

    public void unSetGrid()
    {
        powerBox = null;
        GetComponentInChildren<ControlEnergyObject>().setPowerObject(null);
    }*/
}
