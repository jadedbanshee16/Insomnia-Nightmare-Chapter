using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPage : MonoBehaviour
{
    [SerializeField]
    GameObject[] pageItems;

    [SerializeField]
    string defaultScreen;

    //A function to turn off and on any menu group selected.
    public void updateMenuGroup(int ind, bool isOn)
    {
        pageItems[ind].gameObject.SetActive(isOn);
    }

    //A function to find and set one particular menu to run.
    public void setToMenuGroup(string name)
    {
        for (int i = 0; i < pageItems.Length; i++)
        {
            if (string.Equals(pageItems[i].gameObject.name, name))
            {
                updateMenuGroup(i, true);
            }
            else
            {
                updateMenuGroup(i, false);
            }
        }
    }

    public string getDefaultPage()
    {
        return defaultScreen;
    }
}
