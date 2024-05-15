using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionIndicatorScript : MonoBehaviour
{
    [SerializeField]
    Material[] lightMats;

    [SerializeField]
    MeshRenderer onLight;
    [SerializeField]
    MeshRenderer offLight;

    //Used for an indicator.
    public void switchToOn()
    {
        onLight.material = lightMats[1];
        offLight.material = lightMats[0];
    }

    public void switchToOff()
    {
        onLight.material = lightMats[0];
        offLight.material = lightMats[2];
    }
}
