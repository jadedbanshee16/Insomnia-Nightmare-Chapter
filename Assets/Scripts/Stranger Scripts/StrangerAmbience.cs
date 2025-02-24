using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrangerAmbience : MonoBehaviour
{
    [SerializeField]
    RawImage right;
    [SerializeField]
    RawImage left;

    [SerializeField]
    Transform stranger;
    [SerializeField]
    Transform player;

    [SerializeField]
    float furthestDist;
    [SerializeField]
    float shortestDist;
    [SerializeField]
    float normalizedAngle;

    float OutInTimer = 1;
    bool isChanging = true;
    bool isActive;

    float sR;
    float sL;

    private void Start()
    {
        OutInTimer = 1;
        isChanging = false;
        isActive = stranger.gameObject.activeSelf;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive != stranger.gameObject.activeSelf)
        {
            isChanging = true;
        }

        if (stranger.gameObject.activeSelf)
        {
            float strangerDist = Vector3.Distance(stranger.position, player.position);
            Vector3 forwd = player.forward.normalized;
            Vector3 StrangerDir = (stranger.position - player.position).normalized;
            Vector3 rightp = player.right.normalized;
            forwd.y = 0;
            StrangerDir.y = 0;
            rightp.y = 0;
            float strangerDistForward = Vector3.Dot(StrangerDir, forwd);

            float direction = Vector3.Dot(StrangerDir, rightp);

            /*Vector3 pos = player.position;
            pos.y = 0;
            Debug.DrawRay(pos, forwd, Color.green, 0);
            Debug.DrawRay(pos, rightp, Color.blue, 0);
            Debug.DrawRay(pos, StrangerDir, Color.yellow, 0);
            Debug.DrawRay(pos, Vector3.Cross(StrangerDir, Vector3.up), Color.black, 0);
            Debug.Log(direction +"|"+ Mathf.InverseLerp(-normalizedAngle, -1, direction));*/

            float rightStrength = 0;
            float leftStrength = 0;

            if (strangerDist < furthestDist)
            {
                if (direction >= normalizedAngle)
                {
                    rightStrength = Mathf.InverseLerp(normalizedAngle, 1, direction);
                }

                if (direction <= -normalizedAngle)
                {
                    leftStrength = Mathf.InverseLerp(-normalizedAngle, -1, direction);
                }

                if (strangerDistForward < 0)
                {
                    rightStrength = Mathf.Lerp(rightStrength, 1, -strangerDistForward);
                    leftStrength = Mathf.Lerp(leftStrength, 1, -strangerDistForward);
                } else
                {
                    rightStrength = Mathf.Lerp(rightStrength, 1, strangerDistForward);
                    leftStrength = Mathf.Lerp(leftStrength, 1, strangerDistForward);
                }

                rightStrength = rightStrength * 3;
                leftStrength = leftStrength * 3;
            }

            if (strangerDist > shortestDist && strangerDist < furthestDist)
            {
                //take into account the distance.
                rightStrength = rightStrength * Mathf.Clamp(Mathf.InverseLerp(furthestDist, shortestDist, strangerDist), 0, 1);
                leftStrength = leftStrength * Mathf.Clamp(Mathf.InverseLerp(furthestDist, shortestDist, strangerDist), 0, 1);
            }

            if (isChanging)
            {
                if(OutInTimer > 0)
                {
                    OutInTimer -= Time.deltaTime;
                    rightStrength = rightStrength * (1 - OutInTimer);
                    leftStrength = leftStrength * (1 - OutInTimer);
                } else
                {
                    isChanging = false;
                    OutInTimer = 1;
                }
            }

            //Change the items.
            right.material.SetFloat("_Strength", rightStrength);
            left.material.SetFloat("_Strength", leftStrength);
            sL = leftStrength;
            sR = rightStrength;
        } else
        {
            if (isChanging)
            {
                float sRight = sR;
                float sLeft = sL;

                if (OutInTimer > 0)
                {
                    OutInTimer -= Time.deltaTime;

                    sRight = sRight * OutInTimer;
                    sLeft = sLeft * OutInTimer;
                } else
                {
                    OutInTimer = 1;
                    isChanging = false;
                    sR = 0;
                    sL = 0;
                }

                right.material.SetFloat("_Strength", sRight);
                left.material.SetFloat("_Strength", sLeft);
            }
        }

    }
}
