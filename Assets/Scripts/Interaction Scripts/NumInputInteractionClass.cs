using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumInputInteractionClass : InteractionClass
{
    [SerializeField]
    private int input;

    [SerializeField]
    private GameObject connectedObj;

    public override void Interact()
    {
        if (connectedObj.GetComponent<CombinationManagerClass>())
        {
            connectedObj.GetComponent<CombinationManagerClass>().changeCombination(input);
        }

        if (!controller)
        {
            setController();
        }

        controller.playInteractionAudio(0);
    }
}
