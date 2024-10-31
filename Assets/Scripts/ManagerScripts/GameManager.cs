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

        } else
        {
            loadingTime = loadTime;

            //Set loading level to the black start.
            GameObject.FindGameObjectWithTag("Player").GetComponent<MenuManager>().setToMenuGroup("LoadBlack");

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

            //Check if this is the initial boot.
            isInitialBoot();

            //Set up the groups of this system.
            dayMan_.setIntensity(2);

            //Run a coroutine to load in the world without updates.
            StartCoroutine(loadingSceneUI());
        }


    }

    //Set interactables and energy objects with a unique ID based on their position in the world.
    public void setInteractables()
    {
        InteractionClass[] interactables = GameObject.FindObjectsByType<InteractionClass>(FindObjectsSortMode.None);
        EnergyObjectClass[] energies = GameObject.FindObjectsByType<EnergyObjectClass>(FindObjectsSortMode.None);
        FPSController[] playerEntity = GameObject.FindObjectsByType<FPSController>(FindObjectsSortMode.None);

        //Assign ids to player entities.
        for(int i = 0; i < playerEntity.Length; i++)
        {
            float id = playerEntity[i].transform.position.sqrMagnitude + (playerEntity[i].transform.rotation.eulerAngles.sqrMagnitude);
            playerEntity[i].setPlayerID(id);
        }

        //Assign id based on start up before any moves have been made. This should be solid throughout.
        for(int i = 0; i < interactables.Length; i++)
        {
            float id = interactables[i].transform.position.sqrMagnitude + (interactables[i].transform.rotation.eulerAngles.sqrMagnitude);
            interactables[i].setObjectID(id);

            //Debug.Log(interactables[i] + ": " + interactables[i].getObjectID());
        }

        //Find duplicates.
        for(int i = 0; i < interactables.Length; i++)
        {
            float value = interactables[i].getObjectID();
            int count = 0;

            for(int v = 0; v < interactables.Length; v++)
            {
                if(interactables[v].getObjectID() == value && interactables[v] != interactables[i])
                {
                    count++;
                }
            }

            if(count > 1)
            {
                Debug.LogWarning("Repeat ID detected - Item: " + interactables[i].gameObject.name + " | " + interactables[i].getObjectID());
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
                    GameObject.FindGameObjectWithTag("Player").GetComponent<MenuManager>().adjustLoadValue(newAlpha);
                }
            }

            yield return null;
        }

        updateAudio();

        GameObject.FindGameObjectWithTag("Player").GetComponent<MenuManager>().adjustLoadValue(0);

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
            eventMan_.setStart(true);
        } else
        {
            eventMan_.setStart(false);
        }
    }
}
