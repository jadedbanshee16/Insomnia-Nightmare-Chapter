using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonClass : MonoBehaviour
{
    private enum updateOptionType
    {
        options,
        audio,
        achievements
    }

    [SerializeField]
    TextMeshProUGUI tex;

    [SerializeField]
    protected MenuManager menuManager_;

    [SerializeField]
    updateOptionType[] updatedTypes;

    public void adjustButtonText(string text)
    {
        //Find the text.
        if (tex)
        {
            tex.text = text;
        }
    }

    public virtual void useButton()
    {
        //ButtonInteraction.
        if (menuManager_)
        {
            for(int i = 0; i < updatedTypes.Length; i++)
            {
                if(updatedTypes[i] == updateOptionType.options)
                {
                    menuManager_.saveOptions();
                    menuManager_.loadOptions();
                    //Debug.Log("Made it?");
                    menuManager_.updateButtonNames();
                } else if(updatedTypes[i] == updateOptionType.audio)
                {
                    menuManager_.updateOptionValues();
                    menuManager_.loadOptions();
                } else if(updatedTypes[i] == updateOptionType.achievements)
                {
                    menuManager_.loadAchievements();
                }
            }
        }
    }
}
