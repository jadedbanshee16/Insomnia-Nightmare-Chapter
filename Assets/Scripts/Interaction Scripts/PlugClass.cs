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
    public override void interact(Transform heldPosition)
    {
        Debug.Log("Working?");
        fixedPosition = true;

        newPosition = heldPosition;

        _cable.removeColliders();
    }

    //A function to release item from where it is held.
    public override void release()
    {
        fixedPosition = false;
        newPosition = null;

        GameObject item = GameObject.Find("Player").GetComponent<FPSController>().getHeldItem();

        //Ensure player is no longer holding this item.
        if (item && item == this.gameObject)
        {
            GameObject.Find("Player").GetComponent<FPSController>().removeHeldItem();
        }

        forceAddColliders();
    }

    //A function to test the rope position to make it not stretch.
    private void testRope()
    {
        if (_cable.testLinePositions())
        {
            release();
        }
    }

    public override void forceAddColliders()
    {
        _cable.addColliders();
    }
}
