using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionChangeButton : ButtonClass
{
    [SerializeField]
    OptionsManager.theControls controlType;
    OptionsManager opt;

    protected FPSController playerController;
    //This is to store the old control status of the player, so you can revert once the new button is found.
    protected FPSController.controlStatus oldStat;

    [SerializeField]
    protected GameObject eventSystem_;

    public override void useButton()
    {
        if (!playerController)
        {
            //Find the player controller to change player interaction state.
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<FPSController>();
        }

        if (eventSystem_)
        {
            eventSystem_.SetActive(false);
        }

        oldStat = playerController.getCurrentControl();
        playerController.setCurrentControl(FPSController.controlStatus.noControl);


        StartCoroutine(findNewButton());
    }

    public OptionsManager.theControls getControlType()
    {
        return controlType;
    }

    IEnumerator findNewButton()
    {
        bool isSearchingForIntButton = true;

        adjustButtonText("[Press]");

        if (!opt)
        {
            opt = GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>();
        }

        while (isSearchingForIntButton)
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(kcode))
                {
                    opt.updateControl((int)controlType, kcode);
                    isSearchingForIntButton = false;
                    adjustButtonText(kcode.ToString());
                }
            }

            yield return null;
        }

        eventSystem_.SetActive(true);
        playerController.setCurrentControl(oldStat);

        base.useButton();
        //End this current coroutine. You don't need it to run anymore.
        StopCoroutine(findNewButton());
    }
}
