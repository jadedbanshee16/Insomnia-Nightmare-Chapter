using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEmmersion : MonoBehaviour
{
    OptionsManager volumeOptions;
    AudioManager audioSettings;
    Transform player_;

    [SerializeField]
    Transform emmersionTarget;

    Vector3 lastPlayerPosition;
    int audioID;
    LayerMask mask;

    bool muffled;
    bool isChanged;
    float currentVol;

    // Start is called before the first frame update
    void Start()
    {
        volumeOptions = GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>();
        audioSettings = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioManager>();
        player_ = GameObject.FindGameObjectWithTag("Player").transform;

        if (!emmersionTarget)
        {
            emmersionTarget = this.transform;
        }

        audioID = audioSettings.returnAudioPosition(GetComponent<AudioSource>());
        mask =~ LayerMask.GetMask("PlayerLayer");

        muffled = false;
        isChanged = false;
        currentVol = 1;

        lastPlayerPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(player_.position, lastPlayerPosition);

        //Debug.Log(dist + "|" + muffled + ": " + GetComponent<AudioSource>().volume);

        if (dist > 1)
        {
            //Debug.DrawRay(emmersionTarget.position, player_.position - emmersionTarget.position, Color.cyan, Mathf.Infinity);
            //Send a ray to find out if anything is between the generator and the player.
            if(Physics.Raycast(emmersionTarget.position, Vector3.Normalize(player_.position - emmersionTarget.position), Vector3.Distance(player_.position, emmersionTarget.position), mask, QueryTriggerInteraction.Ignore))
            {
                muffled = true;
                isChanged = true;
            } else
            {
                if (muffled)
                {
                    muffled = false;
                    isChanged = true;
                }
            }
        }

        //Code to change the volume over time.
        //If changed, then decide if muffled.
        //From there, use delta time to add or remove volume and make the change accordingly.
        if (isChanged)
        {
            if (muffled)
            {
                if(currentVol < 1)
                {
                    currentVol += Time.deltaTime * 5;
                    
                    
                } else
                {
                    isChanged = false;
                }
            } else
            {
                if (currentVol > 0)
                {
                    currentVol -= Time.deltaTime * 5;
                }
                else
                {
                    isChanged = false;
                }
            }

            //This should set sound to half what it should be if within.
            audioSettings.setAudioVolume(audioID, volumeOptions.getMasterVol() * (1 - (0.5f * currentVol)));
        }
    }
}
