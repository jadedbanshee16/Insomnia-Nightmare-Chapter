using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugClass : HoldItemClass
{
    Transform currentTransform;
    Rigidbody _body;

    bool fixedPosition;
    Transform newPosition;

    [SerializeField]
    RopeController _cable;

    // Start is called before the first frame update
    void Start()
    {
        currentTransform = GetComponent<Transform>();

        fixedPosition = false;

        currentHolder = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (_body == null)
        {
            _body = GetComponentInParent<Rigidbody>();
        }

        //Check if position of plug is picked up or plugged in.
        if (fixedPosition)
        {

            _body.isKinematic = true;

            //If picked up, ensure it remains with new position.
            if(currentTransform.position != newPosition.position)
            {
                currentTransform.position = newPosition.position;
                currentTransform.rotation = newPosition.rotation;
            }
        } else
        {
            _body.isKinematic = false;
        }

        testRope();
    }

    //A function to do what an interaction would do.
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

        _cable.removeColliders();
    }

    //A function to release item from where it is held.
    public override void release(Transform holder)
    {
        Debug.Log("Released");
        
        fixedPosition = false;
        newPosition = null;

        //Find out what is holding item if item held.
        if (holder && holder.GetComponent<FPSController>())
        {
            holder.GetComponent<FPSController>().removeHeldItem();
        } else if (holder && holder.GetComponent<ItemPositionClass>())
        {
            holder.GetComponent<ItemPositionClass>().removeHeldItem();
        }

        //Set current holder to null.
        currentHolder = null;

        forceAddColliders();
    }

    //A function to test the rope position to make it not stretch.
    private void testRope()
    {
        if (_cable.testLinePositions())
        {
            Debug.Log("Cable Fail");
            release(currentHolder);
        }
    }

    public override void forceAddColliders()
    {
        _cable.addColliders();
    }
}
