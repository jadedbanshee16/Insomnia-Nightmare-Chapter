using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClass : InteractionClass
{


    [SerializeField]
    string input;

    //[SerializeField]
    //float buttonDuration = 1f;
    //float timer;

    [SerializeField]
    KeyPadScreen screen_;

    Animator Anim;
    bool isUsing = false;
    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
        //timer = buttonDuration;
    }

    //Interaction with this button.
    public override void interact(Transform player, Transform newPosition)
    {
        //Ensure only press button when not using at the moment.
        Anim.SetTrigger("Pressed");

        if (string.Equals(input, "Clear"))
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
        }
    }

}