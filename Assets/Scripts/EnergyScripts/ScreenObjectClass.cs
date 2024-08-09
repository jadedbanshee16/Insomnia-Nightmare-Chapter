using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenObjectClass : EnergyObjectClass
{
    [SerializeField]
    TextMeshProUGUI tex_;

    [SerializeField]
    int inputSize;

    [SerializeField]
    string commandCode;

    private string currentCode;

    [SerializeField]
    private EnergyObjectClass affectedObj;

    private void Start()
    {
        currentCode = "";
        displayText(currentCode);
    }

    //A function that will power the current object.
    public override void powerObject(bool b)
    {
        isPowered = b;
        displayText(currentCode);
    }

    //Combine the added string to the current string.
    public void addString(string s)
    {
        if (isPowered)
        {
            currentCode = currentCode + s;

            displayText(currentCode);
        }
    }

    //Remove the last element of the string.
    public void removeString()
    {
        if (isPowered)
        {
            currentCode = currentCode.Substring(0, currentCode.Length - 1);

            displayText(currentCode);
        }

    }

    //Return the current affected object of this screen.
    public EnergyObjectClass getAffectedObject()
    {
        return affectedObj;
    }

    //Make string empty.
    public void clearString()
    {
        if (isPowered)
        {
            currentCode = "";

            displayText(currentCode);
        }
    }

    //A function that returns of current code is equal to command code.
    public bool isCurrentCode()
    {
        if(String.Equals(currentCode, commandCode))
        {
            displayText("Accessed");
            return true;
        }

        displayText("Denied");
        return false;
    }

    //Change the text to what is written in currentCode.
    private void displayText(string s)
    {
        if (isPowered)
        {
            tex_.text = s;
        } else
        {
            tex_.text = "err: No pwr";
        }

    }

    //A function to set the energy manager of this object.
    public override void setEnergyManager(GridManager man)
    {
        energyManager = man;

        //When manager is set, set on depending on energyObject type. Default is off.
        isOn = true;
    }

    //Set the current on state of the object.
    public override void setIsOn(bool b)
    {
        //This item cannot be turned off.
        isOn = true;
    }
}
