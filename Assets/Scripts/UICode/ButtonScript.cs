using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField]
    OptionsManager.theControls controlType;


    public void adjustButtonText(string text)
    {
        //Find the text.
        if (this.GetComponentInChildren<TextMeshProUGUI>())
        {
            this.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }
    }

    public OptionsManager.theControls getControlType()
    {
        return controlType;
    }
}
