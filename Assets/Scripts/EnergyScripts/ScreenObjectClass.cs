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

    private string currentCode = "";

    [SerializeField]
    private EnergyObjectClass affectedObj;

    [SerializeField]
    private MeshRenderer thescreen;

    [SerializeField]
    private Material screenMatOn;
    [SerializeField]
    private Material screenMatOff;

    [SerializeField]
    private String[] messages;



    private void Start()
    {
        //currentCode = "";

        if(String.Equals(currentCode, ""))
        {
            displayText(messages[0]);
        } else
        {
            displayText(currentCode);
        }
    }

    //A function that will power the current object.
    public override void powerObject(bool b)
    {
        isPowered = b;

        //Maybe will break. Will find out.
        if (String.Equals(currentCode, ""))
        {
            displayText(messages[0]);
        }
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
            if(currentCode.Length > 0)
            {
                currentCode = currentCode.Substring(0, currentCode.Length - 1);
            }


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
            currentCode = messages[0];

            displayText(currentCode);
        }
    }

    //A function that returns of current code is equal to command code.
    public bool isCurrentCode()
    {
        if(String.Equals(currentCode, commandCode))
        {
            displayText(messages[1]);
            currentCode = messages[1];
            return true;
        }

        displayText("Denied");
        return false;
    }

    //Change the text to what is written in currentCode.
    public void displayText(string s)
    {
        if (isPowered)
        {
            tex_.text = s;

            if(thescreen)
            {
                Material[] mats = thescreen.materials;
                mats[1] = screenMatOn;
                thescreen.materials = mats;
            }
        } else
        {
            if(thescreen)
            {
                Material[] mats = thescreen.materials;
                mats[1] = screenMatOff;
                thescreen.materials = mats;
            }

            tex_.text = messages[0];
        }

    }

    //An override to set the current code.
    public void setCurrentCode(string s)
    {
        currentCode = s;

        displayText(currentCode);
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

    public string getCurrentCode()
    {
        return currentCode;
    }
}
