using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HoldInteractionClass : InteractionClass
{
    public bool isHeld;

    //Keep the object that this item is designed to be connected to. This is for plugs and such.
    [SerializeField]
    GameObject connectedObj;


    public Transform currentHolder;

    Rigidbody rig_;

    [SerializeField]
    private interactionType type;

    [Header("Anchor Settings")]
    //An object that this object may be anchored to.
    //If there is no anchor, then this item is free to move.
    [SerializeField]
    Transform anchorObject;

    //The distance an object can go from anchor.
    [SerializeField]
    float anchorThreshold;

    //The distance an object's new position can be before the holding must be disconnected.
    [SerializeField]
    float stretchThreshold;

    private bool playOnce;

    private void Start()
    {
        playOnce = false;
        setController();

        rig_ = GetComponent<Rigidbody>();
    }


    //Try to fix this if at all possible.
    private void FixedUpdate()
    {
        if(currentHolder && currentHolder.position != this.transform.position && !currentHolder.GetComponentInParent<FPSController>())
        {
            setObject(currentHolder.GetChild(0).position, currentHolder.GetChild(0).rotation);
        }
    }

    public override void Interact(Vector3 newPos, Quaternion newRot, Transform obj)
    {
        //Set the object to be held.
        if (!isHeld)
        {
            isHeld = true;

            //Ensure kinematic is on only if a hinge is not used.
            if (!this.GetComponent<HingeJoint>())
            {
                rig_.isKinematic = true;
            }
        } else
        {
            //If already held, remove the current holder if it is not player.
            if (currentHolder && currentHolder.gameObject.GetComponent<PositionInteractionClass>())
            {
                currentHolder.gameObject.GetComponent<PositionInteractionClass>().setCurrentHeldItem(null);
                setSystem(null);
                playOnce = false;
            }
        }

        currentHolder = obj;

        //Set the animation of the controller.
        setObject(newPos, newRot);
    }

    public override void secondaryInteract()
    {
        controller.playInteractionAudio(1);
    }

    //Set this current object with the position and rotation of new 
    void setObject(Vector3 pos, Quaternion rot)
    {

        Vector3 newPos = pos;

        if (anchorObject != null)
        {
            //Ensure new position is within anchor distance.
            if (Vector3.Distance(pos, anchorObject.position) > anchorThreshold)
            {
                //Get direction from anchor to new postion.
                Vector3 dir = (pos - anchorObject.position).normalized;
                newPos = anchorObject.position + dir * anchorThreshold;
            }

            //If the new position is further from the possible position than is the stretch threshhold, break the connection.
            if (Vector3.Distance(pos, newPos) > stretchThreshold)
            {
                //If an fps controller holding this item, then first remove from that.
                if (currentHolder && currentHolder.GetComponentInParent<FPSController>())
                {
                    currentHolder.GetComponentInParent<FPSController>().removeHeldItem();
                    removeHeld();
                }
            }
        }

        //If a hinge and anchor object is provided, then change direction rather than position.
        if (GetComponent<HingeJoint>())
        {
            if (anchorObject && currentHolder)
            {
                Vector3 dir = currentHolder.GetComponentInChildren<Transform>().position - anchorObject.position;
                //dir = Quaternion.Inverse(this.transform.rotation) * dir;

                controller.setAngle(dir.normalized);
            }
        } else
        {
            controller.setPosition(newPos, rot);
        }

        if (!playOnce)
        {
            if (currentHolder && !currentHolder.GetComponent<PositionInteractionClass>())
            {
                controller.playInteractionAudio(0);
                playOnce = true;
            }
        }
    }

    public void removeHeld()
    {
        isHeld = false;
        currentHolder = null;
        rig_.isKinematic = false;

        //To tell controller to remove angle targets.
        controller.unsetAngle();

        playOnce = false;
    }

    //Sets the system if the object is connected to a system.
    public void setSystem(GameObject newObject)
    {
        //Only attempt this if connected object is not null.
        if (connectedObj)
        {
            if (newObject != null)
            {
                //Test to see if the position is connected to the system manager and the ocnnected object is a generator.
                if (newObject.GetComponent<SystemManager>() && connectedObj.GetComponent<GeneratorInteractionClass>())
                {
                    connectedObj.GetComponent<GeneratorInteractionClass>().setManager(newObject.GetComponent<SystemManager>());
                }
            }
            else
            {
                if (connectedObj.GetComponent<GeneratorInteractionClass>())
                {
                    connectedObj.GetComponent<GeneratorInteractionClass>().setManager(null);
                }
            }
        }

        //Other possible systems including:
        //The lock class.
        if (newObject && newObject.GetComponent<LockObjectClass>())
        {
            if (newObject.GetComponent<LockObjectClass>().checkLock())
            {
                newObject.GetComponent<LockObjectClass>().setIsOn(!newObject.GetComponent<EnergyObjectClass>().getIsOn());
                newObject.GetComponent<LockObjectClass>().useObject();
            }
        }
    }

    //Return the type of this hold position.
    public interactionType getType()
    {
        return type;
    }

    //Return the currentHolder gameobject.
    public GameObject getCurrentHolder()
    {
        if (currentHolder)
        {
            return currentHolder.gameObject;
        }

        return null;
    }
}
