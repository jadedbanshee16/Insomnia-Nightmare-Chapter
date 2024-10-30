using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollScript : MonoBehaviour
{
    [SerializeField]
    string val;

    public void adjustScroll(float v)
    {
        //Find the text.
        if (this.GetComponent<Scrollbar>())
        {
            this.GetComponent<Scrollbar>().value = v;
        }
    }

    public string getScrollType()
    {
        return val;
    }

    public void changeVal()
    {
        GetComponentInParent<MenuManager>().changeAudio(val, GetComponent<Scrollbar>().value);
    }
}
