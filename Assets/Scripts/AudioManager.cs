using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void setUpManager()
    {
        allRunning = new AudioClip[1][];
        allRunning[0] = metalRunning;

        allWalking = new AudioClip[1][];
        allWalking[0] = metalWalking;
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
}
