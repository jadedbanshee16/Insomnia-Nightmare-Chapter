using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationInteractionClass : InteractionClass
{
    [SerializeField]
    float rotationOffset;

    bool isRotating;
    Quaternion newRotation;

    float timer = 0;

    [SerializeField]
    Transform connectedObject;

    private void Start()
    {
        setController();
    }

    //If object is moved, 
    private void FixedUpdate()
    {
        if (isRotating)
        {
            //Update the timer.
            if(timer < 1)
            {
                timer += Time.deltaTime;
            } else
            {
                timer = 1;
                isRotating = false;
            }

            controller.setPosition(connectedObject.transform.position, Quaternion.Lerp(connectedObject.transform.rotation, newRotation, timer), connectedObject);
        }
    }

    //Another overload for the interaction.
    public override void Interact()
    {
        //Find new quaterion.
        Vector3 rot = connectedObject.transform.rotation.eulerAngles;

        rot.z += rotationOffset;

        newRotation = Quaternion.Euler(rot.x, rot.y, rot.z);

        isRotating = true;

        timer = 0;
    }
}
