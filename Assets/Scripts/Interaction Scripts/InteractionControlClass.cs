using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionControlClass : MonoBehaviour
{
    Animator anim_;

    InteractionIndicatorScript indicator;

    AudioSource aud_;

    [SerializeField]
    AudioClip[] clips;

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

    //This controller function changes the angle of an time using a hinge.
    //Ensure hinge is attached, then change targetPosition based on given angle.
    public void setAngle(Vector3 dir)
    {
        float rad1 = Mathf.Atan2(dir.x, dir.z);
        rad1 = rad1 * Mathf.Rad2Deg;
        rad1 = rad1 - 90;

        float deg1 = Vector3.Angle(-dir, Vector3.forward);

        JointSpring spring = new JointSpring();
        spring.targetPosition = rad1;
        spring.spring = 100;

        if (GetComponent<HingeJoint>())
        {
            GetComponent<HingeJoint>().spring = spring;
        }
    }

    //If set, then any target angle is removed from hinge joint if hinge joint is part of it.
    public void unsetAngle()
    {
        JointSpring spring = new JointSpring();

        spring.targetPosition = 0;
        spring.spring = 0;

        if (GetComponent<HingeJoint>())
        {
            GetComponent<HingeJoint>().spring = spring;
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

    //A function to play a given audio clip.
    public void playInteractionAudio(int ind)
    {
        if(aud_ && clips.Length > 0)
        {
            if(clips.Length > ind)
            {
                aud_.PlayOneShot(clips[ind]);
            }
            
        }


    }

    //Return the amount of clips in the clips section.
    public int getAudioLength()
    {
        return clips.Length;
    }

    //A function to update this interaction control.
    public void updateThisInteraction()
    {
        Debug.Log("Update to: " + this.gameObject.name);
        
        anim_ = GetComponentInChildren<Animator>();

        indicator = GetComponentInChildren<InteractionIndicatorScript>();

        if (indicator)
        {
            indicator.switchToOff();
        }

        aud_ = GetComponent<AudioSource>();
    }

    //A function to get the current transform of this object.
    public Transform getThisTransform()
    {
        return this.transform;
    }
}
