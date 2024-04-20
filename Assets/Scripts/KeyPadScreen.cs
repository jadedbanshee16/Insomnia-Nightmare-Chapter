using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyPadScreen : MonoBehaviour
{
    TextMeshProUGUI text_;

    string currentString = "";

    [SerializeField]
    int inputSize = 4;

    [SerializeField]
    string theCode;

    [SerializeField]
    DoorClass door;

    // Start is called before the first frame update
    void Start()
    {
        text_ = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        text_.text = currentString;

        if(string.Equals(theCode, ""))
        {
            theCode = "0000";
        }
    }

    //A function to add a new string to text.
    public void addString(string newString)
    {
        string newTex = string.Concat(currentString, newString);

        if(newTex.Length <= inputSize)
        {
            currentString = newTex;
            text_.text = currentString;
        }
    }

    //Remove the text string.
    public void clearString()
    {
        currentString = "";

        text_.text = currentString;
    }

    //A function to test code, and open door if it works.
    public void enterString()
    {
        if(string.Equals(currentString, theCode))
        {
            door.openDoor();
        } else
        {
            door.closeDoor();
        }
    }
}
