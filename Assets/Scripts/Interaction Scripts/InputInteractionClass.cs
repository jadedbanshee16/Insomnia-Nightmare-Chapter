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
        //Set the animation of the controller.
        controller.setAnimation("Pressed");


        if(String.Equals(input, "%DELETE"))
        {
            screen.clearString();
        } else if (String.Equals(input, "%ENTER"))
        {
            //Now set object based on affected object in the screen.
            setObject(screen.getAffectedObject());

        } else
        {
            screen.addString(input);
        }
    }

    //Update power manager with the new system.
    public void setObject(EnergyObjectClass c)
    {
        powerManager.updateObject(c, screen.isCurrentCode());
    }
}
