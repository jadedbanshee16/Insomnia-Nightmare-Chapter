using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClass : EnergyObject
{
    [SerializeField]
    private Animator anim_;

    public bool opened;

    private void Start()
    {
        powerObject(false);
    }


    //Use animation to close the door.
    private void closeDoor()
    {
        anim_.SetBool("Open", false);
    }

    //Use animation to open door.
    private void openDoor()
    {
        //Debug.Log("Worked");
        anim_.SetBool("Open", true);
    }

    //An override to power this object and find open or close door.
    public override void powerObject(bool b)
    {
        //Power itself.
        opened = b;
    }

    //Change if door is open.
    public override void useObject()
    {

        if (opened && powered)
        {
            openDoor();

        } else
        {
            closeDoor();
        }
    }
}
