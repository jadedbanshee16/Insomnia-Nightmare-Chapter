using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerIKController : MonoBehaviour
{
    private Animator anim_;

    public InventoryScript inventoryController_;
    public bool ikActive = false;
    public Transform headGoal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim_ = GetComponent<Animator>();

        if(inventoryController_ == null)
        {
            inventoryController_ = GetComponentInChildren<InventoryScript>();
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (anim_)
        {
            if (headGoal != null)
            {
                anim_.SetLookAtWeight(1);
                anim_.SetLookAtPosition(headGoal.position);
            }

            HoldInteractionClass actHand = inventoryController_.getActiveHandObject();

            if (actHand != null)
            {
                anim_.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                anim_.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                anim_.SetIKPosition(AvatarIKGoal.RightHand, actHand.getpickUpPosition().position);
                anim_.SetIKRotation(AvatarIKGoal.RightHand, actHand.getpickUpPosition().rotation);
            }
            else
            {
                anim_.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                anim_.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
        }
    }
}
