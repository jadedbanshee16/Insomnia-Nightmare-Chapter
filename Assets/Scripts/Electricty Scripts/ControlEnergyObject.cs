using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlEnergyObject : MonoBehaviour
{
    public enum interactionType
    {
        lights,
        doorMechanisms,
        doors,
        individual
    }

    [SerializeField]
    EnergizerScript powerBox;

    [SerializeField]
    EnergyObject[] interactions_;

    [SerializeField]
    interactionType currentType;

    float totalPower;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < interactions_.Length; i++)
        {
            totalPower += interactions_[i].getEnergyAmount();
        }
    }

    public void setObject(bool isOn)
    {
        //Go through the interactions and use object.
        for (int i = 0; i < interactions_.Length; i++)
        {
            if (interactions_[i].GetComponent<EnergyObject>())
            {
                interactions_[i].GetComponent<EnergyObject>().useObject(isOn);
            }
        }

        if (powerBox)
        {
            powerBox.energize();
        }

    }

    public void setPowerObject(EnergizerScript script)
    {
        powerBox = script;
    }

    public void setPower(bool isOn)
    {
        if (currentType != interactionType.individual)
        {
            powerBox.setSystem((int)currentType, isOn);
        }


    }
}
