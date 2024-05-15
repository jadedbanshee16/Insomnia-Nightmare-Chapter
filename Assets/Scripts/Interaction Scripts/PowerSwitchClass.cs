using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSwitchClass : InteractionClass
{

    [SerializeField]
    EnergyObject.objectType index;

    bool isOn;

    [SerializeField]
    EnergizerScript powerBox;

    [SerializeField]
    InteractionIndicatorScript switchIndicator;


    Animator Anim;
    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponentInChildren<Animator>();

        switchIndicator = GetComponent<InteractionIndicatorScript>();

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

        if (switchIndicator)
        {
            if (isOn)
            {
                switchIndicator.switchToOn();
            }
            else
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
        powerBox.setSystem((int)index, isOn);
    }

}
