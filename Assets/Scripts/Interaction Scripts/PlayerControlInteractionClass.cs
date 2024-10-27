using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlInteractionClass : InteractionClass
{
    FPSController player_;

    CameraManager camManager_;

    [SerializeField]
    CinemachineVirtualCamera currentCam;

    private bool isOn;
    private bool isAdjusting;

    [SerializeField]
    private bool useMouse;

    [SerializeField]
    Transform adjustedObject;

    Quaternion initialPosition;
    float timer = 0;

    private void Start()
    {
        //currentCam = GetComponentInChildren<CinemachineVirtualCamera>();

        //currentCam.gameObject.SetActive(false);

        setController();

        //Set the initial position.
        if (adjustedObject)
        {
            initialPosition = adjustedObject.rotation;
        }
    }

    private void Update()
    {
        if (isAdjusting)
        {
            if(timer < 1)
            {
                timer += Time.deltaTime * 3;
            } else
            {
                timer = 1;
                isAdjusting = false;
                //makeUninteractable(false);
            }

            //Adjust connected object, given it has to face the camera.
            if (isOn)
            {
                Vector3 newPos = currentCam.transform.position - adjustedObject.position;
                newPos.x += 90;
                controller.setPosition(adjustedObject.position, Quaternion.Slerp(initialPosition, Quaternion.Euler(newPos.x, newPos.y, newPos.z), timer), adjustedObject);
            } else
            {
                controller.setPosition(adjustedObject.position, Quaternion.Lerp(adjustedObject.rotation, initialPosition, timer), adjustedObject);
            }
        }
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

        if (adjustedObject)
        {
            isAdjusting = true;
            timer = 0;
            //makeUninteractable(true);
        }

        if (isOn)
        {
            player_.setLock(isOn, this.gameObject, useMouse);
        } else
        {
            player_.setLock(isOn, null, false);
        }

        //If current cam hasn't been set, then set it here.
        if (!currentCam)
        {
            currentCam = GetComponentInChildren<CinemachineVirtualCamera>();

            currentCam.gameObject.SetActive(false);
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

    //A function which can make all objects in adjustedObject set permissions of player.
    private void makeUninteractable(bool noInteraction)
    {
        if (noInteraction)
        {
            for (int i = 0; i < adjustedObject.childCount; i++)
            {
                if (adjustedObject.GetChild(i).GetComponent<InteractionClass>())
                {
                    adjustedObject.GetChild(i).GetComponent<InteractionClass>().removePermission(interactionType.player);
                }
            }
        }
        else
        {
            for (int i = 0; i < adjustedObject.childCount; i++)
            {
                if (adjustedObject.GetChild(i).GetComponent<InteractionClass>())
                {
                    adjustedObject.GetChild(i).GetComponent<InteractionClass>().addPermission(interactionType.player);
                }
            }
        }
    }
}
