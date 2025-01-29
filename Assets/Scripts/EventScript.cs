using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScript : MonoBehaviour
{
    public enum eventType
    {
        storyEvent,
        optionalEvent
    }

    private float eventID;

    [SerializeField]
    eventType type;

    [SerializeField]
    int eventNum;

    [SerializeField]
    int eventDependency;
    [SerializeField]
    int overrideDialogue;
    [SerializeField]
    int overrideEvent;

    //This is for using TriggerEvent(bool).
    //When the boolean criteria comes in, it will be tested against this. If it is the same, then run event.
    [SerializeField]
    bool eventrigger;

    bool isPlayed;

    private EventManager manager_;
    // Start is called before the first frame update
    void Start()
    {
        manager_ = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EventManager>();
    }

    public void TriggerEvent()
    {
        if (!isPlayed)
        {
            if (manager_ && manager_.isActiveAndEnabled)
            {
                manager_.runNextEvent(eventNum, type, new Vector2(eventDependency, overrideDialogue), overrideEvent);
            }
            isPlayed = true;
        }
    }

    //A function that uses a boolean to decide whether the function runs or not.
    //This should only be used for events that can or cannot occur due to a specific boolean criteria.
    public void TriggerEvent(bool b)
    {
        //Debug.Log("When enter " + gameObject.name + ": " + b);
        //Unlike the usual triggerEvent, played only becomes true if the boolean is equal to the trigger state.
        //This is so the eventManager will not detect this as triggered if the trigger doesn't match.
        if (!isPlayed && b == eventrigger)
        {
            if (manager_ && manager_.isActiveAndEnabled)
            {
                //Debug.Log("Works 3: " + b);
                manager_.runNextEvent(eventNum, type, new Vector2(eventDependency, overrideDialogue), overrideEvent);

            }


           // Debug.Log("Works 4: " + b);
            isPlayed = true;
        }
    }

    public bool getIsPlayed()
    {
        return isPlayed;
    }

    public void setIsPlayed(bool b)
    {
        isPlayed = b;
    }

    public float getEventID()
    {
        return eventID;
    }

    public void setEventID(float i)
    {
        eventID = i;
    }

    public int getEventNum()
    {
        return eventNum;
    }

    public eventType getType()
    {
        return type;
    }
}
