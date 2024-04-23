using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldItemClass : InteractionClass
{
    //Hold the current position class.
    protected Transform currentHolder;


    //Base interaction of the interaction class.
    public override void interact(Transform holder, Transform newPosition)
    {
    }

    //Base function to remove item from current holder.
    public virtual void release(Transform holder)
    {
    }

    //Turn on colliders for cable items.
    public virtual void forceAddColliders()
    {

    }
}
