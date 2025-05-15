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
    public enum walkingStatus
    {
        metal,
        dirt,
        wood,
        grass,
        stairsWood,
        stone
    }

    [HeaderAttribute("Walking audio")]
    [SerializeField]
    AudioClip[] metalWalking;
    [SerializeField]
    AudioClip[] dirtWalking;
    [SerializeField]
    AudioClip[] woodWalking;
    [SerializeField]
    AudioClip[] grassWalking;
    [SerializeField]
    AudioClip[] stairWalking;
    [SerializeField]
    AudioClip[] stoneWalking;

    [HeaderAttribute("Running audio")]
    [SerializeField]
    AudioClip[] metalRunning;
    [SerializeField]
    AudioClip[] dirtRunning;
    [SerializeField]
    AudioClip[] woodRunning;
    [SerializeField]
    AudioClip[] grassRunning;
    [SerializeField]
    AudioClip[] stairRunning;
    [SerializeField]
    AudioClip[] stoneRunning;

    AudioClip[][] allRunning;
    AudioClip[][] allWalking;

    public walkingStatus[] status = { walkingStatus.metal, walkingStatus.metal };

    audSource[] audSources;

    private AudioListener[] listeners;

    public void setUpManager()
    {
        allRunning = new AudioClip[6][];
        allRunning[0] = metalRunning;
        allRunning[1] = dirtRunning;
        allRunning[2] = woodRunning;
        allRunning[3] = grassRunning;
        allRunning[4] = stairRunning;
        allRunning[5] = stoneRunning;

        allWalking = new AudioClip[6][];
        allWalking[0] = metalWalking;
        allWalking[1] = dirtWalking;
        allWalking[2] = woodWalking;
        allWalking[3] = grassWalking;
        allWalking[4] = stairWalking;
        allWalking[5] = stoneWalking;

        setUpAudios();
    }

    //Return a clip length based on the current type and time.
    public int getCurrentClipLength(int obj, int audioInt)
    {
        if(audioInt == 0)
        {
            return allWalking[(int)status[obj]].Length;
        } else if (audioInt == 1)
        {
            return allRunning[(int)status[obj]].Length;
        }

        return 0;
    }

    public void setAudioVolume(int ind, float percent)
    {
        audSources[ind].aud.volume = audSources[ind].maxVol * (percent);
    }

    public int returnAudioPosition(AudioSource a)
    {
        for(int i = 0; i < audSources.Length; i++)
        {
            if (audSources[i].aud == a)
            {
                return i;
            }
        }

        return -1;
    }

    public int getAudioSourceLength()
    {
        return audSources.Length;
    }

    //Return a single audio clip from the running sounds.
    public AudioClip getAudio(int obj, int audioInd, int ind)
    {
        if (audioInd == 0)
        {
            return allWalking[(int)status[obj]][ind];
        }
        else if (audioInd == 1)
        {
            return allRunning[(int)status[obj]][ind];
        }

        return null;
    }

    //Set up audio sources with these audio things.
    public void setUpAudios()
    {
        AudioSource[] audio = GameObject.FindObjectsByType<AudioSource>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        audSources = new audSource[audio.Length];

        //Set up all sources.
        for(int i = 0; i < audSources.Length; i++)
        {
            audSources[i] = new audSource(audio[i], audio[i].volume);
        }

        listeners = GameObject.FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
    }

    public void setStatus(int obj, walkingStatus s)
    {
        status[obj] = s;
    }
}
