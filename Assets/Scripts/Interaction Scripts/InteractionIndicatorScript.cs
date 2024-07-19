using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionIndicatorScript : MonoBehaviour
{
    [SerializeField]
    Material[] lightMats;

    [SerializeField]
    MeshRenderer primaryLight;
    [SerializeField]
    MeshRenderer secondaryLight;

    //Used for an indicator.
    public void switchToOn()
    {

        primaryLight.material = lightMats[1];

        //If there is a secondary light, then switch that as well.
        if (secondaryLight)
        {
            secondaryLight.material = lightMats[0];
        }

    }

    public void switchToOff()
    {
        primaryLight.material = lightMats[0];

        if (secondaryLight)
        {
            secondaryLight.material = lightMats[2];
        }
    }
}
