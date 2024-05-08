using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchClass : InteractionClass
{


    [SerializeField]
    EnergyObject.objectType index;

    bool isOn;

    [SerializeField]
    EnergizerScript powerBox;


    [SerializeField]
    Material[] lightMats;

    [SerializeField]
    MeshRenderer onLight;
    [SerializeField]
    MeshRenderer offLight;



    Animator Anim;
    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponentInChildren<Animator>();

        isOn = false;
        setSwitch();
        powerBox.setSystem((int)index, isOn);
    }

    //Interaction with this button.
    public override void interact(Transform player, Transform newPosition)
    {
        //Ensure only press button when not using at the moment.
        isOn = !isOn;
        setSwitch();

        powerBox.setSystem((int)index, isOn);
    }

    //A function to set the animation and light of the switch.
    private void setSwitch()
    {
        Anim.SetBool("isOn", isOn);

        if (isOn)
        {
            onLight.material = lightMats[1];
            offLight.material = lightMats[0];
        } else
        {
            onLight.material = lightMats[0];
            offLight.material = lightMats[2];
        }
    }

    //A function to turn off light.
    public void turnOff()
    {
        isOn = false;
        setSwitch();
        powerBox.setSystem((int)index, isOn);
    }

}
