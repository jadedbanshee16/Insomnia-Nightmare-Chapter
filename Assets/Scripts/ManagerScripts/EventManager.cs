using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField]
    string[] prompts;

    [SerializeField]
    float eventTime;
    float eventTimer;
    float eventPauseTimer;
    bool inHideEvent;

    [SerializeField]
    float startTime;
    public float startTimer;
    float readTime;
    public float readTimer;

    [SerializeField]
    float eventSpeed;

    private float randomMultiplier;

    bool isStarting;
    bool isEnding = false;

    float endTime = 5;
    float endTimer = 0;

    float spawnTime = 3;
    float spawnTimer = 0;

    WorldStateManager objectStateManager;
    MenuManager promptManager;

    [SerializeField]
    GameObject haunter;


    // Start is called before the first frame update
    void Start()
    {
        eventTimer = eventTime;
        randomMultiplier = 1f;

        haunter.SetActive(false);

        objectStateManager = GetComponent<WorldStateManager>();
        promptManager = GameObject.FindGameObjectWithTag("Player").GetComponent<MenuManager>();

        if (isStarting)
        {
            startTimer = startTime;
            readTime = startTime / 5;
            readTimer = 0;
        } else
        {
            startTimer = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //See if this is the first run. If so, then run the first prompts.
        if (startTimer > 0)
        {
            //Should be roughly seconds.
            startTimer -= Time.deltaTime;

            if(readTimer > 0)
            {
                readTimer -= Time.deltaTime;
            } else
            {
                readTimer = readTime;
                //Run the first event.
                if (startTimer > startTime * 0.8)
                {
                    promptManager.updateText("MessagePrompt", prompts[2]);
                }
                else if (startTimer > startTime * 0.6)
                {
                    promptManager.updateText("MessagePrompt", prompts[3]);
                } 
                else if(startTimer > startTime * 0.4)
                {
                    promptManager.updateText("MessagePrompt", prompts[4]);
                }
                else if (startTimer > startTime * 0.2)
                {
                    promptManager.updateText("MessagePrompt", prompts[5]);
                }
            }

            //Final message.
            if(startTimer <= 0)
            {
                promptManager.updateText("MessagePrompt", prompts[6]);
            }

        } else
        {
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

                if(spawnTimer < spawnTime)
                {
                    spawnTimer += Time.deltaTime;
                } else
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


        //Check if player is far enough away from gameManager.
        if(Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, this.transform.position) > 10)
        {
            //Start affecting the load screen.
            promptManager.setToMenuGroup("LoadingBlack");
            //Set the loading black image to an alph depending on distance to end of distance.
            float alpha = Mathf.InverseLerp(10, 15, Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, this.transform.position));
                
            GameObject.FindGameObjectWithTag("Player").GetComponent<MenuManager>().adjustLoadValue(alpha);

            if(Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, this.transform.position) > 14 && !isEnding)
            {
                isEnding = true;
                endTimer = endTime;
                promptManager.updateText("MessagePrompt", prompts[7]);
            }
        }

        if (isEnding)
        {
            if(endTimer > 0)
            {
                endTimer -= Time.deltaTime;
            } else
            {
                //Load the menu.
                //Delete the old saves.
                WorldStateManager stateMan_ = GetComponent<WorldStateManager>();

                //Delete the saves.
                stateMan_.removeAllSaves();

                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
    }

    public void endByKill()
    {
        promptManager.updateText("MessagePrompt", prompts[8]);
        isEnding = true;
        endTimer = endTime;
        GameObject.FindGameObjectWithTag("Player").GetComponent<MenuManager>().adjustLoadValue(1);
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
        promptManager.updateText("MessagePrompt", prompts[0]);

        eventPauseTimer = Random.Range(3, 15);

        //this.GetComponent<InteractionControlClass>().playInbuiltAudio(0, true);

        inHideEvent = true;
    }

    private void endHideEvent()
    {
        promptManager.updateText("MessagePrompt", prompts[1]);

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
}
