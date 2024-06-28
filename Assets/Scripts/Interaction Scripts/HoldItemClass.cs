using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldItemClass : InteractionClass
{
    //Hold the current position class.
    protected Transform currentHolder;

    protected Transform currentTransform;
    protected Rigidbody _body;

    //Variables used to lock the item to a position while being held.
    [SerializeField]
    protected Transform lockedPosition;
    [SerializeField]
    protected float dist;
    protected float maxDist;

    protected bool fixedPosition;
    protected Transform newPosition;

    void Start()
    {
        currentTransform = GetComponent<Transform>();

        //Set the distance from the locked position.
        if (dist == 0)
        {
            dist = Vector3.Distance(lockedPosition.position, currentTransform.position);
        }

        fixedPosition = false;

        currentHolder = null;
    }

    void Update()
    {
        if (_body == null)
        {
            _body = GetComponentInParent<Rigidbody>();
        }

        //Check if position of plug is picked up or plugged in.
        if (fixedPosition)
        {
            Vector3 theNewPosition = Vector3.zero;
            Quaternion theNewRotation = newPosition.rotation;

            //Get current position between a the current position and the new position.
            if(Vector3.Distance(lockedPosition.position, newPosition.position) < dist)
            {
                theNewPosition = newPosition.position;
            } else
            {
                //Get direction of locked position to new position.
                Vector3 dir = (newPosition.position - lockedPosition.position).normalized;
                dir.y = 0;

                //Make the position equal to the certain distance from the locked position in the newPosition direction.
                theNewPosition = lockedPosition.position + (dir * dist);
            }

            //If picked up, ensure it remains with new position.
            if (currentTransform.position != theNewPosition)
            {
                currentTransform.position = theNewPosition;
                currentTransform.rotation = theNewRotation;
            }
        }

        updateSubClass(fixedPosition);
    }


    //Base interaction of the interaction class.
    public override void interact(Transform holder, Transform newPos)
    {
        if (currentHolder != null)
        {
            //If current holder, find either player or held item class to remove item from that point.
            release(currentHolder);
        }

        //Debug.Log("Working?");
        fixedPosition = true;

        newPosition = newPos;



        currentHolder = holder;

        //_cable.removeColliders();
    }

    //Base function to remove item from current holder.
    public virtual void release(Transform holder)
    {
        //Debug.Log("Released");

        fixedPosition = false;
        newPosition = null;

        //Find out what is holding item if item held.
        if (holder && holder.GetComponent<FPSController>())
        {
            holder.GetComponent<FPSController>().removeHeldItem();
        }
        else if (holder && holder.GetComponent<ItemPositionClass>())
        {
            holder.GetComponent<ItemPositionClass>().removeHeldItem();
        }

        //Set current holder to null.
        currentHolder = null;

        //forceAddColliders();
    }

    public virtual void updateSubClass(bool hasFixedPos)
    {
        //A 
    }
}
