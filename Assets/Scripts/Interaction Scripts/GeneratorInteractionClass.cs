using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorInteractionClass : InteractionClass
{
    [SerializeField]
    SystemManager powerManager;

    [SerializeField]
    bool isOn = false;

    [SerializeField]
    float maxPower;

    private bool wasOn;

    // Start is called before the first frame update
    void Start()
    {
        //isOn = false;

        setController();
    }

    //This interaction will take the object connect to this and attempt to use it as well as the base controller stuff.
    public override void Interact()
    {
        isOn = !isOn;

        //This is to function either with a switch, button or indication.
        //Set the animation of the controller.
        controller.setAnimation("Pressed");

        controller.setAnimation("isOn", isOn);

        controller.setIndicator(isOn);

        controller.playInteractionAudio(0);

        setObject();
    }

    //Update power manager with the new system.
    public void setObject()
    {
        if (powerManager)
        {
            if (isOn)
            {
                powerManager.setGenerator(true);
                powerManager.setGeneratorPower(maxPower);

            } else
            {
                powerManager.setGeneratorPower(0);
            }
        }

        //Make sounds work despite whether powerManager is in or not.
        if (isOn)
        {
            if (!wasOn)
            {
                controller.playInteractionAudio(1);
                controller.playInteractionAudio(2);
                controller.playInbuiltAudio(0.8f, true);

                wasOn = true;
            }

        } else
        {
            if (wasOn)
            {
                controller.playInbuiltAudio(0, false);
                controller.playInteractionAudio(3);

                wasOn = false;
            }
        }
    }

    //Set the current System manager.
    public void setManager(SystemManager man)
    {
        //If not null, set the manager.
        //If null, then test to see if already have powerManager and set the current power set.
        if (man != null)
        {
            powerManager = man;

        } else
        {
            if (powerManager)
            {
                powerManager.setGenerator(false);
                powerManager.setGeneratorPower(0);
                powerManager = man;
            }
        }

        setObject();
    }
}
