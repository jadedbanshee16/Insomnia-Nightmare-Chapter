using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionControlClass : MonoBehaviour
{
    Animator anim_;

    InteractionIndicatorScript indicator;

    //Set the position of this current object to a given other position.
    public void setPosition(Vector3 pos, Quaternion rot)
    {
        this.transform.position = pos;
        this.transform.rotation = rot;
    }

    //Make an animation run in the bool position.
    public void setAnimation(string animationPrompt, bool turnOn)
    {
        if (anim_)
        {
            anim_.SetBool(animationPrompt, turnOn);
        }

    }

    //Make an naimation run as a trigger.
    public void setAnimation(string animationPrompt)
    {
        if (anim_)
        {
            anim_.SetTrigger(animationPrompt);
        }
    }

    //Set the indicator if an indicator script is connected.
    public void setIndicator(bool isOn)
    {
        //Ensure an indicator is found before moving on to the next thing.
        if (!indicator)
        {
            indicator = GetComponentInChildren<InteractionIndicatorScript>();
        }

        if (indicator)
        {
            if (isOn)
            {
                indicator.switchToOn();
            }
            else
            {
                indicator.switchToOff();
            }
        }

    }

    //A function to update this interaction control.
    public void updateThisInteraction()
    {
        anim_ = GetComponentInChildren<Animator>();

        indicator = GetComponentInChildren<InteractionIndicatorScript>();

        if (indicator)
        {
            indicator.switchToOff();
        }
    }
}
