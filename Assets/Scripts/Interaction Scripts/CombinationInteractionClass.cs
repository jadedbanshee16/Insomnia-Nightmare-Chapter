using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationInteractionClass : InteractionClass
{
    [SerializeField]
    int combinationNumber;
    
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

        //Ensure only the positive system makes this change.
        if(rotationOffset >= 0)
        {
            int comb = combinationNumber + 5;

            if(comb >= connectedObject.childCount)
            {
                comb = 0 + (comb - connectedObject.childCount);
            }

            connectedObject.GetChild(comb).gameObject.SetActive(false);

            //Rotated to a random number.
            float rand = Random.Range(0, connectedObject.childCount);

            //Debug.Log("First: " + rand);

            rand += 5;

            rand = (int)rand;

            if(rand >= connectedObject.childCount)
            {
                rand = 0 + (rand - connectedObject.childCount);
            }

            //Debug.Log("Second: " + rand);
            //Rotate to point.
            float newOffset = 0;
            for (int i = 0; i < rand; i++)
            {
                newOffset += rotationOffset;
            }

            Vector3 rot = connectedObject.transform.rotation.eulerAngles;

            rot.z += newOffset;

            newRotation = Quaternion.Euler(rot.x, rot.y, rot.z);

            isRotating = true;

            timer = 0;
        }
    }

    //If object is moved, 
    private void FixedUpdate()
    {
        if (isRotating)
        {
            //Update the timer.
            if(timer < 1)
            {
                timer += Time.deltaTime * 3;
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
        Vector3 rot = connectedObject.transform.rotation.eulerAngles;

        rot.z += rotationOffset;

        newRotation = Quaternion.Euler(rot.x, rot.y, rot.z);

        isRotating = true;

        timer = 0;

        controller.playInteractionAudio(0);
    }
}
