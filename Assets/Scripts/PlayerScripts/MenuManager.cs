using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //A function to use the save state functionality.
    public void saveButton()
    {
        if (!man_)
        {
            man_ = GameObject.FindGameObjectWithTag("GameManager");
        }

        man_.GetComponent<WorldStateManager>().saveNewState();
    }
}
