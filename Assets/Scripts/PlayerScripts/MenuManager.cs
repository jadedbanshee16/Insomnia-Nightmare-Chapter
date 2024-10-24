using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] groups;

    private GameObject man_;

    //A function to turn off and on any menu group selected.
    public void updateMenuGroup(int ind, bool isOn)
    {
        groups[ind].SetActive(isOn);
    }

    //A function to find and set one particular menu to run.
    public void setToMenuGroup(string name)
    {
        for(int i = 0; i < groups.Length; i++)
        {
            if(string.Equals(groups[i].gameObject.name, name))
            {
                updateMenuGroup(i, true);
            } else
            {
                updateMenuGroup(i, false);
            }
        }
    }

    public GameObject getMenuGroup(string name)
    {
        for (int i = 0; i < groups.Length; i++)
        {
            if (string.Equals(groups[i].gameObject.name, name))
            {
                return groups[i];
            }
            else
            {
                updateMenuGroup(i, false);
            }
        }

        return null;
    }

    //A function to use the save state functionality.
    public void saveButton()
    {
        if (!man_)
        {
            man_ = GameObject.FindGameObjectWithTag("GameManager");
        }

        man_.GetComponent<WorldStateManager>().saveNewState();
    }

    public void adjustLoadValue(float alpha)
    {
        if (alpha > 0)
        {
            //Debug.Log(alpha);
            setToMenuGroup("LoadBlack");
            Image loadPanel = getMenuGroup("LoadBlack").GetComponent<Image>();

            Color col = loadPanel.color;

            col.a = alpha;

            //Adjust colour to new value.
            loadPanel.color = col;
        }
        else
        {
            //Set to main using group.
            setToMenuGroup("Stylus");
        }
    }
}
