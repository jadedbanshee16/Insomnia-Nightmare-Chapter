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

        useObject(false);
    }

    //A function to power the object.
    public override void powerObject()
    {
        //Power itself.
        powered = true;
        useObject(isOn);
    }

    //A function to depower the object.
    public override void dePowerObject()
    {
        //Power itself.
        powered = false;
        useObject(isOn);
    }

    //Set the light by turning on or off the light, and setting the light material.
    public override void useObject(bool on)
    {
        isOn = on;

        if (isOn && powered)
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

    public override bool isUsed()
    {
        return isOn;
    }
}
