using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlInteractionClass : InteractionClass
{
    FPSController player_;

    CameraManager camManager_;

    CinemachineVirtualCamera currentCam;

    private bool isOn;

    private void Start()
    {
        currentCam = GetComponentInChildren<CinemachineVirtualCamera>();

        setController();
    }

    //An overload for the interaction to take a specific object.
    public override void Interact(GameObject obj)
    {
        controller.setAnimation("Pressed");

        controller.playInteractionAudio(0);


        //Set the player.
        if (!player_ && obj.GetComponent<FPSController>())
        {
            player_ = obj.GetComponent<FPSController>();
        }

        //Set the camera.
        if(!camManager_ && obj.GetComponent<CameraManager>())
        {
            camManager_ = obj.GetComponent<CameraManager>();
        }

        isOn = !isOn;

        if (isOn)
        {
            player_.setLock(isOn, this.gameObject);
        } else
        {
            player_.setLock(isOn, null);
        }
        

        //Find out if camera switches when working.
        if (isOn)
        {
            camManager_.setCamera(camManager_.findCamera(currentCam));
        } else
        {
            camManager_.setCamera(0);
        }
    }
}
