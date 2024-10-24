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

    KeyCode[] controls;

    [SerializeField]
    KeyCode[] defaultControls;

    private float masterVolume;

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

    //Get the control belonging to that particular control.
    public KeyCode getControl(theControls c)
    {
        return controls[(int)c];
    }

    //Go and set all of controls from player preferences.
    public void setUpControls()
    {
        KeyCode[] newControls = new KeyCode[controlSize];

        for(int i = 0; i < newControls.Length; i++)
        {
            //Debug.Log((KeyCode)PlayerPrefs.GetInt(((theControls)i).ToString()));

            newControls[i] = (KeyCode)PlayerPrefs.GetInt(((theControls)i).ToString());
        }

        if(PlayerPrefs.GetFloat("Master Volume") > 0)
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
            }
        } else
        {
            for (int i = 0; i < controls.Length; i++)
            {
                PlayerPrefs.SetInt(((theControls)i).ToString(), (int)controls[i]);
            }
        }

        //Set up the volume settings (Just 1 for now).
        PlayerPrefs.SetFloat("Master Volume", masterVolume);

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
}
