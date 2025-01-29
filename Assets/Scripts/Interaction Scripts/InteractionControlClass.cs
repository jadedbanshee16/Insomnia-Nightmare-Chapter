using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionControlClass : MonoBehaviour
{
    Animator anim_;

    InteractionIndicatorScript indicator;

    AudioSource aud_;

    EventScript events;

    [SerializeField]
    AudioClip[] clips;

    [SerializeField]
    GameObject[] activeObjects;

    int nulledParameter1 = Animator.StringToHash("Pressed");
    int nulledParameter2 = Animator.StringToHash("Shake");
    List<int> validPams = new List<int>();

    //Set the position of this current object to a given other position.
    public void setPosition(Vector3 pos, Quaternion rot)
    {
        this.transform.position = pos;
        this.transform.rotation = rot;
    }

    //A function to set the position of a specified object.
    public void setPosition(Vector3 pos, Quaternion rot, Transform obj)
    {
        if (obj)
        {
            obj.position = pos;
            obj.rotation = rot;
        } else
        {
            this.transform.position = pos;
            this.transform.rotation = rot;
        }
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
    public void setAngle(Vector3 dir, Transform offset)
    {
        //dir = offset.InverseTransformPoint(dir);

        //dir = dir - offset.position;

        /*float dot = Vector3.Dot(offset.forward, dir - offset.position);

        float angle = Mathf.Acos(dot) / Mathf.PI;

        angle = angle * Mathf.Rad2Deg;

        //dir = offset.InverseTransformPoint(dir);

        float rad1 = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        rad1 = rad1 - 90;*/

        dir.y = offset.position.y;

        float ang = Vector3.Angle(dir - offset.position, offset.right);

        if(Vector3.Dot(offset.forward, dir - offset.position) > 0)
        {
            ang = -ang;
        }



        //Debug.DrawRay(offset.position, dir - offset.position, Color.white, 3f);
        //Debug.DrawRay(offset.position, offset.forward, Color.blue, 3f);

        //Debug.Log(ang);

        //float deg1 = Vector3.Angle(-dir, Vector3.forward);

        JointSpring spring = new JointSpring();
        spring.targetPosition = ang;
        spring.spring = 100;

        if (GetComponent<HingeJoint>())
        {
            GetComponent<HingeJoint>().spring = spring;
        }
    }

    public void setAngle(Vector3 dir, Transform offset, bool inverted)
    {
        //dir = offset.InverseTransformPoint(dir);

        //dir = dir - offset.position;

        /*float dot = Vector3.Dot(offset.forward, dir - offset.position);

        float angle = Mathf.Acos(dot) / Mathf.PI;

        angle = angle * Mathf.Rad2Deg;

        //dir = offset.InverseTransformPoint(dir);

        float rad1 = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        rad1 = rad1 - 90;*/

        dir.y = offset.position.y;

        float ang = Vector3.Angle(dir - offset.position, offset.right);

        if (inverted)
        {
            ang = Vector3.Angle(offset.position - dir, offset.right);

            if (Vector3.Dot(-offset.forward, dir - offset.position) > 0)
            {
                ang = -ang;
            }
        } else
        {
            if (Vector3.Dot(offset.forward, dir - offset.position) > 0)
            {
                ang = -ang;
            }
        }



        //Debug.DrawRay(offset.position, dir - offset.position, Color.white, 3f);
        //Debug.DrawRay(offset.position, offset.forward, Color.blue, 3f);

        //Debug.Log(ang);

        //float deg1 = Vector3.Angle(-dir, Vector3.forward);

        JointSpring spring = new JointSpring();
        spring.targetPosition = ang;
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

        spring.spring = 0;
        //spring.targetPosition = 0;

        if (GetComponent<HingeJoint>())
        {
            GetComponent<HingeJoint>().spring = spring;
            spring.targetPosition = 0;
            GetComponent<HingeJoint>().spring = spring;
        }
    }

    //Make an naimation run as a trigger.
    public void setAnimation(string animationPrompt)
    {
        if (!anim_)
        {
            updateThisInteraction();
        }

        if ((validPams.Contains(nulledParameter1) || validPams.Contains(nulledParameter2)) && anim_)
        {
            anim_.SetTrigger(animationPrompt);
        }
    }

    public void resetAnimation(string animationPrompt)
    {
        if (!anim_)
        {
            updateThisInteraction();
        }

        if ((validPams.Contains(nulledParameter1) || validPams.Contains(nulledParameter2)) && anim_)
        {
            anim_.ResetTrigger(animationPrompt);
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
        if(!aud_){
            updateThisInteraction();
        }

        if(aud_ && clips.Length > 0)
        {
            if(clips.Length > ind)
            {
                aud_.PlayOneShot(clips[ind]);
            }
            
        }
    }

    //A function which would play the inbuilt audio clip using a delay.
    public void playInbuiltAudio(float delay, bool isOn)
    {
        if (!aud_)
        {
            updateThisInteraction();
        }

        if (aud_ && aud_.clip != null)
        {
            if (isOn)
            {
                //Debug.Log("Double winn");
                aud_.PlayDelayed(delay);
            } else
            {
                aud_.Stop();
            }
            
        }
    }

    public void adjustVolume(float newVol)
    {
        if (!aud_)
        {
            updateThisInteraction();
        }

        if (aud_)
        {
            aud_.volume = newVol;
        }
    }

    //Set the objects meant to be active to and from active.
    public void setActive(bool isOn)
    {
        if(activeObjects.Length > 0)
        {
            for(int i = 0; i < activeObjects.Length; i++)
            {
                activeObjects[i].SetActive(isOn);
            }
        }
    }

    //Run an event when this occurs.
    public void triggerEvent()
    {
        if (!events)
        {
            updateThisInteraction();
        }

        if (events)
        {
            events.TriggerEvent();
        }
    }

    public void triggerEvent(bool b)
    {
        if (!events)
        {
            updateThisInteraction();
        }

        if (events)
        {
            //Debug.Log("Works 2: " + b);
            events.TriggerEvent(b);
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
        anim_ = GetComponentInChildren<Animator>();

        //Try this object as well.
        if (!anim_)
        {
            anim_ = GetComponent<Animator>();
        }

        indicator = GetComponentInChildren<InteractionIndicatorScript>();

        events = GetComponent<EventScript>();

        /*if (indicator)
        {
            indicator.switchToOff();
        }*/

        aud_ = GetComponent<AudioSource>();

        //Try to find an audio in the parent.
        if (!aud_)
        {
            aud_ = GetComponentInParent<AudioSource>();
        }

        if (anim_)
        {
            for (int i = 0; i < anim_.parameters.Length; i++)
            {
                validPams.Add(anim_.parameters[i].nameHash);
            }
        }
    }

    //A function to get the current transform of this object.
    public Transform getThisTransform()
    {
        return this.transform;
    }
}
