using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    /*Scripts*/
    public bool isDay;

    [SerializeField]
    SystemManager[] systemManagers;

    AudioManager audMan_;
    OptionsManager optMan_;
    DayNightManager dayMan_;
    EventManager eventMan_;

    private float loadingTime = 0;
    [SerializeField]
    float loadTime;

    private void Awake()
    {
        dayMan_ = GetComponent<DayNightManager>();
        //Figure out if this is a main menu scene. do different things.
        if(string.Equals(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, "MainMenu"))
        {
            optMan_ = GetComponent<OptionsManager>();

            //Start by getting info from player preferences.
            optMan_.setUpControls();
            //Set up the options.
            if (optMan_.isControlsEmpty())
            {
                optMan_.saveControls(true);
            }

            //Assign interactables and energy objects with unique ids.
            //setInteractables();

            //Set up audio to happen in the awake section.
            audMan_ = GetComponent<AudioManager>();

            audMan_.setUpManager();

            for (int i = 0; i < audMan_.getAudioSourceLength(); i++)
            {
                audMan_.setAudioVolume(i, optMan_.getMasterVol());
            }

            dayMan_.setIntensity(-0.78f);
            //Forces speed to 0.
            dayMan_.setSpeed(0);

            //Ensure the cursor is visible and not locked.
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

        } else
        {
            loadingTime = loadTime;

            optMan_ = GetComponent<OptionsManager>();

            //Start by getting info from player preferences.
            optMan_.setUpControls();

            //Set up the options.
            if (optMan_.isControlsEmpty())
            {
                optMan_.saveControls(true);
            }

            //Assign interactables and energy objects with unique ids.
            setInteractables();

            //Set up audio to happen in the awake section.
            audMan_ = GetComponent<AudioManager>();

            audMan_.setUpManager();

            for (int i = 0; i < audMan_.getAudioSourceLength(); i++)
            {
                audMan_.setAudioVolume(i, 0);
            }

            if (systemManagers.Length > 0)
            {
                for (int i = 0; i < systemManagers.Length; i++)
                {
                    systemManagers[i].setManager();
                }
            }
            else
            {
                Debug.LogWarning("Game Manager does not have any systems to manage.");
            }
            eventMan_ = GetComponent<EventManager>();

            //Initiate the first state of the world.
            GetComponent<WorldStateManager>().loadWorld();

            //Set loading level to the black start.
            GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<MenuManager>().setToMenuGroup("LoadBlack");

            //Check if this is the initial boot.
            isInitialBoot();

            //Set up the groups of this system.
            dayMan_.setIntensity(2);

            //Run a coroutine to load in the world without updates.
            StartCoroutine(loadingSceneUI());
        }

        //Run the achievements code.
        if (GetComponent<AchievementManager>())
        {
            GetComponent<AchievementManager>().setAchievements();
        }
    }

    //Set interactables and energy objects with a unique ID based on their position in the world.
    public void setInteractables()
    {
        InteractionClass[] interactables = GameObject.FindObjectsByType<InteractionClass>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        EnergyObjectClass[] energies = GameObject.FindObjectsByType<EnergyObjectClass>(FindObjectsSortMode.None);
        FPSController[] playerEntity = GameObject.FindObjectsByType<FPSController>(FindObjectsSortMode.None);
        EventScript[] eventObjects = GameObject.FindObjectsByType<EventScript>(FindObjectsSortMode.None);
        MenuManager[] menus = GameObject.FindObjectsByType<MenuManager>(FindObjectsSortMode.None);
        SystemManager[] systemMans = GameObject.FindObjectsByType<SystemManager>(FindObjectsSortMode.None);

        for(int i = 0; i < menus.Length; i++)
        {
            float id = menus[i].transform.position.sqrMagnitude + (menus[i].transform.rotation.eulerAngles.sqrMagnitude);
            menus[i].setMenuID(id);
        }

        //Assign ids to player entities.
        for(int i = 0; i < playerEntity.Length; i++)
        {
            float id = playerEntity[i].transform.position.sqrMagnitude + (playerEntity[i].transform.rotation.eulerAngles.sqrMagnitude);
            playerEntity[i].setPlayerID(id);
        }

        //Assign id based on start up before any moves have been made. This should be solid throughout.
        for(int i = 0; i < interactables.Length; i++)
        {
            int nameNum = 0;
            for(int v = 0; v < interactables[i].name.Length; v++)
            {
                nameNum += (int)interactables[i].name[v];
            }
            float id = interactables[i].transform.position.sqrMagnitude + (interactables[i].transform.rotation.eulerAngles.sqrMagnitude) + (interactables[i].transform.localScale.sqrMagnitude) + nameNum;
            interactables[i].setObjectID(id);

            //Debug.Log(interactables[i] + ": " + interactables[i].getObjectID());
        }

        for(int i = 0; i < systemMans.Length; i++)
        {
            int nameNum = 0;
            for (int v = 0; v < systemMans[i].gameObject.name.Length; v++)
            {
                nameNum += (systemMans[i].gameObject.name[v]);

                /*if(string.Equals(systemManagers[i].gameObject.name, "EnergySystem"))
                {
                    Debug.Log("nameNum: " + systemManagers[i].gameObject.name[v] + " = " + (int)systemManagers[i].gameObject.name[v] + " += " + nameNum);
                }*/
            }

            float id = (systemMans[i].transform.position.sqrMagnitude) + (systemMans[i].transform.rotation.eulerAngles.sqrMagnitude) + (systemMans[i].transform.localScale.sqrMagnitude) + nameNum;
            systemMans[i].setSystemId(id);
        }

        //Find duplicates.
        for(int i = 0; i < interactables.Length; i++)
        {
            float value = interactables[i].getObjectID();
            int count = 0;
            string name = "";

            for(int v = 0; v < interactables.Length; v++)
            {
                if(interactables[v].getObjectID() == value && interactables[v] != interactables[i])
                {
                    count++;
                    name = interactables[v].name;
                }
            }

            if(count > 1)
            {
                Debug.LogWarning("Repeat ID detected - Item: " + interactables[i].gameObject.name + " | " + interactables[i].getObjectID() + " | " + name);
            } 
        }

        //Assign id based on start up before any moves have been made. This should be solid throughout.
        for (int i = 0; i < energies.Length; i++)
        {
            float id = energies[i].transform.position.sqrMagnitude + (energies[i].transform.rotation.eulerAngles.sqrMagnitude);
            energies[i].setObjectID(id);

            //Debug.Log(interactables[i] + ": " + interactables[i].getObjectID());
        }

        //Find duplicates.
        for (int i = 0; i < energies.Length; i++)
        {
            float value = energies[i].getObjectID();
            int count = 0;

            for (int v = 0; v < energies.Length; v++)
            {
                if (energies[v].getObjectID() == value)
                {
                    count++;
                }
            }

            if (count > 1)
            {
                Debug.LogWarning("Repeat ID detected - Energy: " + energies[i].gameObject.name + " | " + energies[i].getObjectID());
            }
        }

        //Assign id based on start up before any moves have been made. This should be solid throughout.
        for (int i = 0; i < eventObjects.Length; i++)
        {
            float id = eventObjects[i].transform.position.sqrMagnitude + (eventObjects[i].transform.rotation.eulerAngles.sqrMagnitude);
            eventObjects[i].setEventID(id);

            //Debug.Log(interactables[i] + ": " + interactables[i].getObjectID());
        }

        //Find duplicates.
        for (int i = 0; i < eventObjects.Length; i++)
        {
            float value = eventObjects[i].getEventID();
            int count = 0;

            for (int v = 0; v < eventObjects.Length; v++)
            {
                if (eventObjects[v].getEventID() == value)
                {
                    count++;
                }
            }

            if (count > 1)
            {
                Debug.LogWarning("Repeat ID detected - Event: " + eventObjects[i].gameObject.name + " | " + eventObjects[i].getEventID());
            }
        }

        //Find duplicates.
        for (int i = 0; i < systemMans.Length; i++)
        {
            float value = systemMans[i].getSystemId();
            int count = 0;

            for (int v = 0; v < systemMans.Length; v++)
            {
                if (systemMans[v].getSystemId() == value)
                {
                    count++;
                }
            }

            if (count > 1)
            {
                Debug.LogWarning("Repeat ID detected - Event: " + systemMans[i].gameObject.name + " | " + systemMans[i].getSystemId());
            }
        }

    }

    IEnumerator loadingSceneUI()
    {
        while(loadingTime > 0)
        {
            //Go through the load time.
            if (loadingTime > 0)
            {
                loadingTime -= Time.deltaTime;

                if (loadingTime < 0.5)
                {
                    //Change the loading panel to current load time.
                    float newAlpha = (loadingTime / 0.5f);

                    //Debug.Log("Loading" + newAlpha);
                    GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<MenuManager>().adjustLoadValue(newAlpha);
                }
            }

            yield return null;
        }

        updateAudio();

        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<MenuManager>().adjustLoadValue(0);

        //End this current coroutine. You don't need it to run anymore.
        StopCoroutine(loadingSceneUI());
    }

    //A function to return if loading has been completed in this scene's gamemanager.
    public bool isDoneLoading()
    {
        return loadingTime <= 0;
    }

    public void updateAudio()
    {
        for (int i = 0; i < audMan_.getAudioSourceLength(); i++)
        {
            audMan_.setAudioVolume(i, optMan_.getMasterVol());
        }
    }


    public bool getDay()
    {
        return isDay;
    }

    public void setDay(bool b)
    {
        isDay = b;
        if (!eventMan_)
        {
            eventMan_ = GetComponent<EventManager>();
        }

        if (eventMan_)
        {
            if (isDay)
            {
                eventMan_.setEventMultiplier(0.5f);
            } else
            {
                eventMan_.setEventMultiplier(0.8f);
            }
        }
    }

    //If this function is run, set events to initial boot.
    public void isInitialBoot()
    {
        //Debug.Log(GetComponent<WorldStateManager>().getSaveCount());

        if(GetComponent<WorldStateManager>().getSaveCount() == 1)
        {
            if (eventMan_)
            {
                eventMan_.setStart(true);
            }
        } else
        {
            if (eventMan_)
            {
                eventMan_.setStart(false);
            }
        }
    }
}
