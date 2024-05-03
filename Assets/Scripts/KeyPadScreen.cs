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
    DoorClass door;

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

        dePowerObject();
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
            door.setOpened(true);
        }
        else
        {
            door.setOpened(false);
        }
    }

    //An override to power this object and change display.
    public override void powerObject()
    {
        //Power itself.
        powered = true;
        displayText(currentString);
    }

    //And override to depower this object and change display.
    public override void dePowerObject()
    {
        powered = false;
        displayText("Err: No Pwr");
    }

    //Display the text provided.
    public void displayText(string tex)
    {
        text_.text = tex;
    }

    //Get the current string provided.
    public string getCurrentString()
    {
        return currentString;
    }
}
