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

    Vector3 lastTransform;

    // Start is called before the first frame update
    void Start()
    {
        rig_ = GetComponent<Rigidbody>();
        controller = GetComponent<InteractionControlClass>();
        lastTransform = this.transform.position;
    }

    private void Update()
    {
        if(Vector3.Distance(lastTransform, this.transform.position) > 0.01)
        {
            controller.setAnimation("Shake");
        }

        lastTransform = this.transform.position;
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
