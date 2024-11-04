using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //[SerializeField]
    //string[] storyPrompts;

    //This should be the same size as the story prompts.
    //[SerializeField]
    //int[] exitTime;

    List<eventClass> eventMessages;

    List<eventClass> storyEvents;


    [Header("Timers")]
    //The timers that deal with when an event can occur.
    [SerializeField]
    float eventTime;
    float eventTimer;
    //A timer to pauses the usual event generation.
    float eventPauseTimer;
    //A timer to delay spawning of haunter until 
    float spawnTime = 3;
    float spawnTimer = 0;
    [SerializeField]
    float eventSpeed;

    float storyTime;
    float storyTimer;



    private float randomMultiplier;

    bool isStarting;
    bool isEnding = false;
    bool inHideEvent;

    //A number to say at what point in the story you are at.
    [SerializeField]
    int eventToken = 0;
    bool eventChange;

    WorldStateManager objectStateManager;
    MenuManager promptManager;

    [SerializeField]
    GameObject haunter;


    // Start is called before the first frame update
    void Start()
    {
        //Setup the world to starting time.
        eventTimer = eventTime;
        randomMultiplier = 1f;
        haunter.SetActive(false);
        objectStateManager = GetComponent<WorldStateManager>();
        promptManager = GameObject.FindGameObjectWithTag("Player").GetComponent<MenuManager>();

        eventChange = true;

        //Set up starting time [Obsolete]
        /*if (isStarting)
        {
            startTimer = startTime;
            readTime = startTime / 5;
            readTimer = 0;
        } else
        {
            startTimer = 0;
        }*/

        //Populate the event list.
        //Debug.Log(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        TextAsset ass = Resources.Load<TextAsset>("Dialogue_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        string eventResources = ass.text;
        string[] sections = eventResources.Split('\n');
        storyEvents = new List<eventClass>();
        int storyBeat = -1;

        for(int i = 0; i < sections.Length; i++)
        {
            string[] line = sections[i].Split(';');

            //Check if story beat is the same as the line before.
            if (storyBeat != int.Parse(line[0]))
            {
                //If not the same story beat, then add message to new beat.
                storyBeat = int.Parse(line[0]);
                storyEvents.Add(new eventClass(int.Parse(line[0]), float.Parse(line[2]), 0));
                storyEvents[storyEvents.Count - 1].addMessage(line[1]);
            } else
            {
                //Find the last story event made and add the next message.
                storyEvents[storyEvents.Count - 1].addMessage(line[1]);
            }
        }

        ass = Resources.Load<TextAsset>("OptionalDialogue_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        eventResources = ass.text;
        sections = eventResources.Split('\n');
        eventMessages = new List<eventClass>();
        storyBeat = -1;

        for (int i = 0; i < sections.Length; i++)
        {
            string[] line = sections[i].Split(';');

            //Check if story beat is the same as the line before.
            if (storyBeat != int.Parse(line[0]))
            {
                //If not the same story beat, then add message to new beat.
                storyBeat = int.Parse(line[0]);
                eventMessages.Add(new eventClass(int.Parse(line[0]), 0, 0));
                eventMessages[eventMessages.Count - 1].addMessage(line[1]);
            }
            else
            {
                //Find the last story event made and add the next message.
                eventMessages[eventMessages.Count - 1].addMessage(line[1]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Run a given event if event changes.
        if(eventToken < storyEvents.Count)
        {
            if (eventChange)
            {
                //Ensure only this event runs, and only runs once.
                eventChange = false;

                //Run a prompt if there is a prompt to this event.
                if (!string.Equals(storyEvents[eventToken].getMessage(), ""))
                {
                    promptManager.updateText("MessagePrompt", storyEvents[eventToken].getMessage());
                }

                //See if this uses exitTime. If it does, then run the story timer.
                storyTime = storyEvents[eventToken].getEventTime();
                storyTimer = 0;
            }


            //If the story prompt has exit time, then run the timer.
            if (storyTimer < storyTime)
            {
                storyTimer += Time.deltaTime;
            }
            else
            {
                //Ensure this only works in exit time mode.
                if (storyTime > 0)
                {
                    eventToken++;
                    eventChange = true;
                }
            }
        }

        //This will created unique points for the dialogue.

        //Check if player is far enough away from gameManager (Ending time.)
        if (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, this.transform.position) > 10)
        {
            //Start affecting the load screen.
            promptManager.setToMenuGroup("LoadingBlack");
            //Set the loading black image to an alph depending on distance to end of distance.
            float alpha = Mathf.InverseLerp(10, 15, Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, this.transform.position));

            GameObject.FindGameObjectWithTag("Player").GetComponent<MenuManager>().adjustLoadValue(alpha);

            if (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, this.transform.position) > 14 && !isEnding)
            {
                isEnding = true;

                //Go to the prospective eventToken. For the ending, this is last.
                eventToken = storyEvents.Count - 1;
                eventChange = true;
                //promptManager.updateText("MessagePrompt", storyPrompts[7]);
            }
        }

        //If the story is over (basically if you run out of prompts), do this.
        if(eventToken >= storyEvents.Count)
        {
            //Load the menu.
            //Delete the old saves.
            WorldStateManager stateMan_ = GetComponent<WorldStateManager>();

            //Delete the saves.
            stateMan_.removeAllSaves();

            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }


        //Ensure generic events can occur.
        //If events aren't paused, run the usual generic event system.
        if (eventPauseTimer <= 0)
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
        }
        else
        {
            eventPauseTimer -= Time.deltaTime / eventSpeed;
            //stepSounds();

            if (spawnTimer < spawnTime)
            {
                spawnTimer += Time.deltaTime;
            }
            else
            {
                if (inHideEvent)
                {
                    haunter.SetActive(true);
                }
            }

            if (inHideEvent && eventPauseTimer <= 0)
            {
                eventPauseTimer = 1;
                inHideEvent = false;
                endHideEvent();
            }
        }
    }

    //A function that creates an ending based on getting killed by a haunt.
    public void endByKill()
    {
        //promptManager.updateText("MessagePrompt", eventMessages[0].getMessage());
        eventToken = storyEvents.Count - 1;
        //Change story override.
        storyEvents[storyEvents.Count - 1].setMessageOverride(1);
        eventChange = true;
        //eventChange = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<MenuManager>().adjustLoadValue(1);
    }

    public void runNextEvent(int eventInd, EventScript.eventType t, Vector2 dep, int over)
    {
        if(t == EventScript.eventType.storyEvent)
        {
            //For different dialogues based on when event was done.
            //If below the needed event, then say this event.
            //If above this event, then say the next event coming.
            eventToken = eventInd;
            eventChange = true;
        } else
        {
            //Ensure the story events are paused as this event is being made.
            eventChange = false;
            eventToken--;
            storyTimer = 0;

            //Optional events can run in 2, so check where one is at in the story.
            storyEvents[(int)dep.x].setMessageOverride((int)dep.y);

            //Change dialogue depending if story event has already passed.
            if(eventToken >= dep.x - 1)
            {
                eventMessages[eventInd].setMessageOverride(over);
            }


            //Say the message of the prompt.
            if (!string.Equals(eventMessages[eventInd].getMessage(), ""))
            {
                promptManager.updateText("MessagePrompt", eventMessages[eventInd].getMessage());
            }

            //Ensure all forced interaction based events (that is, first interactions or interactions that only work when not making an interaction.
            bool changed = false;

            for(int i = 1; i < 4; i++)
            {
                if(eventToken <= i - 1 && !changed)
                {
                    storyEvents[i].setMessageOverride(1);
                    changed = true;
                }
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
            if(!string.Equals(objectStateManager.getInteractionEnergy(i).gameObject.name, "DrawerDrsser1.001"))
            {
                arr.Add(objectStateManager.getInteractionEnergy(i));
            }
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
        promptManager.updateText("MessagePrompt", eventMessages[1].getMessage());

        eventPauseTimer = Random.Range(3, 15);

        //this.GetComponent<InteractionControlClass>().playInbuiltAudio(0, true);

        inHideEvent = true;
    }

    private void endHideEvent()
    {
        promptManager.updateText("MessagePrompt", eventMessages[2].getMessage());

        haunter.SetActive(false);
        spawnTimer = spawnTime;
        //this.GetComponent<InteractionControlClass>().playInbuiltAudio(0, false);
    }

    /*private void stepSounds()
    {
        float rand = Random.Range(0, 1);

        if(stepTimer > 0)
        {
            stepTimer -= Time.deltaTime;
        } else
        {
            stepTime = Random.Range(1, 2);
            stepTimer = stepTime;
            this.GetComponent<InteractionControlClass>().playInteractionAudio(Random.Range(0, this.GetComponent<InteractionControlClass>().getAudioLength() - 1));
        }
    }*/

    public void setStart(bool b)
    {
        isStarting = b;
    }

    public int getEventToken()
    {
        return eventToken;
    }

    public void setEventToken(int b)
    {
        eventToken = b;
    }
}

public class eventClass
{
    int eventID;
    List<string> messages;
    float eventTime;
    int messageOverride;

    public eventClass(int i, float f, int m)
    {
        eventID = i;
        eventTime = f;
        messageOverride = m;
        messages = new List<string>();
    }

    public void addMessage(string s)
    {
        messages.Add(s);
    }

    public int getID()
    {
        return eventID;
    }

    public float getEventTime()
    {
        return eventTime;
    }

    public string getMessage()
    {
        if(messageOverride < messages.Count && messageOverride >= 0)
        {
            return messages[messageOverride];
        }

        //Say default dialogue.
        return messages[0];
    }

    public int getMessageOverride()
    {
        return messageOverride;
    }

    public void setMessageOverride(int i)
    {
        messageOverride = i;
    }
}
