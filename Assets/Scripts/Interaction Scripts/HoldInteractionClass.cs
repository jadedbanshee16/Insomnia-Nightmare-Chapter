using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HoldInteractionClass : InteractionClass
{
    public bool isHeld;
    private bool wasHeld;

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

    [SerializeField]
    bool invertHoldAngle;

    private bool playOnce;

    private bool isPlayerHeld = false;

    private void Start()
    {
        playOnce = false;
        setController();

        rig_ = GetComponent<Rigidbody>();
    }


    //Try to fix this if at all possible.
    private void Update()
    {
        if(currentHolder && getType() != interactionType.senserInteraction)
        {
            if(Vector3.Distance(currentHolder.GetChild(0).position, this.transform.position) > 0.0000002f)
            {
                //Ensure that if an object that is supposed to move even when locked in place, it can move physically. Something like doors
                //Dont know why this works without, but be watchful for door bugs.
                /*if(getType() == interactionType.autoPosition && !isPlayerHeld)
                {
                    rig_.isKinematic = false;
                } else
                {

                }*/

                setObject(currentHolder.GetChild(0).position, currentHolder.GetChild(0).rotation);
            } else
            {
                //Ensure held object remains held when it hits a position cannot be pushed if locked in place.
                rig_.isKinematic = true;
            }

            /*if (string.Equals(this.gameObject.name, "Can3(Special)"))
            {
                Debug.Log("Used at update: " + currentHolder.gameObject.name);
            }*/
        }
    }

    public override void Interact(Vector3 newPos, Quaternion newRot, Transform obj)
    {
        if (!rig_)
        {
            rig_ = GetComponent<Rigidbody>();
        }

        //Set the object to be held.
        if (!isHeld)
        {
            isHeld = true;
        } else
        {
            wasHeld = true;
            //If already held, remove the current holder if it is not player.
            if (currentHolder && currentHolder.gameObject.GetComponent<PositionInteractionClass>())
            {
                playOnce = false;
                
                currentHolder.gameObject.GetComponent<PositionInteractionClass>().setCurrentHeldItem(null);
                setSystem(null);
            }
        }

        currentHolder = obj;

        //Ensure kinematic is on only if a hinge is not used.
        //If used with hing joint, hinge will not swing as normal.
        if (!this.GetComponent<HingeJoint>())
        {
            rig_.isKinematic = true;
        } else
        {
            //When interacted with, ensure rig can be rotated first.
            rig_.isKinematic = false;
        }

        /*if(string.Equals(this.gameObject.name, "Can3(Special)"))
        {
            if(obj != null)
            {
                Debug.Log("Used at interaction: " + obj.gameObject.name);
            } else
            {
                Debug.Log("Used at interaction: " + "null");
            }
           
        }*/

        //Set the animation of the controller.
        setObject(newPos, newRot);

        if (currentHolder && currentHolder.GetComponentInParent<FPSController>())
        {
            isPlayerHeld = true;
        } else
        {
            isPlayerHeld = false;
        }
    }

    public override void secondaryInteract()
    {
        controller.playInteractionAudio(2);
    }

    //Set this current object with the position and rotation of new 
    void setObject(Vector3 pos, Quaternion rot)
    {
        if (!controller)
        {
            setController();
        }

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
                //Vector3 dir = pos - anchorObject.position;
                //dir = Quaternion.Inverse(this.transform.rotation) * dir;
                controller.setAngle(pos, anchorObject, invertHoldAngle);
            }
        } else
        {
            controller.setPosition(newPos, rot);
        }

        if (!playOnce)
        {
            if (currentHolder && !currentHolder.GetComponent<PositionInteractionClass>())
            {
                if (!wasHeld)
                {
                    controller.playInteractionAudio(1);
                } else
                {
                    controller.playInteractionAudio(0);
                }

                
                playOnce = true;
            }
        }
    }

    public void removeHeld()
    {
        isHeld = false;
        wasHeld = false;
        currentHolder = null;

        if (!rig_)
        {
            rig_ = GetComponent<Rigidbody>();
        }

        rig_.isKinematic = false;

        setController();

        //To tell controller to remove angle targets.
        controller.unsetAngle();

        playOnce = false;
        isPlayerHeld = false;
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
        //The inverted lock class
        //The lock class.
        //MultiInteraction class
        //Energy class
        if(newObject && newObject.GetComponent<InvertedLockObjectClass>())
        {
            if (!newObject.GetComponent<LockObjectClass>().checkLock())
            {
                //Force close.
                newObject.GetComponent<LockObjectClass>().forceClose();
            }

            newObject.GetComponent<LockObjectClass>().setIsOn(true);
            newObject.GetComponent<LockObjectClass>().useObject();
        } else if (newObject && newObject.GetComponent<LockObjectClass>())
        {
            if (newObject.GetComponent<LockObjectClass>().checkLock())
            {
                newObject.GetComponent<LockObjectClass>().setIsOn(!newObject.GetComponent<EnergyObjectClass>().getIsOn());
                newObject.GetComponent<LockObjectClass>().useObject();
            }
        } else if (newObject && newObject.GetComponent<EnergyObjectClass>() && !newObject.GetComponent<LockObjectClass>())
        {
            newObject.GetComponent<EnergyObjectClass>().getEnergyManager().updateObject(newObject.GetComponent<EnergyObjectClass>(), true);
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

    public Transform getAnchor()
    {
        return anchorObject;
    }
}
