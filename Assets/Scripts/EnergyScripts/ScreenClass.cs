using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenClass : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI tex_;

    [SerializeField]
    private MeshRenderer thescreen;

    [SerializeField]
    private string offText;

    [SerializeField]
    private Material screenMatOn;
    [SerializeField]
    private Material screenMatOff;



    public void displayText(string s, bool isPowered)
    {
        if (isPowered)
        {
            tex_.text = s;

            if (thescreen)
            {
                Material[] mats = thescreen.materials;
                mats[1] = screenMatOn;
                thescreen.materials = mats;
            }
        }
        else
        {
            if (thescreen)
            {
                Material[] mats = thescreen.materials;
                mats[1] = screenMatOff;
                thescreen.materials = mats;
            }

            tex_.text = offText;
        }

    }

    public string getCurrentString()
    {
        return tex_.text;
    }
}
