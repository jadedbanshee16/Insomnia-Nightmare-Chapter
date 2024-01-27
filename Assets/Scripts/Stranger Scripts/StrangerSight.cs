using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*A script to collect trigger and sight information for the 'eyes' collider.*/

public class StrangerSight : MonoBehaviour
{
    //Booleans to see player.
    private bool seePlayer;

    public GameObject parent;

    private void Update()
    {
        //Change the roation of y to match that of the parent.
        var yRotation = parent.transform.eulerAngles.y;
        this.transform.rotation = Quaternion.Euler(this.transform.eulerAngles.x, yRotation + 90, this.transform.eulerAngles.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check if looking at the player. If so, make a ray pass.
        if (other.gameObject.CompareTag("Player"))
        {
            seePlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Check if looking at the player. If so, make a ray pass.
        if (other.gameObject.CompareTag("PlayerHitbox"))
        {
            seePlayer = false;
        }
    }

    public bool castRayCheck(Vector3 pos)
    {
        //Create a hit.
        RaycastHit hit;
        //Create ray that heads to specified position.
        Ray ray = new Ray(transform.position, pos - transform.position);
        // Send ray 
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("tag: " + hit.collider.gameObject.tag + ", name: " + hit.collider.gameObject.name);
            if (hit.collider.gameObject.name == "Player")
            {
                return true;
            }
        }

        //If the ray hasn't hit what it was after, there is cover so return false.
        return false;
    }

    public bool getSeePlayer()
    {
        return seePlayer;
    }
}
