using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    public Transform m_player;
    public float m_mouseSensitivity = 2f;
    float camVertRot = 0f;

    bool cursorLocked = true;
    bool headLocked = false;

    private CinemachineVirtualCamera _cam;

    [SerializeField]
    private Vector2 crouchBob;
    [SerializeField]
    private Vector2 walkBob;
    [SerializeField]
    private Vector2 runBob;

    private Vector3 startingForward;

    // Start is called before the first frame update
    void Start()
    {
        if (cursorLocked)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        } else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        _cam = GetComponent<CinemachineVirtualCamera>();

        startingForward = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (!headLocked)
        {
            //Get the axis of the mouse.
            float inputX = Input.GetAxis("Mouse X") * m_mouseSensitivity;
            float inputY = Input.GetAxis("Mouse Y") * m_mouseSensitivity;

            //Change the vertical angle of mouse. Minus to invert.
            camVertRot -= inputY;
            //Clam so you don't 360 all the time.
            camVertRot = Mathf.Clamp(camVertRot, -60, 70);

            //Now, rotate the player with the inputX.
            //If no player is selected, then rotate self based on local use.
            if (m_player)
            {
                m_player.Rotate(Vector3.up * inputX);
                //change transform using eulker angles. (Vector3 doesn't change except x axis, so .right)
                transform.localEulerAngles = Vector3.right * camVertRot;
            } else
            {
                //Change the vertical angle of mouse. Minus to invert.
                float camHozLoc = inputX;
                float camVertLoc = inputY;


                //Clam so you don't 360 all the time.
                //camHozRot = Mathf.Clamp(camVertRot, -90, 70);
                //change transform using eulker angles. (Vector3 doesn't change except x axis, so .right)
                Vector3 angles = new Vector3(-camVertLoc, camHozLoc, 0);
                Vector3 v = transform.eulerAngles + angles;

                float min = startingForward.y - 70 < 0 ? 360 + (startingForward.y - 70) : startingForward.y - 70;
                float max = startingForward.y + 70 > 360 ? (startingForward.y + 70) - 360 : startingForward.y + 70;

                //angles.y = Mathf.Clamp(angles.y, min, max);

                bool inverted = max < min;
                bool isOutOfBounds = false;

                //v.y += 40;

                //Debug.Log(v + " | " + min + "," + max);

                if(v.y < min)
                {
                    //Test to see if lesser than max to see if valid.
                    if(inverted)
                    {
                        if(v.y > max)
                        {
                            isOutOfBounds = true;
                        }
                    } else
                    {
                        isOutOfBounds = true;
                    }
                }

                if(v.y > max)
                {
                    if (inverted)
                    {
                        if(v.y < min)
                        {
                            isOutOfBounds = true;
                        }
                    } else
                    {
                        isOutOfBounds = true;
                    }
                }

                if (isOutOfBounds)
                {
                    v.y = Mathf.Abs(v.y - min) < Mathf.Abs(v.y - max) ? v.y = min : v.y = max;
                }


                /*if (v.y > 360)
                {
                    //Clamp the new angle to be between the 0 and the converted max angle.
                    v.y = Mathf.Clamp(v.y - 360, 0, (startingForward.y + 70) - 360);
                }

                
                if (v.y < 0)
                {
                    v.y = Mathf.Clamp(360 + v.y, 360 + (startingForward.y - 70), 360);
                }*/

                

                //Debug.Log(startingForward + " | " + angles + " | " + min + "," + max);
                transform.eulerAngles = v;
            }
        }
    }

    public void changeHeadBob(int i)
    {
        if(i == 3)
        {
            _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = runBob.x;
            _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = runBob.y;
        }
        else if (i == 1)
        {
            _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = crouchBob.x;
            _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = crouchBob.y;
        }
        else
        {
            _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().AmplitudeGain = walkBob.x;
            _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().FrequencyGain = walkBob.y;
        }
    }

    public void lockHead(bool b)
    {
        headLocked = b;
    }
}
