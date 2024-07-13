using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyPadScreen : EnergyObject
{
    TextMeshProUGUI text_;

    string currentString = "";

    [SerializeField]
    int inputSize = 4;

    [SerializeField]
    string theCode;

    [SerializeField]
    EnergyObject usableObject;

    //private bool energized;

    // Start is called before the first frame update
    void Start()
    {
        text_ = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        displayText(currentString);

        if(string.Equals(theCode, ""))
        {
            theCode = "0000";
        }

        powerObject(true);
    }

    //A function to add a new string to text.
    public void addString(string newString)
    {
        string newTex = string.Concat(currentString, newString);

        if (newTex.Length <= inputSize)
        {
            currentString = newTex;
            displayText(currentString);
        }
    }

    //Remove the text string.
    public void clearString()
    {
        currentString = "";

        displayText(currentString);
    }

    //A function to test code, and open door if it works.
    public void enterString()
    {
        if (string.Equals(currentString, theCode))
        {
            usableObject.gameObject.GetComponent<ControlEnergyObject>().setObject(true);
            //powerBox.energize();
        }
        else
        {
            usableObject.gameObject.GetComponent<ControlEnergyObject>().setObject(false);
        }
    }

    //An override to power this object and change display.
    public override void powerObject(bool b)
    {
        //Power itself.
        powered = b;

        if (powered)
        {
            displayText(currentString);
        } else
        {
            displayText("Err: No Pwr");
        }

    }

    public override void useObject(string input)
    {
        //Do nothing as this energy object is unusable.
        if (string.Equals(input, "Clear"))
        {
            //This is the delete button.
            if (powered)
            {
                clearString();
            }
        }
        else if (string.Equals(input, "Enter"))
        {
            //If enter is pressed, then make it test the current string.
            if (powered)
            {
                enterString();
            }
        }
        else
        {
            //If it's anything else, then it's a number. Add string.
            if (powered)
            {
                addString(input);
            }
        }
    }

    //Display the text provided.
    public void displayText(string tex)
    {
        text_.text = tex;
    }
}
