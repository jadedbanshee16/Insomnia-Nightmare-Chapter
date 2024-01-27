using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrangerAI : MonoBehaviour
{
    /*Outside components*/
    //Player transform.
    private Transform player_;
    //Stranger movement script for movement related aspects of the enemy.
    private StrangerMovement strangerMov_;
    //Stranger animation script for animation related aspects of the enemy
    private StrangerAnimation strangerAnim_;
    //The Stranger eyes collider.
    private StrangerSight strangerEye_;

    /*Speed enums*/
    enum strangerSpeed
    {
        sprint,
        standard,
        walk
    }

    /*Stranger Status enum
     * Search: Random walk
     * Alert: Stand still after seeing player and set stare.
     * Investigate: Go to specific location and look around.
     * chase: Go to certain location in alarm mode.
     * Run: Get away in a specific direction.
     */
    public enum strangerStatus
    {
        search,
        alert,
        investigate,
        chase,
        run
    }

    /*Status booleans*/
    //True if at desired position.
    private bool atPos;
    //True if current target position had been successfully changed.
    private bool changePos;
    //True when Stranger is currently in an idle cycle.
    private bool isIdle;

    public strangerStatus stat;

    /*Timers*/
    private float idleTimer;
    private float idleTime;

    private float alertTimer;
    private float alertTime;

    // Start is called before the first frame update
    void Start()
    {
        strangerAnim_ = gameObject.GetComponentInChildren<StrangerAnimation>();
        player_ = GameObject.FindGameObjectWithTag("Player").transform;
        strangerMov_ = gameObject.GetComponent<StrangerMovement>();
        strangerEye_ = gameObject.GetComponentInChildren<StrangerSight>();

        //Set starting 
        changePos = true;
        atPos = true;
        isIdle = false;

        //Start stat in search mode.
        stat = strangerStatus.search;
    }

    // Update is called once per frame
    void Update()
    {
        //Complete if stranger is in the search status.
        if (stat == strangerStatus.search)
        {
            //If the move instance is set, then get a new point to move to RANDOM.
            if (changePos)
            {
                if (strangerMov_.changeTargetPosition())
                {
                    atPos = false;
                    changePos = false;
                    //Random movement, so standard speed in movement and animations, and IK.
                    setSpeedState(strangerSpeed.walk.ToString(), 0.5f, false, false);
                }
            }

            //Check if currently facing player, then test if sees the player.
            if (strangerEye_.getSeePlayer())
            {
                //Check if there is cover.
                if (strangerEye_.castRayCheck(player_.position))
                {
                    //Start the investigate stage.
                    stat = strangerStatus.alert;
                }
            }

        //Complete if the stranger is in the chase status.
        } else if (stat == strangerStatus.chase)
        {
            if (strangerMov_.changeTargetPosition(player_.position))
            {
                atPos = false;
                changePos = false;
                //Set speed to sprint speed and set animation / ik kinetics to player.
                setSpeedState(strangerSpeed.sprint.ToString(), 2, true, true);
            }

        //Complete when the stranger sees the player, but isn't ready to move towards it yet.
        } else if (stat == strangerStatus.alert)
        {
            //Walk is standard, and make the stranger look at player.
            setSpeedState(strangerSpeed.walk.ToString(), 0.5f, false, true);

            //Ensure stranger can still see the player.
            if(strangerEye_.getSeePlayer() && strangerEye_.castRayCheck(player_.position))
            {
                //Get stranger to turn to player and stay in place.
                strangerMov_.changeTargetPosition(transform.position);
                strangerMov_.rotateToTarget(player_.position);
            }
        }

        /*FOR ALL STATUSES*/

        //If not at position and not changing position, then move to new position set in Current target of movement script.
        if (!atPos && !changePos)
        {
            strangerMov_.moveTo();
            strangerAnim_.setMoveState(true);
        }

        //Check if Stranger is close to the target position.
        if (strangerMov_.atPosition() && !(stat == strangerStatus.alert))
        {
            atPos = true;
            //Get a random number for the timer.
            //This is called once, before atPos is finally changed.
            if (!isIdle)
            {
                isIdle = true;
                idleTime = getRandomNumber();
                idleTimer = idleTime;
            }

            strangerAnim_.setMoveState(false);
            idleTimer = iterateTimer(idleTimer);
            if (idleTimer <= 0)
            {
                changePos = true;
                isIdle = false;
            }
        }
    }

    /*A function which sets animation speed, type and movement speed, type.*/
    void setSpeedState(string st, float sp, bool b, bool b2)
    {
        //Set movement speed.
        strangerMov_.setSpeed(st);
        //Set animation speed.
        strangerAnim_.setSpeed(sp);
        //Set IK state.
        strangerAnim_.setIK(b);
        //Set stare state.
        strangerAnim_.setIsLooking(b2);
    }

    /*Get a random number */
    float getRandomNumber()
    {
        return Random.Range(0.5f, 5);
    }

    /*Where timers are used, work through this timer. */
    float iterateTimer(float t)
    {
        //If timer is greater than 0, then, iterate down until complete.
        if (t > 0)
        {
            t -= Time.deltaTime;
        }

        return t;
    }
}
