using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera[] cams_;

    [SerializeField]
    CinemachineVirtualCamera initialCam;

    private int curCam;
    // Start is called before the first frame update
    void Start()
    {
        CinemachineVirtualCamera[] allcams = GameObject.FindObjectsByType<CinemachineVirtualCamera>(FindObjectsSortMode.None);

        //Debug.Log(allcams.Length);

        cams_ = new CinemachineVirtualCamera[allcams.Length];

        cams_[0] = initialCam;

        int foundObj = 1;
        //Go through and add allcams to camera base, ensuring the initial camera is the first camera.
        for(int i = 0; i < allcams.Length; i++)
        {
            if(initialCam != allcams[i])
            {
                cams_[foundObj] = allcams[i];
                foundObj++;
            }
        }
        
        curCam = 0;
        setCamera(0);
    }

    private void updateCamera(int cam)
    {
        for(int i = 0; i < cams_.Length; i++)
        {
            if(i != cam)
            {
                cams_[i].gameObject.SetActive(false);
            } else
            {
                cams_[i].gameObject.SetActive(true);
            }
        }
    }

    public int findCamera(CinemachineVirtualCamera cam)
    {
        for(int i = 0; i < cams_.Length; i++)
        {
            if(cams_[i] == cam)
            {
                return i;
            }
        }

        return 0;
    }

    public void setCamera(int cam)
    {
        curCam = cam;

        updateCamera(curCam);
    }
}
