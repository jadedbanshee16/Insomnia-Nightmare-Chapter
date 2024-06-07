using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionClass : MonoBehaviour
{
    public enum interactionLevel
    {
        player,
        playerController,
        plug,
    }

    [SerializeField]
    interactionLevel[] permittedInteractions;

    //Interaction base that takes two arguments.
    //The holder transform.
    //The position the item must go to.
    virtual public void interact(Transform holder, Transform newPosition)
    {

    }

    public bool isInteractionType(interactionLevel lvl)
    {
        for(int i = 0; i < permittedInteractions.Length; i++)
        {
            if(permittedInteractions[i] == lvl)
            {
                return true;
            }
        }

        return false;
    }
}
