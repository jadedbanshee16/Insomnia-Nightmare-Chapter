using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightObject : EnergyObject
{
    bool isOn;

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

        setLight(false);

        isOn = true;
    }

    //A function to power the object.
    public override void powerObject()
    {
        //Power itself.
        powered = true;

        setLight(true);
    }

    //A function to depower the object.
    public override void dePowerObject()
    {
        //Remove power of self.
        powered = false;

        //Set light depending of it is powered or not.
        setLight(false);
    }

    //Set the light by turning on or off the light, and setting the light material.
    public void setLight(bool on)
    {
        isOn = on;

        if (isOn && powered)
        {
            lightMat_.material = onMat;
            theLight.gameObject.SetActive(true);
        } else
        {
            lightMat_.material = offMat;
            theLight.gameObject.SetActive(false);
        }
    }
}
