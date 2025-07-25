using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    MenuPage[] menuPages;

    [SerializeField]
    GameObject[] externalMessage;

    private GameObject man_;

    Scrollbar[] dynamicScrolls = new Scrollbar[0];

    private bool isDisplayingMessage = false;
    float promptTime = 1;
    float promptTimer = 0;

    private float menuID;

    public bool lockMenu = false;

    //A function to turn off and on any menu group selected.
    public void updateMenuGroup(int ind, bool isOn)
    {
        menuPages[ind].gameObject.SetActive(isOn);
    }

    //A function to find and set one particular menu to run.
    public void setToMenuGroup(string name)
    {
        //This may cause lock issues.
        //Affectively lockMenu should only be true when leaving the playfield or killed by the haunter.
        if (!lockMenu)
        {
            for (int i = 0; i < menuPages.Length; i++)
            {
                if (string.Equals(menuPages[i].gameObject.name, name))
                {
                    updateMenuGroup(i, true);
                }
                else
                {
                    updateMenuGroup(i, false);
                }
            }
        }
    }

    /*public void addMenuGroup(string name)
    {
        //Debug.Log("added");
        //Find an activate a specific menu group on top of the other menu groups, instead of turning the others to false.
        for (int i = 0; i < menuPages.Length; i++)
        {
            if (string.Equals(menuPages[i].gameObject.name, name))
            {
                updateMenuGroup(i, true);
            }
        }
    }*/

    public string getActiveMenuItems()
    {
        string active = "";

        for(int i = 0; i < menuPages.Length; i++)
        {
            if(i < menuPages.Length - 1)
            {
                if (menuPages[i].gameObject.activeSelf)
                {
                    active = String.Concat(active, 1 + ":");
                }
                else
                {
                    active = String.Concat(active, 0 + ":");
                }
            } else
            {
                if (menuPages[i].gameObject.activeSelf)
                {
                    active = String.Concat(active, 1);
                }
                else
                {
                    active = String.Concat(active, 0);
                }
            }
        }

        return active;
    }

    public void setActiveMenuItems(string active)
    {
        string[] actives = active.Split(":");

        for(int i = 0; i < actives.Length; i++)
        {
            if(int.Parse(actives[i]) == 1)
            {
                updateMenuGroup(i, true);
            } else
            {
                updateMenuGroup(i, false);
            }
        }
    }

    /*private void removeMenuGroup(string name)
    {
        //Find an activate a specific menu group on top of the other menu groups, instead of turning the others to false.
        for (int i = 0; i < menuPages.Length; i++)
        {
            if (string.Equals(menuPages[i].gameObject.name, name))
            {
                updateMenuGroup(i, false);
            }
        }
    }*/

    public GameObject getMenuGroup(string name)
    {
        for (int i = 0; i < menuPages.Length; i++)
        {
            if (string.Equals(menuPages[i].gameObject.name, name))
            {
                return menuPages[i].gameObject;
            }
            else
            {
                updateMenuGroup(i, false);
            }
        }

        return null;
    }

    //Used at the end of each button press to save controls into the system.
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

    //Unsure where this is used. Use to update audio and set up controls on button press.
    public void loadOptions()
    {
        if (!man_)
        {
            man_ = GameObject.FindGameObjectWithTag("GameManager");
        }

        if (man_.GetComponent<OptionsManager>())
        {
            man_.GetComponent<OptionsManager>().setUpControls();
            loadAudio();
        }
    }

    public void loadAchievements()
    {
        if (!man_)
        {
            man_ = GameObject.FindGameObjectWithTag("GameManager");
        }

        if (man_.GetComponent<AchievementManager>())
        {
            man_.GetComponent<AchievementManager>().updateAchievementMenu();
            //loadAudio();
        }
    }

    public void loadAudio()
    {
        if (!man_)
        {
            man_ = GameObject.FindGameObjectWithTag("GameManager");
        }

        if (man_.GetComponent<OptionsManager>())
        {
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
        ButtonClass[] dynamicButtons = this.GetComponentsInChildren<ButtonClass>();

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
                if (dynamicButtons[i].GetComponent<OptionChangeButton>())
                {
                    //Update text to what is matched in the options manager.
                    dynamicButtons[i].adjustButtonText(opt.getControl(dynamicButtons[i].GetComponent<OptionChangeButton>().getControlType()).ToString());
                }
            }
        }
    }

    //Change and update the value of the audio.
    public void updateOptionValues()
    {
        if (!man_)
        {
            man_ = GameObject.FindGameObjectWithTag("GameManager");
        }

        OptionsManager opt = man_.GetComponent<OptionsManager>();

        //opt.setMasterVolume()
        if (dynamicScrolls.Length <= 0)
        {
            dynamicScrolls = this.GetComponentsInChildren<Scrollbar>();

            //Debug.Log("Worked?: " + dynamicButtons.Length);
        }

        if (dynamicScrolls.Length > 0)
        {
            for (int i = 0; i < dynamicScrolls.Length; i++)
            {
                if (dynamicScrolls[i].GetComponent<ScrollChangeButton>())
                {
                    //Update text to what is matched in the options manager.
                    dynamicScrolls[i].GetComponent<ScrollChangeButton>().adjustScroll(opt.getValues(dynamicScrolls[i].GetComponent<ScrollChangeButton>().getScrollType()));
                }
            }
        }
    }

    public void changeScrollValue(string type, float v)
    {
        if (!man_)
        {
            man_ = GameObject.FindGameObjectWithTag("GameManager");
        }

        OptionsManager opt = man_.GetComponent<OptionsManager>();

        opt.setValue(type, v);
        updateOptionValues();
    }

    //A function which finds the first text of a given menu item and updates that to whatever prompt is given.
    public void updateText(string ind, string s, float time)
    {
        //first, add the given menu group.
        externalMessage[0].SetActive(true);

        //Change prompt of text.
        externalMessage[0].GetComponentInChildren<TextMeshProUGUI>().text = s;
        Color col = externalMessage[0].GetComponentInChildren<Image>().color;

        isDisplayingMessage = true;
        promptTimer = time;
        StartCoroutine(promptCoRoutine(externalMessage[0].GetComponentInChildren<Image>(), ind, time));
    }

    IEnumerator promptCoRoutine(Image img, string name, float t)
    {
        Color col = img.color;
        TextMeshProUGUI tex = img.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        Color texCol = tex.color;

        while (isDisplayingMessage)
        {
            //Go through and set to smaller.
            if(promptTimer > 0)
            {
                promptTimer -= Time.deltaTime / 3;

                //Start changing alpha until it is ready.
                if(promptTimer <= t / 2)
                {
                    col.a = promptTimer / (t / 2);
                    texCol.a = promptTimer / (t / 2);
                    img.color = col;
                    tex.color = texCol;
                } else
                {
                    col.a = 1;
                    texCol.a = 1;
                    img.color = col;
                    tex.color = texCol;
                }
            } else
            {
                isDisplayingMessage = false;
                col.a = 1;
                texCol.a = 1;
                img.color = col;
                tex.color = texCol;
                externalMessage[0].SetActive(false);
                StopCoroutine(promptCoRoutine(img, name, t));
            }

            yield return null;
        }
    }

    public float getMenuID()
    {
        return menuID;
    }

    public void setMenuID(float i)
    {
        menuID = i;
    }

    public int getMenuAmount()
    {
        return menuPages.Length;
    }

    public bool isRunningMessage()
    {
        return isDisplayingMessage;
    }

    public void setisRunningMessage(bool b)
    {
        isDisplayingMessage = b;
    }
}
