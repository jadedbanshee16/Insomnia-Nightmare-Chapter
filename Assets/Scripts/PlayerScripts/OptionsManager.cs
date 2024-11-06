using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public enum theControls
    {
        interaction,
        drop,
        itemControl,
        torchControl,
        forward,
        backward,
        left,
        right,
        run,
        crouch,
        exit
    };

    private int controlSize = 11;

    public KeyCode[] controls;

    [SerializeField]
    KeyCode[] defaultControls;

    public float masterVolume;

    //A function to update all controls in the control list.
    //WARNING input an array that is the same size as controls.
    public void updateControls(KeyCode[] newControls)
    {
        controls = new KeyCode[controlSize];

        for(int i = 0; i < controls.Length; i++)
        {
            controls[i] = newControls[i];
        }
    }

    public void updateControl(int ind, KeyCode newKey)
    {
        controls[ind] = newKey;
    }

    //Get the control belonging to that particular control.
    public KeyCode getControl(theControls c)
    {
        return controls[(int)c];
    }

    public float getVolume(string s)
    {
        if(string.Equals(s, "Master Volume"))
        {
            return masterVolume;
        }

        return 0;
    }

    //Go and set all of controls from player preferences.
    public void setUpControls()
    {
        KeyCode[] newControls = new KeyCode[controlSize];

        for(int i = 0; i < newControls.Length; i++)
        {
            //Debug.Log((KeyCode)PlayerPrefs.GetInt(((theControls)i).ToString()));
            //Test if the player pref has the key. If not, then set to default.
            if (!PlayerPrefs.HasKey(((theControls)i).ToString()))
            {
                PlayerPrefs.SetInt(((theControls)i).ToString(), (int)defaultControls[i]);
            }
            newControls[i] = (KeyCode)PlayerPrefs.GetInt(((theControls)i).ToString());
        }

        if(!PlayerPrefs.HasKey("Master Volume"))
        {
            PlayerPrefs.SetFloat("Master Volume", 1f);
        }

        if(PlayerPrefs.GetFloat("Master Volume") >= 0)
        {
            masterVolume = PlayerPrefs.GetFloat("Master Volume");
        } else
        {
            masterVolume = 1;
        }

        //Update controls to the new controls.
        updateControls(newControls);
    }

    public void saveControls(bool def)
    {
        //Get all controls from 'Controls' or 'DefualtControls'.
        if (def)
        {
            for(int i = 0; i < defaultControls.Length; i++)
            {
                //Debug.Log(((theControls)i).ToString() + ": " + defaultControls[i]);

                PlayerPrefs.SetInt(((theControls)i).ToString(), (int)defaultControls[i]);

                //Set up the volume settings (Just 1 for now).
                PlayerPrefs.SetFloat("Master Volume", 0.5f);
            }
        } else
        {
            for (int i = 0; i < controls.Length; i++)
            {
                PlayerPrefs.SetInt(((theControls)i).ToString(), (int)controls[i]);
            }

            //Set up the volume settings (Just 1 for now).
            PlayerPrefs.SetFloat("Master Volume", masterVolume);
        }

        //Set up controls using this new saved system.
        setUpControls();
    }

    //A function to return whether the controls list is empty.
    public bool isControlsEmpty()
    {
        return controls.Length <= 0;
    }

    //A function to return the master volume.
    public float getMasterVol()
    {
        return masterVolume;
    }

    //A function to set the master volume.
    public void setVolume(string s, float f)
    {
        if(string.Equals(s, "Master Volume"))
        {
            masterVolume = f;
        }
    }
}
