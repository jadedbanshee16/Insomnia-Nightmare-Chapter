using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField]
    string[] prompts;

    [SerializeField]
    float eventTime;
    [SerializeField]
    float eventTimer;
    [SerializeField]
    float eventPauseTimer;
    bool inHideEvent;

    [SerializeField]
    float eventSpeed;

    private float randomMultiplier;

    bool isStarting;

    WorldStateManager objectStateManager;
    MenuManager promptManager;


    // Start is called before the first frame update
    void Start()
    {
        eventTimer = eventTime;
        randomMultiplier = 1f;

        objectStateManager = GetComponent<WorldStateManager>();
        promptManager = GameObject.FindGameObjectWithTag("Player").GetComponent<MenuManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(eventPauseTimer <= 0)
        {
            if (eventTimer > 0)
            {
                eventTimer -= Time.deltaTime;
            }
            else
            {
                eventTimer = eventTime;
                generateEvent();
            }
        } else
        {
            eventPauseTimer -= Time.deltaTime / eventSpeed;

            if(inHideEvent && eventPauseTimer <= 0)
            {
                eventPauseTimer = 1;
                inHideEvent = false;
                endHideEvent();
            }
        }
    }

    private void generateEvent()
    {
        //See if event occurs.
        float random = Random.Range(0, 1);

        //if it is greater than random multipler, then run an event.
        if (random < randomMultiplier)
        {
            //Run an event.
            //Depending on day or night, choose the more likely even.
            random = Random.Range(0, 100);
            //Debug.Log("Event...");
            if (randomMultiplier > 0.6f)
            {
                //Chance based on night time.
                if (random < 25)
                {
                    EnergyEvent();
                }
                else if( random < 60)
                {
                    throwEvent();
                } else
                {
                    //Chance based on night time.
                    hideEvent();
                }
            }
            else
            {
                //Chance based on day time.
                if (random < 50)
                {
                    EnergyEvent();
                }
                else
                {
                    throwEvent();
                }
            }
        }
    }

    public void setEventMultiplier(float ran)
    {
        randomMultiplier = ran;
    }

    //A function that finds a holdable object, checks if it is connected to anything,
    //And through if it is not.
    private void throwEvent()
    {
        //Find a throwable items.
        List<HoldInteractionClass> arr = new List<HoldInteractionClass>();

        for(int i = 0; i < objectStateManager.getObjectAmount(); i++)
        {
            //If a holdable item and not currently being held.
            if((objectStateManager.getInteractionObjects(i).isInteractionType(InteractionClass.interactionType.player) ||
               (objectStateManager.getInteractionObjects(i).isInteractionType(InteractionClass.interactionType.playerHold))) &&
               !objectStateManager.getInteractionObjects(i).isHeld)
            {
                arr.Add(objectStateManager.getInteractionObjects(i));
            }
        }

        //Now, randomly choose 1 and choose a random location to through it.
        int rand = Random.Range(0, arr.Count);

        Vector3 randDirection = Random.insideUnitSphere * 4;

        //Check if door.
        if (arr[rand].GetComponent<HingeJoint>())
        {
            //Get random angle and set that angle.
            //All door objects have an anchor, I think.
            randDirection.y = arr[rand].getAnchor().position.y;
            arr[rand].GetComponent<InteractionControlClass>().setAngle(randDirection, arr[rand].getAnchor());
        } else
        {
            arr[rand].GetComponent<Rigidbody>().AddForce(randDirection * 60);
        }
    }

    private void EnergyEvent()
    {
        //Find a throwable items.
        List<EnergyInteractionClass> arr = new List<EnergyInteractionClass>();

        for (int i = 0; i < objectStateManager.getEnergyAmount(); i++)
        {
            arr.Add(objectStateManager.getInteractionEnergy(i));
        }

        //Now go and try to turn off or on any of these points.
        int rand = Random.Range(0, arr.Count);

        arr[rand].Interact();
    }

    private void hideEvent()
    {
        //Debug.Log("Worked?");
        //Set the given prompt.
        //Also, make events stop for a second.
        promptManager.updateText("MessagePrompt", prompts[0]);

        eventPauseTimer = Random.Range(3, 15);

        inHideEvent = true;
    }

    private void endHideEvent()
    {
        promptManager.updateText("MessagePrompt", prompts[1]);
    }
}
