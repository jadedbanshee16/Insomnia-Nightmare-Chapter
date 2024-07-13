using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightObject : EnergyObject
{
    public bool isOn;

    MeshRenderer lightMat_;
    Light theLight;

    [SerializeField]
    Material onMat;
    [SerializeField]
    Material offMat;

    // Start is called before the first frame update
    void Start()
    {
        lightMat_ = GetComponentInChildren<MeshRenderer>();
        theLight = GetComponentInChildren<Light>();
    }

    public override void powerObject(bool b)
    {
        isOn = b;
    }

    //Set the light by turning on or off the light, and setting the light material.
    public override void useObject()
    {

        if (isOn)
        {
            if (lightMat_)
            {
                lightMat_.material = onMat;
            }
            theLight.gameObject.SetActive(true);
        } else
        {
            if (lightMat_)
            {
                lightMat_.material = offMat;
            }
            theLight.gameObject.SetActive(false);
        }
    }
}
