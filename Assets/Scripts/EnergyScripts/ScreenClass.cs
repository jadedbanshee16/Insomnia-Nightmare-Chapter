using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenClass : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI tex_;

    [SerializeField]
    private string thisScreen;

    private Canvas canv;

    private void Start()
    {
        canv = GetComponentInParent<Canvas>();
    }



    public void displayText(string s)
    {
        if (!canv)
        {
            canv = GetComponentInParent<Canvas>();
        }

        //Set canvas to on. do this every time.
        canv.enabled = true;

        tex_.text = s;
    }

    public string getCurrentString()
    {
        return tex_.text;
    }
}
