using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] groups;

    [SerializeField]
    GameObject[] externalMessage;

    private GameObject man_;

    ButtonScript[] dynamicButtons = new ButtonScript[0];

    ScrollScript[] dynamicScrolls = new ScrollScript[0];

    private bool isSearchingForIntButton = false;

    private bool isDisplayingMessage = false;
    float promptTime = 1;
    float promptTimer = 0;

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

    private void addMenuGroup(string name)
    {
        //Find an activate a specific menu group on top of the other menu groups, instead of turning the others to false.
        for (int i = 0; i < groups.Length; i++)
        {
            if (string.Equals(groups[i].gameObject.name, name))
            {
                updateMenuGroup(i, true);
            }
        }
    }

    private void removeMenuGroup(string name)
    {
        //Find an activate a specific menu group on top of the other menu groups, instead of turning the others to false.
        for (int i = 0; i < groups.Length; i++)
        {
            if (string.Equals(groups[i].gameObject.name, name))
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

    //A function that will quit the entire application.
    public void quitButton()
    {
        Application.Quit();
    }

    //A function to load a particular scene.
    public void loadScene(int ind)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(ind);
    }

    public void saveOptions()
    {
        if (!man_)
        {
            man_ = GameObject.FindGameObjectWithTag("GameManager");
        }

        if (man_.GetComponent<OptionsManager>())
        {
            man_.GetComponent<OptionsManager>().saveControls(false);
        }
    }

    public void loadOptions()
    {
        if (!man_)
        {
            man_ = GameObject.FindGameObjectWithTag("GameManager");
        }

        if (man_.GetComponent<OptionsManager>())
        {
            man_.GetComponent<OptionsManager>().setUpControls();
            man_.GetComponent<GameManager>().updateAudio();
        }
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

    //When activated, this will find all buttons with dynamic names.
    //Then, it would find what type of button it is and adjust to the given text.
    public void updateButtonNames()
    {
        //If no buttons, try to find buttons.
        if (dynamicButtons.Length <= 0)
        {
            dynamicButtons = this.GetComponentsInChildren<ButtonScript>();

            //Debug.Log("Worked?: " + dynamicButtons.Length);
        }

        if (!man_)
        {
            man_ = GameObject.FindGameObjectWithTag("GameManager");
        }

        OptionsManager opt = man_.GetComponent<OptionsManager>();

        //If still no buttons, then ignore.
        if (dynamicButtons.Length > 0)
        {
            for(int i = 0; i < dynamicButtons.Length; i++)
            {
                //Update text to what is matched in the options manager.
                dynamicButtons[i].adjustButtonText(opt.getControl(dynamicButtons[i].getControlType()).ToString());
            }
        }
    }

    //Change and update the value of the audio.
    public void updateAudio()
    {
        if (!man_)
        {
            man_ = GameObject.FindGameObjectWithTag("GameManager");
        }

        OptionsManager opt = man_.GetComponent<OptionsManager>();

        //opt.setMasterVolume()
        if (dynamicScrolls.Length <= 0)
        {
            dynamicScrolls = this.GetComponentsInChildren<ScrollScript>();

            //Debug.Log("Worked?: " + dynamicButtons.Length);
        }

        if (dynamicScrolls.Length > 0)
        {
            for (int i = 0; i < dynamicScrolls.Length; i++)
            {
                //Update text to what is matched in the options manager.
                dynamicScrolls[i].adjustScroll(opt.getVolume(dynamicScrolls[i].getScrollType()));
            }
        }
    }

    public void changeAudio(string type, float v)
    {
        if (!man_)
        {
            man_ = GameObject.FindGameObjectWithTag("GameManager");
        }

        OptionsManager opt = man_.GetComponent<OptionsManager>();

        opt.setVolume(type, v);
        updateAudio();
    }

    public void findButton(int ind)
    {
        isSearchingForIntButton = true;
        StartCoroutine(findNewButton(ind));
    }

    IEnumerator findNewButton(int index)
    {
        if (!man_)
        {
            man_ = GameObject.FindGameObjectWithTag("GameManager");
        }

        OptionsManager opt = man_.GetComponent<OptionsManager>();

        while (isSearchingForIntButton)
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(kcode))
                {
                    opt.updateControl(index, kcode);
                    isSearchingForIntButton = false;
                    updateButtonNames();
                }
            }

            yield return null;
        }

        //End this current coroutine. You don't need it to run anymore.
        StopCoroutine(findNewButton(index));
    }

    //A function which finds the first text of a given menu item and updates that to whatever prompt is given.
    public void updateText(string ind, string s)
    {
        //first, add the given menu group.
        externalMessage[0].SetActive(true);

        //Change prompt of text.
        externalMessage[0].GetComponentInChildren<TextMeshProUGUI>().text = s;
        Color col = externalMessage[0].GetComponentInChildren<Image>().color;

        isDisplayingMessage = true;
        promptTimer = promptTime;
        StartCoroutine(promptCoRoutine(externalMessage[0].GetComponentInChildren<Image>(), ind));
    }

    IEnumerator promptCoRoutine(Image img, string name)
    {
        Color col = img.color;

        while (isDisplayingMessage)
        {
            //Go through and set to smaller.
            if(promptTimer > 0)
            {
                promptTimer -= Time.deltaTime / 3;

                //Start changing alpha until it is ready.
                if(promptTimer <= promptTime / 2)
                {
                    col.a = promptTimer / (promptTime / 2);
                    img.color = col;
                }
            } else
            {
                isDisplayingMessage = false;
                col.a = 1;
                img.color = col;
                externalMessage[0].SetActive(false);
                StopCoroutine(promptCoRoutine(img, name));
            }

            yield return null;
        }
    }
}
