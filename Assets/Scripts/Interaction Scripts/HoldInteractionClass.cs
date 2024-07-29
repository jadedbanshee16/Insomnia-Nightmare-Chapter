using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HoldInteractionClass : InteractionClass
{
    private bool isHeld;

    //Keep the object that this item is designed to be connected to. This is for plugs and such.
    [SerializeField]
    GameObject connectedObj;

    private Transform currentHolder;

    Rigidbody rig_;

    [SerializeField]
    private interactionType type;

    private void Start()
    {
        setController();

        rig_ = GetComponent<Rigidbody>();
    }

    public override void Interact(Vector3 newPos, Quaternion newRot, Transform obj)
    {
        //Set the animation of the controller.
        setObject(newPos, newRot);

        //Set the object to be held.
        if (!isHeld)
        {
            isHeld = true;
            rig_.isKinematic = true;
        } else
        {
            //If already held, remove the current holder if it is not player.
            if (currentHolder.gameObject.GetComponent<PositionInteractionClass>())
            {
                currentHolder.gameObject.GetComponent<PositionInteractionClass>().setCurrentHeldItem(null);
                setSystem(null);
            }
        }

        currentHolder = obj;
    }

    void setObject(Vector3 pos, Quaternion rot)
    {
        controller.setPosition(pos, rot);
    }

    public void removeHeld()
    {
        isHeld = false;
        currentHolder = null;
        rig_.isKinematic = false;
    }

    public void setSystem(GameObject newObject)
    {
        if (newObject)
        {
            //Test to see if the position is connected to the system manager and the ocnnected object is a generator.
            if (newObject.GetComponent<SystemManager>() && connectedObj.GetComponent<GeneratorInteractionClass>())
            {
                connectedObj.GetComponent<GeneratorInteractionClass>().setManager(newObject.GetComponent<SystemManager>());
            }
        } else
        {
            if (connectedObj.GetComponent<GeneratorInteractionClass>())
            {
                connectedObj.GetComponent<GeneratorInteractionClass>().setManager(null);
            }
        }

    }

    public interactionType getType()
    {
        return type;
    }
}
