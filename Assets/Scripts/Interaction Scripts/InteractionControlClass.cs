using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionControlClass : MonoBehaviour
{
    Animator anim_;

    InteractionIndicatorScript indicator;

    private void Start()
    {
        anim_ = GetComponentInChildren<Animator>();

        indicator = GetComponentInChildren<InteractionIndicatorScript>();

        if (indicator)
        {
            indicator.switchToOff();
        }
    }

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
}
