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
    protected EnergyObject[] interactions_;

    [SerializeField]
    interactionType currentType;

    public virtual void setObject(bool isOn)
    {
        if(currentType == interactionType.individual)
        {
            for (int i = 0; i < interactions_.Length; i++)
            {
                powerBox.useObject(interactions_[i], isOn);
            }
        } else
        {
            powerBox.setSystem((int)currentType, isOn);
        }
    }

    public virtual void setObject()
    {
        //By default, this should not be used. This is specific to setting up inputs using the inheritance.
    }

    public void setPowerObject(EnergizerScript script)
    {
        powerBox = script;
    }

    public interactionType getPowerType()
    {
        return currentType;
    }
}
