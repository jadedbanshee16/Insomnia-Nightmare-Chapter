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
            manager_.runNextEvent(eventNum, type, new Vector2(eventDependency, overrideDialogue), overrideEvent);
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
}
