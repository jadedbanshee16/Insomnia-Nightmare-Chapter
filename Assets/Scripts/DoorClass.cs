using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorClass : MonoBehaviour
{
    [SerializeField]
    private Animator anim_;

    bool energized;

    private void Start()
    {
        energized = true;
        closeDoor();
    }

    public void closeDoor()
    {
        anim_.SetBool("Open", false);
    }

    public void openDoor()
    {
        Debug.Log("Worked");
        if (energized)
        {

            anim_.SetBool("Open", true);
        }
    }


    public void getEnergized(bool b)
    {
        energized = b;

        if (!b)
        {
            closeDoor();
        }
    }
}
