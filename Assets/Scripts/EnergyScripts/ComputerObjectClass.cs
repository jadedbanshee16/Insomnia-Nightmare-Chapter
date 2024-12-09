using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerObjectClass : EnergyObjectClass
{
    private enum computerType
    {
        text,
        password
    }

    [SerializeField]
    ScreenClass screenObject;
    [SerializeField]
    InputInteractionClass inputObject;

    [SerializeField]
    private computerType computerInputType;

    [SerializeField]
    string[] messages;
    [SerializeField]
    string[] codes;

    private string currentString = "";

    [SerializeField]
    int inputSize;

    [SerializeField]
    private GameObject[] affectedObj;

    [SerializeField]
    private bool persistantScreenUnlock;



    private void Start()
    {
        //currentCode = "";

        if (screenObject)
        {
            if (String.Equals(currentString, ""))
            {
                screenObject.displayText(messages[messages.Length - 1], isPowered);
            }
            else
            {
                screenObject.displayText(currentString, isPowered);
            }
        }
    }

    //A function that will power the current object.
    public override void powerObject(bool b)
    {
        isPowered = b;

        //Maybe will break. Will find out.
        if (String.Equals(currentString, ""))
        {
            screenObject.displayText(messages[messages.Length - 1], isPowered);
        }


    }

    //Combine the added string to the current string.
    public void addString(string s)
    {
        if (isPowered)
        {
            currentString = currentString + s;

            screenObject.displayText(currentString, isPowered);
        }
    }

    //Remove the last element of the string.
    public void removeString()
    {
        if (isPowered)
        {
            if (currentString.Length > 0)
            {
                currentString = currentString.Substring(0, currentString.Length - 1);
            }

            if(currentString.Length > 0)
            {
                screenObject.displayText(currentString, isPowered);
            } else
            {
                screenObject.displayText(messages[messages.Length - 1], isPowered);
            }
        }

    }

    //Make string empty.
    public void clearString()
    {
        if (isPowered)
        {
            currentString = "";

            screenObject.displayText(messages[messages.Length - 1], isPowered);
        }
    }

    //A function that returns of current code is equal to command code.
    public int isCurrentCode()
    {
        for (int i = 0; i < codes.Length; i++)
        {
            if (String.Equals(currentString, codes[i]))
            {
                return i;
            }
        }
        return -1;
    }

    public int isCurrentCode(string s)
    {
        for (int i = 0; i < codes.Length; i++)
        {
            if (String.Equals(s, codes[i]))
            {
                return i;
            }
        }
        return -1;
    }

    //Runs the current code and completes the given message if the code matches.
    public void validateCode()
    {
        int comparedCode = isCurrentCode();

        //Turn off all affected objects if not persistant, so you can reset.
        if (!persistantScreenUnlock)
        {
            for(int i = 0; i < affectedObj.Length; i++)
            {
                if (affectedObj[i] != null && affectedObj[i].GetComponent<EnergyObjectClass>())
                {
                    inputObject.setObject(affectedObj[i].GetComponent<EnergyObjectClass>(), false);
                }
            }
        }

        if(comparedCode >= 0)
        {
            //if the given message has access granted, then see if an energy object or menu is needed.
            if(messages[comparedCode].Contains("access granted:"))
            {
                if(comparedCode < affectedObj.Length && affectedObj[comparedCode] != null && affectedObj[comparedCode].GetComponent<MenuManager>())
                {
                    //If switching to another screen, check type.
                    if (computerInputType == computerType.password)
                    {
                        currentString = "";
                        screenObject.displayText(messages[messages.Length - 1], isPowered);
                    }

                    //Debug.Log(comparedCode + ": " + messages[comparedCode].Substring(16, messages[comparedCode].Length - 16) + " | " + messages[comparedCode].Substring(16, messages[comparedCode].Length - 16).Length);
                    //Attempt to move to menu group on the next given aspect of the string.
                    affectedObj[comparedCode].GetComponent<MenuManager>().setToMenuGroup(messages[comparedCode].Substring(16, messages[comparedCode].Length - 16));
                    return;
                } else if (affectedObj[comparedCode] != null && affectedObj[comparedCode].GetComponent<EnergyObjectClass>())
                {
                    inputObject.setObject(affectedObj[comparedCode].GetComponent<EnergyObjectClass>(), true);
                    screenObject.displayText(messages[comparedCode], isPowered);
                }
            } else if (messages[comparedCode].Contains("additn granted:"))
            {
                if (comparedCode < affectedObj.Length && affectedObj[comparedCode] != null && affectedObj[comparedCode].GetComponent<MenuManager>())
                {
                    //Debug.Log(comparedCode + ": " + messages[comparedCode].Substring(16, messages[comparedCode].Length - 16) + " | " + messages[comparedCode].Substring(16, messages[comparedCode].Length - 16).Length);
                    //Attempt to move to menu group on the next given aspect of the string.
                    affectedObj[comparedCode].GetComponent<MenuManager>().addMenuGroup(messages[comparedCode].Substring(16, messages[comparedCode].Length - 16));
                    return;
                }
                else if (affectedObj[comparedCode] != null && affectedObj[comparedCode].GetComponent<EnergyObjectClass>())
                {
                    inputObject.setObject(affectedObj[comparedCode].GetComponent<EnergyObjectClass>(), true);
                    screenObject.displayText(messages[comparedCode], isPowered);
                }
            } else if (messages[comparedCode].Contains("unlock granted:"))
            {
                if (comparedCode < affectedObj.Length && affectedObj[comparedCode] != null && affectedObj[comparedCode].GetComponent<MenuManager>())
                {
                    //If switching to another screen, check type.
                    if (computerInputType == computerType.password)
                    {
                        currentString = "";
                        screenObject.displayText(messages[messages.Length - 1], isPowered);
                    }

                    //Debug.Log(comparedCode + ": " + messages[comparedCode].Substring(16, messages[comparedCode].Length - 16) + " | " + messages[comparedCode].Substring(16, messages[comparedCode].Length - 16).Length);
                    //Attempt to move to menu group on the next given aspect of the string.
                    affectedObj[comparedCode].GetComponent<MenuManager>().setToMenuGroup(messages[comparedCode].Substring(16, messages[comparedCode].Length - 16));
                    return;
                }
                else if (affectedObj[comparedCode] != null && affectedObj[comparedCode].GetComponent<EnergyObjectClass>())
                {
                    inputObject.setObject(affectedObj[comparedCode].GetComponent<EnergyObjectClass>(), false);
                    screenObject.displayText(messages[comparedCode], isPowered);
                }
            }

            //Display the text.
            if (screenObject)
            {
                screenObject.displayText(messages[comparedCode], isPowered);
            }
        } else
        {
            //Display the final message, which is default fail text.
            if (screenObject)
            {
                screenObject.displayText(messages[messages.Length - 2], isPowered);
            }
        }
    }

    //Runs the current code and completes the given message if the code matches.
    public void validateCode(string code)
    {
        int comparedCode = isCurrentCode(code);

        //Turn off all affected objects if not persistant, so you can reset.
        if (!persistantScreenUnlock)
        {
            for (int i = 0; i < affectedObj.Length; i++)
            {
                if (affectedObj[i] != null && affectedObj[i].GetComponent<EnergyObjectClass>())
                {
                    inputObject.setObject(affectedObj[i].GetComponent<EnergyObjectClass>(), false);
                }
            }
        }

        if (comparedCode >= 0)
        {
            //if the given message has access granted, then see if an energy object or menu is needed.
            if (messages[comparedCode].Contains("access granted:"))
            {
                if (comparedCode < affectedObj.Length && affectedObj[comparedCode] != null && affectedObj[comparedCode].GetComponent<MenuManager>())
                {
                    //If switching to another screen, check type.
                    if(computerInputType == computerType.password)
                    {
                        currentString = "";
                        screenObject.displayText("", isPowered);
                    }
                    //Debug.Log(comparedCode + ": " + messages[comparedCode].Substring(16, messages[comparedCode].Length - 16) + " | " + messages[comparedCode].Substring(16, messages[comparedCode].Length - 16).Length);
                    //Attempt to move to menu group on the next given aspect of the string.
                    affectedObj[comparedCode].GetComponent<MenuManager>().setToMenuGroup(messages[comparedCode].Substring(16, messages[comparedCode].Length - 16));
                    return;
                }
                else if (affectedObj[comparedCode] != null && affectedObj[comparedCode].GetComponent<EnergyObjectClass>())
                {
                    inputObject.setObject(affectedObj[comparedCode].GetComponent<EnergyObjectClass>(), true);
                }
            }
            else if (messages[comparedCode].Contains("additn granted:"))
            {
                if (comparedCode < affectedObj.Length && affectedObj[comparedCode] != null && affectedObj[comparedCode].GetComponent<MenuManager>())
                {
                    //Debug.Log(comparedCode + ": " + messages[comparedCode].Substring(16, messages[comparedCode].Length - 16) + " | " + messages[comparedCode].Substring(16, messages[comparedCode].Length - 16).Length);
                    //Attempt to move to menu group on the next given aspect of the string.
                    affectedObj[comparedCode].GetComponent<MenuManager>().addMenuGroup(messages[comparedCode].Substring(16, messages[comparedCode].Length - 16));
                    return;
                }
                else if (affectedObj[comparedCode] != null && affectedObj[comparedCode].GetComponent<EnergyObjectClass>())
                {
                    inputObject.setObject(affectedObj[comparedCode].GetComponent<EnergyObjectClass>(), true);
                }
            }

            //Display the text.
            if (screenObject)
            {
                screenObject.displayText(messages[comparedCode], isPowered);
            }
        }
        else
        {
            //Display the final message, which is default fail text.
            if (screenObject)
            {
                screenObject.displayText(messages[messages.Length - 2], isPowered);
            }
        }
    }

    //When an input comes in,
    //Make sure it is an added string or one of the possible commands including
    //DELETE, ENTER, BACKSPACE
    public void validateInput(string s, bool playSound)
    {
        if (!controller)
        {
            controller = GetComponent<InteractionControlClass>();
        }
        //If delete command, clear the string.
        if (String.Equals(s, "%DELETE"))
        {
            if (playSound)
            {
                controller.playInteractionAudio(1);
            }
            clearString();
        }
        //If the enter command, validate code.
        else if (String.Equals(s, "%ENTER"))
        {
            //Now set object based on affected object in the screen.
            if (playSound)
            {
                controller.playInteractionAudio(1);
            }

            validateCode();

        }
        //If the backspace command, then remove 1 element from the string.
        else if (String.Equals(s, "%BACKSPACE"))
        {
            if (playSound)
            {
                controller.playInteractionAudio(1);
            }
            removeString();
        }
        else if (s.Contains("%BUTTON:"))
        {
            //This is a button, which means the current string is switched straight over to this item.
            if (playSound)
            {
                controller.playInteractionAudio(1);
            }
            validateCode(s);
        }
        //If no commands, then add the string element.
        else
        {
            if (playSound)
            {
                controller.playInteractionAudio((int)UnityEngine.Random.Range(2, controller.getAudioLength() - 1));
            }
            addString(s);
        }
    }

    //An override to set the current code.
    public void setCurrentCode(string s)
    {
        currentString = s;

        if (screenObject)
        {
            screenObject.displayText(currentString, isPowered);
        }
        //If inactive, activate screen object for a second to change it, then deactivate.
        /*if(!screenObject.gameObject.activeSelf)
        {
            screenObject.gameObject.SetActive(true);
            screenObject.displayText(currentString, isPowered);
            screenObject.gameObject.SetActive(false);
        } else
        {
            
        }*/
    }

    //A function to set the energy manager of this object.
    public override void setEnergyManager(GridManager man)
    {
        energyManager = man;

        //When manager is set, set on depending on energyObject type. Default is off.
        isOn = true;

        //Set any components that needed to be made.
        controller = GetComponent<InteractionControlClass>();
    }

    //Set the current on state of the object.
    public override void setIsOn(bool b)
    {
        //This item cannot be turned off.
        isOn = true;
    }

    public string getCurrentCode()
    {
        return currentString;
    }
}
