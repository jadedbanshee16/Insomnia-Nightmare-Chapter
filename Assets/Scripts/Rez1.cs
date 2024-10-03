using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rez1 : MonoBehaviour
{
    [SerializeField]
    GameObject[] cams;

    Transform[] currentCamPositions;

    int setCam;

    [SerializeField]
    int offset;

    [SerializeField]
    Transform[] floor1Cams;

    private void Start()
    {
        currentCamPositions = new Transform[cams.Length];

        for(int i = 0; i < currentCamPositions.Length; i++)
        {
            currentCamPositions[i] = floor1Cams[i];
        }

        setCam = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Change set cam.
        if (setCam >= 0 && setCam < floor1Cams.Length - 1 && Input.GetKeyDown(KeyCode.RightArrow))
        {
            setCam++;
            changeCams();

        } else if (setCam > 0 && setCam <= floor1Cams.Length - 1 && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            setCam--;
            changeCams();
        }

        updateCams();
    }

    void changeCams()
    {
        Transform keptTransform = null;
        //Go through all cams.
        for (int i = 0; i < cams.Length; i++)
        {

            if(i == 0)
            {
                keptTransform = currentCamPositions[i];
                currentCamPositions[i] = floor1Cams[setCam];
            } else
            {
                Transform temp = currentCamPositions[i];

                currentCamPositions[i] = keptTransform;

                keptTransform = temp;
            }
        }
    }

    void updateCams()
    {
        for(int i = 0; i < cams.Length; i++)
        {
            Vector3 newPos = new Vector3(currentCamPositions[i].position.x, currentCamPositions[i].position.y, currentCamPositions[i].position.z + offset);
            cams[i].transform.position = newPos;
            cams[i].transform.rotation = currentCamPositions[i].rotation;
        }
    }
}
