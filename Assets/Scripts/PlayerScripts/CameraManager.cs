using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera[] cams_;

    private int curCam;
    // Start is called before the first frame update
    void Start()
    {
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
