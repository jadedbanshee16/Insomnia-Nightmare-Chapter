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

    private bool energized;

    // Start is called before the first frame update
    void Start()
    {
        text_ = gameObject.GetComponentInChildren<TextMeshProUGUI>();

        text_.text = currentString;

        if(string.Equals(theCode, ""))
        {
            theCode = "0000";
        }

        energized = false;
    }

    //A function to add a new string to text.
    public void addString(string newString)
    {
        if (energized)
        {
            string newTex = string.Concat(currentString, newString);

            if (newTex.Length <= inputSize)
            {
                currentString = newTex;
                text_.text = currentString;
            }
        } else
        {
            text_.text = "";
        }
    }

    //Remove the text string.
    public void clearString()
    {
        if (energized)
        {
            currentString = "";

            text_.text = currentString;
        }
    }

    //A function to test code, and open door if it works.
    public void enterString()
    {
        if (energized)
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

    }

    public void energizeItem(bool b)
    {
        energized = b;

        //Make the system run once after being energized or not.
        addString("");
    }
}
