using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTriggerScript : MonoBehaviour
{
    [SerializeField]
    private float time;
    private float timer = 0;

    [SerializeField]
    private bool oneUse;
    private bool isUsed = false;

    private void tripTrigger()
    {
        //Go through and make interactions if needed.
        if (GetComponent<AchievementScript>())
        {
            GetComponent<AchievementScript>().triggerAchievement();
        }

        if (GetComponent<EventScript>())
        {
            GetComponent<EventScript>().TriggerEvent();
        }
    }

    //Runs if a collider is set on this.
    //This means it doesn't need an event or interaction.
    private void OnTriggerStay(Collider other)
    {
        if (oneUse)
        {
            if (!isUsed)
            {
                if (other.CompareTag("Player"))
                {
                    if (timer < time)
                    {
                        timer += Time.deltaTime;
                    }
                    else
                    {
                        isUsed = true;
                        tripTrigger();
                    }
                }
            }
        } else
        {
            if (other.CompareTag("Player"))
            {
                if (timer < time)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    tripTrigger();
                    timer = 0;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Reset the timer;
            timer = 0;
        }
    }
}
