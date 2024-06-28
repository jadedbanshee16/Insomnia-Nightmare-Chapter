using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorClass : EnergyObject
{
    [SerializeField]
    private float powerProvided;

    [SerializeField]
    private EnergizerScript currentGrid;


    // Start is called before the first frame update
    void Start()
    {
        //Make sure generator has a connected cable.
        if (GetComponentInChildren<PlugClass>())
        {
            GetComponentInChildren<PlugClass>().setMainObject(this.GetComponent<GeneratorClass>());
        }
    }

    public override void useObject(bool b)
    {
        setPower(b);
    }

    public float getPowerAmount()
    {
        return powerProvided;
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
        currentGrid = script;

        //Set this in the controller as well.
        GetComponentInChildren<ControlEnergyObject>().setPowerObject(currentGrid);
    }

    public void unSetGrid()
    {
        currentGrid = null;
        GetComponentInChildren<ControlEnergyObject>().setPowerObject(null);
    }
}
