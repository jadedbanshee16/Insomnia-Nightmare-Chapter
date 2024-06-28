using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchClass : InteractionClass
{
    bool isOn;

    [SerializeField]
    InteractionIndicatorScript switchIndicator;

    Animator Anim;

    ControlEnergyObject objectController;

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponentInChildren<Animator>();
        objectController = GetComponent<ControlEnergyObject>();

        switchIndicator = GetComponent<InteractionIndicatorScript>();

        isOn = false;
        //setSwitch();

        if (Anim)
        {
            Anim.SetBool("isOn", isOn);
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

    //Interaction with this button.
    public override void interact(Transform player, Transform newPosition)
    {
        //Ensure only press button when not using at the moment.
        isOn = !isOn;
        setSwitch();

        objectController.setPower(isOn);
    }

    //A function to set the animation and light of the switch.
    private void setSwitch()
    {
        if (Anim)
        {
            Anim.SetBool("isOn", isOn);
        }

        objectController.setObject(isOn);


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
        objectController.setPower(isOn);
    }
}
