using Cinemachine;
using System.Collections;
using System.Collections.Generic;
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
            camVertRot = Mathf.Clamp(camVertRot, -90, 70);
            //change transform using eulker angles. (Vector3 doesn't change except x axis, so .right)
            transform.localEulerAngles = Vector3.right * camVertRot;

            //Now, rotate the player with the inputX.
            m_player.Rotate(Vector3.up * inputX);
        }
    }

    public void changeHeadBob(int i)
    {
        if(i == 3)
        {
            _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = runBob.x;
            _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = runBob.y;
        }
        else if (i == 1)
        {
            _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = crouchBob.x;
            _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = crouchBob.y;
        }
        else
        {
            _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = walkBob.x;
            _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = walkBob.y;
        }
    }

    public void lockHead(bool b)
    {
        headLocked = b;
    }
}
