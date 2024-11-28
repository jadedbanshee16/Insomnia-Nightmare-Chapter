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

    [SerializeField]
    private string thisScreen;

    private Canvas canv;

    private void Start()
    {
        canv = GetComponentInParent<Canvas>();
    }



    public void displayText(string s, bool isPowered)
    {
        if (!canv)
        {
            canv = GetComponentInParent<Canvas>();
        }

        if (isPowered)
        {
            //Set canvas to on. do this every time.
            canv.enabled = true;

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

            //Set the canvas to false to turn off the screen.
            canv.enabled = false;
        }

    }

    public string getCurrentString()
    {
        return tex_.text;
    }
}
