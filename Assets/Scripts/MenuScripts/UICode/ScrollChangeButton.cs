using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollChangeButton : ButtonClass
{
    [SerializeField]
    string value;

    Scrollbar scrollbar_;

    public void adjustScroll(float v)
    {
        //Find the text.
        if (!scrollbar_)
        {
            scrollbar_ = this.GetComponent<Scrollbar>();
        }

        scrollbar_.value = v;

        base.useButton();
    }

    public string getScrollType()
    {
        return value;
    }

    public void changeVal()
    {
        //Find the text.
        if (!scrollbar_)
        {
            scrollbar_ = this.GetComponent<Scrollbar>();
        }

        if (menuManager_)
        {
            menuManager_.changeScrollValue(value, scrollbar_.value);
            menuManager_.loadAudio();
            menuManager_.saveOptions();
        }
        //To update the current values with the currect audio levels.

    }
}
