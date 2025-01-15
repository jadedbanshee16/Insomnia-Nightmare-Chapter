using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintanenceObjectClass : EnergyObjectClass
{
    [SerializeField]
    GeneratorInteractionClass gen;

    public override void useObject()
    {
        gen.setIsFixed(isOn);
        gen.setObject();
    }
}
