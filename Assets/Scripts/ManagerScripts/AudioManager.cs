using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Struct to keep all audio files.
public struct audSource{
    public audSource(AudioSource s, float max)
    {
        aud = s;
        maxVol = max;
    }

    public AudioSource aud;
    public float maxVol;
}


public class AudioManager : MonoBehaviour
{
    enum walkingStatus
    {
        metal
    }

    [HeaderAttribute("Walking audio")]
    [SerializeField]
    AudioClip[] metalWalking;

    [HeaderAttribute("Running audio")]
    [SerializeField]
    AudioClip[] metalRunning;

    AudioClip[][] allRunning;
    AudioClip[][] allWalking;

    walkingStatus status = walkingStatus.metal;

    audSource[] audSources;

    private AudioListener[] listeners;

    public void setUpManager()
    {
        allRunning = new AudioClip[1][];
        allRunning[0] = metalRunning;

        allWalking = new AudioClip[1][];
        allWalking[0] = metalWalking;

        setUpAudios();
    }

    //Return a clip length based on the current type and time.
    public int getCurrentClipLength(int audioInt)
    {
        if(audioInt == 0)
        {
            return allWalking[(int)status].Length;
        } else if (audioInt == 1)
        {
            return allRunning[(int)status].Length;
        }

        return 0;
    }

    public void setAudioVolume(int ind, float percent)
    {
        audSources[ind].aud.volume = audSources[ind].maxVol * percent;
    }

    public int getAudioSourceLength()
    {
        return audSources.Length;
    }

    //Return a single audio clip from the running sounds.
    public AudioClip getAudio(int audioInd, int ind)
    {
        if (audioInd == 0)
        {
            return allWalking[(int)status][ind];
        }
        else if (audioInd == 1)
        {
            return allRunning[(int)status][ind];
        }

        return null;
    }

    //Set up audio sources with these audio things.
    public void setUpAudios()
    {
        AudioSource[] audio = GameObject.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        audSources = new audSource[audio.Length];

        //Set up all sources.
        for(int i = 0; i < audSources.Length; i++)
        {
            audSources[i] = new audSource(audio[i], audio[i].volume);
        }

        listeners = GameObject.FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
    }
}
