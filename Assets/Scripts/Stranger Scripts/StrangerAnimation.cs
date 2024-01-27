using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangerAnimation : MonoBehaviour
{
    //Stranger animator
    private Animator anim_;

    //Player object to look at.
    private Transform player_;

    //Is IK active?
    private bool IKActive;

    //Is looking at player.
    private bool isLooking;

    // Start is called before the first frame update
    void Start()
    {
        anim_ = gameObject.GetComponent<Animator>();
        player_ = GameObject.FindGameObjectWithTag("Player").transform;

        IKActive = false;
        isLooking = false;
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        //Does animator exist?
        if (anim_ != null)
        {
            //if the IK is active, set the position and rotation directly to the goal.
            if (IKActive)
            {
                // Set the look target position, if one has been assigned
                if (player_ != null)
                {
                    //Set stranger hands to reach out to players.
                    anim_.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.3f);
                    anim_.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.3f);
                    anim_.SetIKPosition(AvatarIKGoal.RightHand, player_.position);
                    anim_.SetIKRotation(AvatarIKGoal.RightHand, player_.rotation);

                    anim_.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.3f);
                    anim_.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0.3f);
                    anim_.SetIKPosition(AvatarIKGoal.LeftHand, player_.position);
                    anim_.SetIKRotation(AvatarIKGoal.LeftHand, player_.rotation);
                }

            }
            else
            {
                //Unset everything.
                anim_.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                anim_.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                anim_.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                anim_.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }

            if (isLooking)
            {
                //Set where Stranger look at.
                anim_.SetLookAtWeight(1);
                anim_.SetLookAtPosition(player_.position);
            } else
            {
                anim_.SetLookAtWeight(0);
            }
        }
    }

    public void setSpeed(float s)
    {
        anim_.SetFloat("Speed", s);
    }

    public void setIK(bool b)
    {
        IKActive = b;
    }

    public void setMoveState(bool b)
    {
        anim_.SetBool("Move", b);
    }

    public void setIsLooking(bool b)
    {
        isLooking = b;
    }
}
