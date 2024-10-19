using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationManagerClass : InteractionClass
{
    [SerializeField]
    int combinationNumber;

    [SerializeField]
    int currentNumber;

    [SerializeField]
    float rotationOffset;

    bool isRotating;
    Quaternion newRotation;

    float timer = 0;

    [SerializeField]
    Transform connectedObject;

    [SerializeField]
    LockObjectClass lockObject;

    // Start is called before the first frame update
    void Start()
    {
        setController();

        //Ensure only the positive system makes this change.
        if (rotationOffset >= 0)
        {
            //Rotated to a random number.
            int rand = currentNumber;

            if (rand > 9)
            {
                rand = 0 + (rand - 9);
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

        if (currentNumber != combinationNumber)
        {
            if (lockObject)
            {
                lockObject.GetComponent<EnergyObjectClass>().setIsOn(true);
                lockObject.GetComponent<EnergyObjectClass>().useObject();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            //Update the timer.
            if (timer < 1)
            {
                timer += Time.deltaTime * 3;
            }
            else
            {
                timer = 1;
                isRotating = false;
            }

            controller.setPosition(connectedObject.transform.position, Quaternion.Lerp(connectedObject.transform.rotation, newRotation, timer), connectedObject);
        }
    }

    public void changeCombination(int input)
    {
        Vector3 rot = connectedObject.transform.rotation.eulerAngles;

        rot.z += input;

        bool wasWrong = false;

        if(input >= 0)
        {
            currentNumber++;

            if(currentNumber > 9)
            {
                currentNumber = 0;
            }

            if(currentNumber - 1 != combinationNumber)
            {
                wasWrong = true;
                if (currentNumber == 0 && combinationNumber == 9)
                {
                    wasWrong = false;
                }
            }
        } else
        {
            currentNumber--;

            if(currentNumber < 0)
            {
                currentNumber = 9;
            }

            if (currentNumber + 1 != combinationNumber)
            {
                wasWrong = true;
                if (currentNumber == 9 && combinationNumber == 0)
                {
                    wasWrong = false;
                }
            }
        }

        if (currentNumber != combinationNumber)
        {
            if (lockObject)
            {
                if (!wasWrong)
                {
                    lockObject.GetComponent<EnergyObjectClass>().setIsOn(true);
                    lockObject.GetComponent<EnergyObjectClass>().useObject();
                }
            }
        } else
        {
            if (lockObject)
            {
                lockObject.GetComponent<EnergyObjectClass>().setIsOn(false);
                lockObject.GetComponent<EnergyObjectClass>().useObject();
            }
        }

        newRotation = Quaternion.Euler(rot.x, rot.y, rot.z);

        isRotating = true;

        timer = 0;

        controller.playInteractionAudio(0);
    }
}
