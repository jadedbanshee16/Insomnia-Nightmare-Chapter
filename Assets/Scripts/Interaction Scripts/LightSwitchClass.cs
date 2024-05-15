using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitchClass : InteractionClass
{

    bool isOn;

    [SerializeField]
    InteractionIndicatorScript switchIndicator;

    [SerializeField]
    EnergizerScript powerBox;

    [SerializeField]
    EnergyObject[] interactions_;

    float totalPower;

    Animator Anim;
    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponentInChildren<Animator>();

        switchIndicator = GetComponent<InteractionIndicatorScript>();

        isOn = false;
        setSwitch();

        for (int i = 0; i < interactions_.Length; i++)
        {
            totalPower += interactions_[i].getEnergyAmount();
        }

    }

    //Interaction with this button.
    public override void interact(Transform player, Transform newPosition)
    {
        //Ensure only press button when not using at the moment.
        isOn = !isOn;
        setSwitch();

        if (isOn)
        {
            powerBox.energize();
        }

    }

    //A function to set the animation and light of the switch.
    private void setSwitch()
    {
        Anim.SetBool("isOn", isOn);

        //Go through the interactions and use object.
        for (int i = 0; i < interactions_.Length; i++)
        {
            if (interactions_[i].GetComponent<EnergyObject>())
            {
                interactions_[i].GetComponent<EnergyObject>().useObject(isOn);
            }
        }

        if (isOn)
        {
            if (switchIndicator)
            {
                switchIndicator.switchToOn();
            }
        }
        else
        {
            if (switchIndicator)
            {
                switchIndicator.switchToOff();
            }
        }

    }

    //A function to turn off light.
    public void turnOff()
    {
        isOn = false;
        setSwitch();
    }
}
