using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClass : MonoBehaviour
{
    [SerializeField]
    private Animator anim_;

    bool energized;
    bool opened;

    private void Start()
    {
        energized = false;
        closeDoor();
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
        if (energized)
        {

            anim_.SetBool("Open", true);
        }
    }

    //Run the door when energize.
    public void energizeItem(bool b)
    {
        energized = b;

        if (!b)
        {
            closeDoor();
        } else
        {
            setOpened(opened);
        }
    }

    //Change if door is open.
    public void setOpened(bool b)
    {
        opened = b;

        if (opened && energized)
        {
            openDoor();
        } else
        {
            closeDoor();
        }
    }
}
