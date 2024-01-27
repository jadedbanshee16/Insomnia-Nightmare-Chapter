using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StrangerMovement : MonoBehaviour
{
    /*Set speeds*/
    public float SprintSpeed;
    public float StandardSpeed;
    public float WalkSpeed;

    public float walkRad;

    private Vector3 curTarget_;

    private NavMeshAgent nav_;

    // Start is called before the first frame update
    void Start()
    {
        nav_ = gameObject.GetComponent<NavMeshAgent>();

        //Set speed to default speed.
        setSpeed("");
    }

    /*Get character to move to a specified position using Navmesh.
     */
    public void moveTo()
    {
        nav_.SetDestination(curTarget_);
    }

    /*Check if stranger is near or on the position they want to get to.*/
    public bool atPosition()
    {
        return Vector3.Distance(curTarget_, transform.position) < nav_.stoppingDistance;
    }

    /*Code find and set a valid position to move towards*/
    public bool changeTargetPosition()
    {
        curTarget_ = getPosition();
        //Generate the path to see if it is valid.
        NavMeshPath path = new NavMeshPath();
        nav_.CalculatePath(curTarget_, path);
        //Check to see if the path is valid.
        return path.status == NavMeshPathStatus.PathComplete;
    }

    /*Using the nearest position, set current target to that position.*/
    public bool changeTargetPosition(Vector3 pos)
    {
        curTarget_ = getPosition(pos);

        //Generate the path to see if it is valid.
        NavMeshPath path = new NavMeshPath();
        nav_.CalculatePath(curTarget_, path);
        //Check to see if the path is valid.
        return path.status == NavMeshPathStatus.PathComplete;
    }

    /*A function to set the rotation of the enemy to face the target location.*/
    public void rotateToTarget(Vector3 tar)
    {
        //Get direction to look.
        Vector3 lookPos = tar - transform.position;
        //Do not change the y.
        lookPos.y = 0;
        //Convert to Quaternion.
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        //Change rotation over time to face new target.
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2);
    }

    public void setSpeed(string s)
    {
        if(string.Equals(s, "sprint"))
        {
            nav_.speed = SprintSpeed;
        } else if (string.Equals(s, "walk"))
        {
            nav_.speed = WalkSpeed;
        } else
        {
            nav_.speed = StandardSpeed;
        }
    }

    /*Find a random point on navmesh to go to.
     */
    Vector3 getPosition()
    {
        //Get a random point in a sphere walking distance from the stranger.
        Vector3 randDir = Random.insideUnitSphere * walkRad;
        //Set it around the character.
        randDir += transform.position;

        //Get the closest sample position on the NavMesh.
        NavMeshHit hit;
        NavMesh.SamplePosition(randDir, out hit, walkRad, 1);

        //Return postion.
        return hit.position;
    }

    /*Set random position to a given position on the map.*/
    Vector3 getPosition(Vector3 pos)
    {
        //Get the closest sample position on the NavMesh.
        NavMeshHit hit;
        NavMesh.SamplePosition(pos, out hit, walkRad, 1);

        //Return postion.
        return hit.position;
    }
}
