using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClass : EnergyObject
{
    [SerializeField]
    private Animator anim_;

    bool opened;

    private void Start()
    {
        dePowerObject();
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
    public override void powerObject()
    {
        //Power itself.
        powered = true;

        setOpened(opened);
    }

    //And override to depower this object and close door.
    public override void dePowerObject()
    {
        powered = false;

        closeDoor();
    }

    //Change if door is open.
    public void setOpened(bool b)
    {
        opened = b;

        if (opened && powered)
        {
            openDoor();
        } else
        {
            closeDoor();
        }
    }
}
