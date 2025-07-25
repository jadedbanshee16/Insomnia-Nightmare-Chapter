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

    AchievementScript[] endAchievements;

    MessageScript messageManager;


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

    [SerializeField]
    float endingDist = 14;


    [SerializeField]
    private float randomMultiplier;

    bool isStarting;
    bool isEnding = false;
    bool endingArea = false;
    bool isEventPlayed = false;
    bool inHideEvent;

    //public bool first;

    WorldStateManager objectStateManager;

    [SerializeField]
    GameObject haunter;

    //Used to stop a haunting from occuring at given times, like cutscenes.
    private bool canHaunt;

    [SerializeField]
    GameObject[] excludedHauntitems;

    private float hauntAudioTimer = 1;
    private bool isEndingHaunt;


    // Start is called before the first frame update
    void Start()
    {
        //Setup the world to starting time.
        eventTimer = eventTime;
        //randomMultiplier = 1f;
        haunter.SetActive(false);
        canHaunt = true;
        objectStateManager = GetComponent<WorldStateManager>();

        endAchievements = GetComponentsInChildren<AchievementScript>();
        isEndingHaunt = false;
        messageManager = GetComponent<MessageScript>();

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
    }

    // Update is called once per frame
    void Update()
    {


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
                //Debug.Log("Common?");
                if (canHaunt)
                {
                    haunter.GetComponent<HauntScript>().playSound(true);
                }
            }
            else
            {
                if (inHideEvent && canHaunt)
                {
                    haunter.SetActive(true);

                }
            }

            if (inHideEvent && eventPauseTimer <= 0)
            {
                eventPauseTimer = 0;
                inHideEvent = false;
                endHideEvent();
            }
        }

        endLevelEvent();
        endLevel();


        //Run an achievement if ending.
        if (isEnding)
        {
            //Find all possible ending achievements.
            if (!isEventPlayed)
            {

                //First achievement is end achievement.
                endAchievements[0].triggerAchievement();

                //Run if reading the note in the first level.
                if(!findEventPlayed(0, EventScript.eventType.optionalEvent, true) && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Mind Chapter 1")
                {
                    //Debug.Log("Played");
                    endAchievements[1].triggerAchievement();
                }

                //Run if you haven't misplaced an item in the second level.
                if (!findEventPlayed(4, EventScript.eventType.optionalEvent, true) && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Mind Chapter 2")
                {
                    //Debug.Log("Played");
                    endAchievements[3].triggerAchievement();
                }

                //Trigger the ending change. The more you finish, the more you see in the world.
                //This should be obsolete
                //endAchievements[3].triggerAchievement();

                isEventPlayed = true;
            }
        }

        if (isEndingHaunt)
        {
            if (hauntAudioTimer > 0)
            {
                hauntAudioTimer -= Time.deltaTime;
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<InteractionControlClass>().adjustVolume(0.4f * (hauntAudioTimer / 1));
            }
            else
            {
                haunter.GetComponent<HauntScript>().playSound(false);
                isEndingHaunt = false;
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<InteractionControlClass>().adjustVolume(0.4f);
                hauntAudioTimer = 1;
            }
        }
    }

    //A function that creates an ending based on getting killed by a haunt.
    public void endByKill()
    {
        //promptManager.updateText("MessagePrompt", eventMessages[0].getMessage());

        //Make sure everything else in this message queue is cleared.
        messageManager.clearMessageQueue();

        //Add the prompt to the message script.
        messageManager.runEventMessage(messageManager.getDeathMessageID(), EventScript.eventType.storyEvent);

        //Change story override.
        messageManager.setMessageOverride(messageManager.getEndingMessageID(), 1);

        //eventChange = true;
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<MenuManager>().adjustLoadValue(1);

        //Adjust the level manager script.
        LevelManager levelMan_ = GetComponent<LevelManager>();

        canHaunt = false;

        //Set the end achievement for the 3rd achievement object.
        endAchievements[2].triggerAchievement();
    }

    public void endLevel()
    {
        //This should run the end of the meesage
        if (messageManager.isMessagesFinished())
        {
            //Load the menu.
            //Delete the old saves.
            WorldStateManager stateMan_ = GetComponent<WorldStateManager>();

            //Delete the saves.
            stateMan_.removeAllSaves();

            string ind = "Mind Chapter";

            //Find the LevelManager and use it to load the specific level
            if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelManager>())
            {
                ind = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelManager>().retrieveLevel(ind);
            }

            //This should 
            if (ind.Contains("1"))
            {
                ind = "MainMenu";
            }
            UnityEngine.SceneManagement.SceneManager.LoadScene(ind);
        }
    }

    public void endLevelEvent()
    {
        //Check if player is far enough away from gameManager (Ending time.)
        if (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, this.transform.position) > endingDist - 5)
        {
            //Start affecting the load screen.
            messageManager.setLoadingOnPrompts();

            endingArea = true;

            //Set the loading black image to an alph depending on distance to end of distance.
            float alpha = Mathf.InverseLerp(endingDist - 5, endingDist, Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, this.transform.position));

            //Debug.Log("Alpha: " + alpha);

            //Set the alpha of the current selected point. (This should be the loading prompt.
            GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<MenuManager>().adjustLoadValue(alpha);

            if (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, this.transform.position) > endingDist && !isEnding)
            {
                isEnding = true;
                canHaunt = false;

                //Turn off haunt and make it so it can't comes back.
                if (!canHaunt && haunter.activeSelf)
                {
                    haunter.SetActive(false);
                    //haunter.GetComponent<HauntScript>().playSound(false);
                    isEndingHaunt = true;
                }

                //Ensure no other messages would continue playing at this level.
                messageManager.clearMessageQueue();
                //Run the event for ending.
                runNextEvent(messageManager.getEndingMessageID(), EventScript.eventType.storyEvent, Vector2.zero, 0);

                //Adjust the level manager script.
                LevelManager levelMan_ = GetComponent<LevelManager>();

                //Debug.Log("Should be once");
                //levelMan_.updateLevelFile("Mind Chapter", 1);
            }
        } else
        {
            //This will ensure that only if entering ending area that this will prompt.
            if (endingArea)
            {
                //Debug.Log("Worked");
                //If returning to the play field, then this will reset group to stylus and unlock the system.
                messageManager.resetLoadingOnPrompts();

                endingArea = false;
            }
        }
    }

    public void runNextEvent(int eventInd, EventScript.eventType t, Vector2 dep, int over)
    {
        //First, test if the ending is the end token. If so, complete the last event stuff and quit.
        if(t == EventScript.eventType.storyEvent)
        {
            if (eventInd == messageManager.getEndingMessageID())
            {
                //This should make the ending at the end of the game.
                isEnding = true;

                //Adjust the level manager script.
                LevelManager levelMan_ = GetComponent<LevelManager>();

                levelMan_.updateLevelFile("Mind Chapter", 1);

                GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<MenuManager>().adjustLoadValue(1);

                canHaunt = false;

                //Turn off haunt and make it so it can't comes back.
                if (!canHaunt && haunter.activeSelf)
                {
                    haunter.SetActive(false);
                    //haunter.GetComponent<HauntScript>().playSound(false);
                    isEndingHaunt = true;
                }

                AudioManager audMan_ = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioManager>();

                for (int i = 0; i < audMan_.getAudioSourceLength(); i++)
                {
                    audMan_.setAudioVolume(i, 0);
                }

                eventTimer = 100;
            }

            //Add the given information to the queue.
            messageManager.runEventMessage(eventInd, t);
        } else
        {
            messageManager.runEventMessage(eventInd, t);

            messageManager.setMessageOverride((int)dep.x, (int)dep.y);
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
                if (random < 10)
                {
                    throwEvent();
                }
                else if( random < 30)
                {
                    EnergyEvent();
                } else
                {
                    //Chance based on night time.
                    hideEvent();
                }

                //hideEvent();
            }
            else
            {
                //Chance based on day time.
                if (random < 40)
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

    //Search and return an event script.
    //Variables:
    // num - the number of the event, to find the event script.
    // t - to ensure we're looking for the correct type of event, so it doesn't find story achievements when looking for a number in the optionals.
    // truthState - if multiple eventScripts with the same number and time, determine if they all have to be played or if only one of them. True means only one of them needs to be played.
    //              This is to allow mutiple events scripts to cater to the same event, for instance if you press one switch it should trigger the event.
    public bool findEventPlayed(int num, EventScript.eventType t, bool truthState)
    {
        EventScript[] events = GameObject.FindObjectsByType<EventScript>(FindObjectsSortMode.None);

        bool isPlayed = false;

        for(int i = 0; i < events.Length; i++)
        {
            if(events[i].getEventNum() == num && events[i].getType() == t)
            {
                if (events[i].getIsPlayed())
                {
                    isPlayed = events[i].getIsPlayed();
                } else
                {
                    //This is to make false if not all events ran. This should only occur if truthState is false, meaning you need all events to occur to count event as played.
                    if (!truthState)
                    {
                        isPlayed = false;
                    }
                }
            }
        }

        return isPlayed;
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
                bool isExcluded = false;
                //Check to see if item is not in excluded list so it doesn't throw important items out of the world.
                for(int v = 0; v < excludedHauntitems.Length; v++)
                {
                    if(excludedHauntitems[v].GetComponent<HoldInteractionClass>() && string.Equals(objectStateManager.getInteractionObjects(i).gameObject.name, excludedHauntitems[v].name))
                    {
                        isExcluded = true;
                    }
                }

                if (!isExcluded)
                {
                    arr.Add(objectStateManager.getInteractionObjects(i));
                }
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
            /*if(!string.Equals(objectStateManager.getInteractionEnergy(i).gameObject.name, "DrawerDrsser1.001"))
            {
                arr.Add(objectStateManager.getInteractionEnergy(i));
            }*/

            bool isExcluded = false;
            //Check to see if item is not in excluded list so it doesn't throw important items out of the world.
            for (int v = 0; v < excludedHauntitems.Length; v++)
            {
                if (excludedHauntitems[v].GetComponent<EnergyInteractionClass>() && string.Equals(objectStateManager.getInteractionEnergy(i).gameObject.name, excludedHauntitems[v].name))
                {
                    isExcluded = true;
                }
            }

            if (!isExcluded)
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

        messageManager.runEventMessage(1, EventScript.eventType.optionalEvent);

        eventPauseTimer = Random.Range(3, 15);

        //this.GetComponent<InteractionControlClass>().playInbuiltAudio(0, true);

        inHideEvent = true;
    }

    private void endHideEvent()
    {
        messageManager.runEventMessage(2, EventScript.eventType.optionalEvent);

        haunter.SetActive(false);
        //haunter.GetComponent<HauntScript>().playSound(false);
        isEndingHaunt = true;
        spawnTimer = 0;
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

    public int getExcludedCount()
    {
        return excludedHauntitems.Length;
    }

    public GameObject getExcludedItem(int i)
    {
        return excludedHauntitems[i];
    }

    public bool getCanHaunt()
    {
        return canHaunt;
    }
}
