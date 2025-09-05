using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerIKController : MonoBehaviour
{
    private Animator anim_;

    public bool ikActive = false;
    public Transform headGoal;
    public Transform lookGoal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim_ = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (anim_)
        {
            if (lookGoal)
            {
                anim_.SetLookAtWeight(1);
                anim_.SetLookAtPosition(lookGoal.position);
            }
        }
    }
}
