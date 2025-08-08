using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is designed to add additional controller actions for some rigid body objects.
/// This includes:
/// 1. Making sounds or actions when touching other objects.
/// </summary>
public class ObjectScript : MonoBehaviour
{
    Rigidbody rig_;

    InteractionControlClass controller;

    HoldInteractionClass holdInteractionsystem;

    Vector3 lastTransform;
    [SerializeField]
    Vector3 anchorPointPos;

    bool wasHeld;

    // Start is called before the first frame update
    void Start()
    {
        rig_ = GetComponent<Rigidbody>();
        controller = GetComponent<InteractionControlClass>();
        holdInteractionsystem = GetComponent<HoldInteractionClass>();
        lastTransform = this.transform.position;
        anchorPointPos = this.transform.position;

        wasHeld = false;
    }

    private void Update()
    {
        if(Vector3.Distance(lastTransform, this.transform.position) > 0.01)
        {
            controller.setAnimation("Shake");
        }

        lastTransform = this.transform.position;

        //This is a system that retrieves the item if it falls and is not held.
        //This is so objects that fall through the map can still be accessed from the original point.
        if (Vector3.Distance(anchorPointPos, this.transform.position) > 100f && !holdInteractionsystem.getIsHeld())
        {
            this.transform.position = new Vector3(anchorPointPos.x, anchorPointPos.y + 0.1f, anchorPointPos.z);
            rig_.linearVelocity = Vector3.up + Vector3.right;
        }

        //This is to change the new anchor point, so a player can pick up and change a given anchor point.
        if (holdInteractionsystem.getIsHeld())
        {
            wasHeld = true;
        } else
        {
            if (wasHeld)
            {
                wasHeld = false;
                anchorPointPos = this.transform.position;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!rig_)
        {
            rig_ = GetComponent<Rigidbody>();
        }

        if (!controller)
        {
            controller = GetComponent<InteractionControlClass>();
        }

        //If on collision, and not kimetic, make a sound.
        if (!rig_.isKinematic && collision.gameObject.layer != LayerMask.NameToLayer("PlayerLayer") && collision.gameObject.layer != LayerMask.NameToLayer("MovementLayer"))
        {
            controller.playInteractionAudio(0);
        }
    }
}
