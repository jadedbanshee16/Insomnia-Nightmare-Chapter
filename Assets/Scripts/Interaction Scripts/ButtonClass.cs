using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClass : InteractionClass
{

    //[SerializeField]
    //float buttonDuration = 1f;
    //float timer;

    [SerializeField]
    ControlEnergyObject obj_;

    Animator Anim;

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponentInChildren<Animator>();
        //timer = buttonDuration;
    }

    //Interaction with this button.
    public override void interact(Transform player, Transform newPosition)
    {
        //Ensure only press button when not using at the moment.
        Anim.SetTrigger("Pressed");

        obj_.setObject();

        /*if (string.Equals(input, "Clear"))
        {
            //This is the delete button.
            if (screen_.isPowered())
            {
                screen_.clearString();
            }
        }
        else if (string.Equals(input, "Enter"))
        {
            //If enter is pressed, then make it test the current string.
            if (screen_.isPowered())
            {
                screen_.enterString();
            }
        }
        else
        {
            //If it's anything else, then it's a number. Add string.
            if (screen_.isPowered())
            {
                screen_.addString(input);
            }
        }*/
    }

}
