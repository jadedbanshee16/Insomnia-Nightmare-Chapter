using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionClass : MonoBehaviour
{
    protected enum interactionLevel
    {
        player,
        plug
    }

    [SerializeField]
    interactionLevel[] permittedInteractions;

    //Interaction base that takes two arguments.
    //The holder transform.
    //The position the item must go to.
    virtual public void interact(Transform holder, Transform newPosition)
    {

    }
}
