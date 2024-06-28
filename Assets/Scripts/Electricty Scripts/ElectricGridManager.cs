using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricGridManager : MonoBehaviour
{
    [SerializeField]
    PowerPortClass powerPoint;

    bool internalPower;

    bool powerOn;

    [SerializeField]
    EnergyObject[] toPower;
    private bool[] systems;

    SwitchBoard board_;

    private float energyAmount = 250;
    private float energyUsed;
    //private bool energized;


    //Set up the system with all energy objects.
    private void Start()
    {
        //Get all energy objects as children.
        toPower = this.GetComponentsInChildren<EnergyObject>();

        powerPoint = this.GetComponentInChildren<PowerPortClass>();

        //Set power on based on if using internal power.
        if (!powerPoint)
        {
            internalPower = false;
            powerOn = false;
            energyAmount = 0;
        } else
        {
            powerOn = true;
            internalPower = true;
        }

        //Make open types the same length as the energyobject type enum. Set all to false.
        systems = new bool[System.Enum.GetValues(typeof(EnergyObject.objectType)).Length];

        if (GetComponentInChildren<SwitchBoard>())
        {
            board_ = this.GetComponentInChildren<SwitchBoard>();
        }


        /*energyAmount = 0;
        energized = false;
        //Make open types the same length as the energyobject type enum. Set all to false.
        systems = new bool[System.Enum.GetValues(typeof(EnergyObject.objectType)).Length];*/
    }
}
