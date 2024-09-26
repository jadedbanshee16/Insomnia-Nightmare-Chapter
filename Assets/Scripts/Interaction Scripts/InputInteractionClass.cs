using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputInteractionClass : InteractionClass
{
    [SerializeField]
    private string input;

    [SerializeField]
    ScreenObjectClass screen;

    private GridManager powerManager;

    void Start()
    {
        setController();

        powerManager = GetComponentInParent<GridManager>();
    }

    //The main interaction. A player may call this for an interaction. So far, it will just run an animation to begin with.
    public override void Interact()
    {
        controller.setAnimation("Pressed");



        if (String.Equals(input, "%DELETE"))
        {
            screen.clearString();
            controller.playInteractionAudio(1);
        } else if (String.Equals(input, "%ENTER"))
        {
            //Now set object based on affected object in the screen.
            if (screen.isObjectPowered())
            {
                setObject(screen.getAffectedObject());
            }

            controller.playInteractionAudio(1);

        } else if (String.Equals(input, "%BACKSPACE"))
        {
            screen.removeString();
            controller.playInteractionAudio(1);
        } else
        {
            screen.addString(input);

            controller.playInteractionAudio((int)UnityEngine.Random.Range(2,  controller.getAudioLength() - 1));
        }
    }

    //Update power manager with the new system.
    public void setObject(EnergyObjectClass c)
    {
        powerManager.updateObject(c, screen.isCurrentCode());
    }

    public void setInput(String s)
    {
        input = s;

        //First, when a keycode is pressed and is not a common character, then return the item for that character.
        if (Input.GetKey(KeyCode.Backspace))
        {
            input = "%BACKSPACE";
        }
        else if (Input.GetKey(KeyCode.Return))
        {
            input = "%ENTER";
        }
    }
}
