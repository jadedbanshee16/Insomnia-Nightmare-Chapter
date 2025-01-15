using System.Collections;
using System.Collections.Generic;
using System.IO;
//using UnityEditor;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    [SerializeField]
    int currentCam;
    CameraManager camController;
    [SerializeField]
    List<GameObject> entities;
    [SerializeField]
    List<HoldInteractionClass> interactionablesItems;
    [SerializeField]
    List<LockObjectClass> interactionablesLocks;
    [SerializeField]
    List<EnergyInteractionClass> interactionablesEnergy;
    [SerializeField]
    List<GeneratorInteractionClass> interactionablesGenerator;
    [SerializeField]
    List<GridInteractionClass> interactionablesGrid;
    [SerializeField]
    List<ComputerObjectClass> energiesComputers;
    [SerializeField]
    List<CombinationManagerClass> energiesCombinations;
    [SerializeField]
    List<EventScript> eventsScripts;
    [SerializeField]
    List<MenuManager> menuScripts;

    private worldState _state;

    private int saveCount = 0;

    public void loadWorld()
    {        
        //Get path for the file manager.
        string p = string.Concat(Application.persistentDataPath, "/saveFiles/fileManager" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + ".txt");

        //Debug.Log(p);
        _state = new worldState();

        //If the manager file doesn't exist, create one.
        if (!System.IO.File.Exists(p))
        {
            createNewManager(p);
            generateInitialState();

            //Because this is the first boot in this level, then use the first boot file.

            /*p = string.Concat(Application.persistentDataPath, "/saveFiles/" + initialFileName + ".json");

            //Check if initial file is loaded.
            if (!System.IO.File.Exists(p))
            {
                p = Application.persistentDataPath + "/saveFiles/";
                //If all else fails, then save current state to initial state.
                saveIntoJson(-1, p, initialFileName);
            } else
            {
                p = Application.persistentDataPath + "/saveFiles/";
                //If all else fails, then save current state to initial state.
                saveIntoJson(0, p, "saveFile");
            }*/
        }

        //Find the file path of the newest save and load that data.
        System.IO.StreamReader re = new System.IO.StreamReader(p);

        string lastPath = "";
        string currentLine;
        while ((currentLine = re.ReadLine()) != null)
        {
            //This just reads all the lines.
            lastPath = currentLine;
            saveCount++;
        }

        re.Close();

        if (System.IO.File.Exists(lastPath))
        {
            string jsonString = System.IO.File.ReadAllText(lastPath);
            //Now use this path and turn to state.
            _state = JsonUtility.FromJson<worldState>(jsonString);
            //Debug.Log("Exists");
            loadWorldState();
        }
    }

    //Find and save the current data of connected objects to this work state.
    /*public void saveIntoJson(int fileIndex, string currentPath, string fileName)
    {
        //saveWorldState();

        //Debug.Log(_state.energy[0].id);

        //Get a json text of all datas.
        string text = JsonUtility.ToJson(_state, true);

        //Debug.Log(text);

        if(fileIndex < 0)
        {
            System.IO.File.WriteAllText(currentPath + fileName + ".json", text);
        } else
        {
            System.IO.File.WriteAllText(currentPath + fileName + fileIndex + ".json", text);
        }
    }*/

    public void createNewManager(string path)
    {
        //Create new directory to save files.
        System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/saveFiles");
        //Now save the new current file name to the list of names in manager.
        System.IO.StreamWriter wr = new System.IO.StreamWriter(path, false);

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        wr.WriteLine(Application.persistentDataPath + "/saveFiles/" + sceneName + "InitialState.json");

        wr.Close();
    }

    public void removeSave(int ind)
    {
        //Go through the given manager, remove the save file, then remove the index.
        string p = string.Concat(Application.persistentDataPath, "/saveFiles/fileManager" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + ".txt");

        System.IO.StreamReader re = new System.IO.StreamReader(p);

        List<string> saveList = new List<string>();

        string currentLine;

        int count = 0;

        while ((currentLine = re.ReadLine()) != null)
        {
            //This just reads all the lines.
            if(count != ind)
            {
                //Debug.Log("Save: " + currentLine);
                saveList.Add(currentLine);
            } else
            {
                //Debug.Log("Delete: " + currentLine);
                //Delete the file.
                File.Delete(currentLine);
            }

            count++;
        }

        re.Close();

        //Remove the file from the list.
        System.IO.StreamWriter wr = new System.IO.StreamWriter(p, false);

        for(int i = 0; i < saveList.Count; i++)
        {
            wr.WriteLine(saveList[i]);
        }

        wr.Close();
    }

    public void removeAllSaves()
    {
        //Go through the given manager, remove the save file, then remove the index.
        string p = string.Concat(Application.persistentDataPath, "/saveFiles/fileManager" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + ".txt");

        System.IO.StreamReader re = new System.IO.StreamReader(p);

        List<string> saveList = new List<string>();

        string currentLine;

        int count = 0;
        int ind = 0;

        while ((currentLine = re.ReadLine()) != null)
        {
            //This just reads all the lines.
            if (count != ind)
            {
                //Debug.Log("Delete: " + currentLine);
                //Delete the file.
                File.Delete(currentLine);
            }
            else
            {
                //Debug.Log("Save: " + currentLine);
                saveList.Add(currentLine);
            }

            count++;
        }

        re.Close();

        //Remove the file from the list.
        System.IO.StreamWriter wr = new System.IO.StreamWriter(p, false);

        for (int i = 0; i < saveList.Count; i++)
        {
            wr.WriteLine(saveList[i]);
        }

        wr.Close();
    }

    //A function to save the current world state.
    public void saveWorldState()
    {
        InteractionClass[] interactables = GameObject.FindObjectsByType<InteractionClass>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        LockObjectClass[] lockables = GameObject.FindObjectsByType<LockObjectClass>(FindObjectsSortMode.None);
        FPSController[] playerEntities = GameObject.FindObjectsByType<FPSController>(FindObjectsSortMode.None);
        EnergyObjectClass[] energyObjects = GameObject.FindObjectsByType<EnergyObjectClass>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        EventScript[] eventObjects = GameObject.FindObjectsByType<EventScript>(FindObjectsSortMode.None);
        MenuManager[] menuManagers = GameObject.FindObjectsByType<MenuManager>(FindObjectsSortMode.None);

        _state = new worldState();
        interactionablesItems = new List<HoldInteractionClass>();
        interactionablesLocks = new List<LockObjectClass>();
        interactionablesEnergy = new List<EnergyInteractionClass>();
        interactionablesGenerator = new List<GeneratorInteractionClass>();
        interactionablesGrid = new List<GridInteractionClass>();
        energiesComputers = new List<ComputerObjectClass>();
        entities = new List<GameObject>();
        eventsScripts = new List<EventScript>();
        menuScripts = new List<MenuManager>();

        for (int i = 0; i < playerEntities.Length; i++)
        {
            //Return the entity.
            _state.setEntity(playerEntities[i].gameObject.name, playerEntities[i].getPlayerID(), playerEntities[i].transform.position, playerEntities[i].transform.rotation);

            entities.Add(playerEntities[i].gameObject);
        }

        //Go through and save data of moveable objects.
        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].GetComponent<HoldInteractionClass>() &&
               (interactables[i].GetComponent<HoldInteractionClass>().isInteractionType(InteractionClass.interactionType.player) ||
                interactables[i].GetComponent<HoldInteractionClass>().isInteractionType(InteractionClass.interactionType.playerHold)))
            {
                if (interactables[i].GetComponent<HoldInteractionClass>().getCurrentHolder())
                {
                    _state.setItem(interactables[i].name, interactables[i].getObjectID(), interactables[i].transform.position, interactables[i].transform.rotation, interactables[i].GetComponent<HoldInteractionClass>().getCurrentHolder().GetComponent<InteractionClass>().getObjectID());
                }
                else
                {
                    _state.setItem(interactables[i].name, interactables[i].getObjectID(), interactables[i].transform.position, interactables[i].transform.rotation, -1);
                }

                interactionablesItems.Add(interactables[i].GetComponent<HoldInteractionClass>());
            }
        }

        //Go through and save data of lockable objects.
        for(int i = 0; i < lockables.Length; i++)
        {
            _state.setLocks(lockables[i].gameObject.name, lockables[i].getObjectID(), lockables[i].getInitialLock());
            interactionablesLocks.Add(lockables[i]);
        }

        //Go through energyInteraction classes.
        for(int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].GetComponent<EnergyInteractionClass>())
            {
                _state.setEnergyInteractions(interactables[i].gameObject.name, interactables[i].getObjectID(), interactables[i].GetComponent<EnergyInteractionClass>().getIsOn());
                interactionablesEnergy.Add(interactables[i].GetComponent<EnergyInteractionClass>());
            }
        }

        //This is for the generator class.
        for(int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].GetComponent<GeneratorInteractionClass>())
            {
                if (interactables[i].GetComponent<GeneratorInteractionClass>().getSystemManager())
                {
                    _state.setGeneratorData(interactables[i].gameObject.name, interactables[i].getObjectID(), interactables[i].GetComponent<GeneratorInteractionClass>().getIsOn(), interactables[i].GetComponent<GeneratorInteractionClass>().getIsFixed(), interactables[i].GetComponent<GeneratorInteractionClass>().getSystemManager().getSystemId());
                } else
                {
                    _state.setGeneratorData(interactables[i].gameObject.name, interactables[i].getObjectID(), interactables[i].GetComponent<GeneratorInteractionClass>().getIsOn(), interactables[i].GetComponent<GeneratorInteractionClass>().getIsFixed(), -1);
                }
                interactionablesGenerator.Add(interactables[i].GetComponent<GeneratorInteractionClass>());
            }
        }

        //For the grid switch classes.
        for(int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].GetComponent<GridInteractionClass>())
            {
                _state.setGridData(interactables[i].gameObject.name, interactables[i].getObjectID(), interactables[i].GetComponent<GridInteractionClass>().getIsOn());
                interactionablesGrid.Add(interactables[i].GetComponent<GridInteractionClass>());
            }
        }

        //Go through and find screen classes.
        for(int i = 0; i < energyObjects.Length; i++)
        {
            if (energyObjects[i].GetComponent<ComputerObjectClass>())
            {
                _state.setScreenData(energyObjects[i].gameObject.name, energyObjects[i].getObjectID(), energyObjects[i].GetComponent<ComputerObjectClass>().getCurrentCode(), energyObjects[i].GetComponent<ComputerObjectClass>().getIsOn());
                energiesComputers.Add(energyObjects[i].GetComponent<ComputerObjectClass>());
            }
        }

        //This goes through interactables to fill up the lists with the combination class.
        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].GetComponent<CombinationManagerClass>())
            {
                _state.setCombinationData(interactables[i].gameObject.name, interactables[i].getObjectID(), interactables[i].GetComponent<CombinationManagerClass>().getCurrentNumber());
                energiesCombinations.Add(interactables[i].GetComponent<CombinationManagerClass>());
            }
        }

        //Event scripts
        for (int i = 0; i < eventObjects.Length; i++)
        {
            //Return the entity.
            _state.setEventData(eventObjects[i].gameObject.name, eventObjects[i].getEventID(), eventObjects[i].getIsPlayed());

            eventsScripts.Add(eventObjects[i]);
        }

        for(int i = 0; i < menuManagers.Length; i++)
        {
            if (!menuManagers[i].gameObject.CompareTag("Player"))
            {
                _state.setMenuData(menuManagers[i].gameObject.name, menuManagers[i].getMenuID(), menuManagers[i].getActiveMenuItems());
                menuScripts.Add(menuManagers[i]);
            }
        }

        //Now set the day data.
        _state.setWorldData(GetComponent<DayNightManager>().getSun().transform.position, GetComponent<DayNightManager>().getSun().transform.rotation, GetComponent<GameManager>().getDay(), GetComponent<EventManager>().getEventToken());
    }

    //A function to load the world based on the current state in the manager.
    private void loadWorldState()
    {
        //Before loading world, ensure all worl objects are in the given lists.
        getObjectLists();


        //Get eveyr interactable object in one go.
        InteractionClass[] objs = GameObject.FindObjectsByType<InteractionClass>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        //Get the system managers that are active in the scene.
        SystemManager[] managers = GameObject.FindObjectsByType<SystemManager>(FindObjectsSortMode.None);

        //Debug.Log(_state.items.Count);
        //Go through each state item and match it to a given interactive reference.
        for (int i = 0; i < _state.items.Count; i++)
        {
            //Go through interaction items to find if correct position.
            for (int v = 0; v < interactionablesItems.Count; v++)
            {
                if (interactionablesItems[v].getObjectID() == _state.items[i].id)
                {
                    interactionablesItems[v].transform.position = _state.items[i].position;
                    interactionablesItems[v].transform.rotation = _state.items[i].rotation;

                    GameObject connObj = null;

                    //Find the matching object if connected object is there.
                    if (_state.items[i].connectedObjectId >= 0)
                    {
                        int count = 0;
                        //Go through every object in list and find the matching id.
                        while (count < objs.Length && !connObj)
                        {
                            if (objs[count].getObjectID() == _state.items[i].connectedObjectId)
                            {
                                connObj = objs[count].gameObject;
                            }

                            count++;
                        }
                    }

                    //If there is no connected object, ensure the object will not be placed back into those controllers.
                    if (!connObj)
                    {
                        //Debug.Log("Working? " + interactionablesItems[v].gameObject.name);
                        interactionablesItems[v].GetComponent<HoldInteractionClass>().Interact(_state.items[i].position, _state.items[i].rotation, null);
                        interactionablesItems[v].GetComponent<HoldInteractionClass>().removeHeld();
                    }

                    if (connObj && connObj.GetComponent<PositionInteractionClass>())
                    {
                        //Debug.Log("Working? " + interactionablesItems[v].gameObject.name);
                        //Due to some 'can hold' needed for interaction, run that first even though it is already obviously able to hold.
                        connObj.GetComponent<PositionInteractionClass>().canHoldItem(interactionablesItems[v].GetComponent<HoldInteractionClass>(), false);

                        //Check if using active objects.
                        if (connObj.GetComponent<PositionInteractionClass>().hasPermission(InteractionClass.interactionType.activeObjects))
                        {
                            connObj.GetComponent<PositionInteractionClass>().interactionWithActives(interactionablesItems[v].gameObject);
                        } else
                        {
                            connObj.GetComponent<PositionInteractionClass>().Interact(interactionablesItems[v].gameObject);
                        }
                    }
                    else if (connObj && connObj.GetComponentInParent<FPSController>())
                    {
                        //Debug.Log("Working2? " + interactionablesItems[v].gameObject.name);
                        connObj.GetComponentInParent<FPSController>().setHeldItem(interactionablesItems[v]);
                        interactionablesItems[v].GetComponent<InteractionClass>().Interact(connObj.transform.GetChild(0).position, connObj.transform.GetChild(0).rotation, connObj.transform);

                    }
                }
            }
        }

        //Do all locked objects.
        for (int i = 0; i < _state.locks.Count; i++)
        {
            for(int v = 0; v < interactionablesLocks.Count; v++)
            {
                //If the same object, set the initial lock.
                if(interactionablesLocks[v].getObjectID() == _state.locks[i].id)
                {
                    interactionablesLocks[v].setInitialLock(_state.locks[i].initialLock);

                    //Now set the forceIsOn to true.
                    if (interactionablesLocks[v].getInitialLock())
                    {
                        //Debug.Log("Lock: " + interactionablesLocks[v].name);
                        //Ensure only works if not using inverted lock.
                        if (!interactionablesLocks[v].GetComponent<InvertedLockObjectClass>())
                        {
                            interactionablesLocks[v].setIsOn(interactionablesLocks[v].getInitialLock());
                            interactionablesLocks[v].forceIsOn(true);
                        }
                    }

                    if (interactionablesLocks[v].GetComponent<InvertedLockObjectClass>())
                    {
                        //Du a use object flip.
                        //Debug.Log("Flippy flop?");
                        interactionablesLocks[v].setIsOn(true);
                        interactionablesLocks[v].setIsOn(false);
                    }
                    //Because all objects have been used already on making the grid function, reuse the locks.
                    interactionablesLocks[v].useObject();
                }
            }
        }

        //Do all energy interaction objects.
        for (int i = 0; i < _state.energy.Count; i++)
        {
            for (int v = 0; v < interactionablesEnergy.Count; v++)
            {
                //If the same object, set the initial lock.
                if (interactionablesEnergy[v].getObjectID() == _state.energy[i].id)
                {
                    //Prep the object to be opposite what it needs to turn to.
                    //interactionablesEnergy[v].setIsOn(!_state.energy[i].isOn);

                    if (_state.energy[i].isOn)
                    {
                        //Debug.Log("React?");
                        //No make the interaction.
                        interactionablesEnergy[v].Interact();
                    }
                }
            }
        }

        //Debug.Log(_state.screens.Count);
        //Debug.Log(energiesComputers.Count);

        //Do all energy interaction objects.
        for (int i = 0; i < _state.screens.Count; i++)
        {
            for (int v = 0; v < energiesComputers.Count; v++)
            {
                //Debug.Log("React2");
                //If the same object, set the initial lock.
                if (energiesComputers[v].getObjectID() == _state.screens[i].id)
                {
                    //Prep the object to be opposite what it needs to turn to.
                    //interactionablesEnergy[v].setIsOn(!_state.energy[i].isOn);
                    /*if (string.Equals(energiesComputers[v].name, "PasswordMenu"))
                    {
                        Debug.Log(_state.screens[i].currentData);
                    }*/
                    energiesComputers[v].setCurrentCode(_state.screens[i].currentData);

                    //Set the isOn because the computer object should be able to be turned on or off by the save file.
                    energiesComputers[v].setIsOn(_state.screens[i].on);
                }
            }
        }

        for(int i = 0; i < _state.menus.Count; i++)
        {
            for(int v = 0; v < menuScripts.Count; v++)
            {
                if(menuScripts[v].getMenuID() == _state.menus[i].id)
                {
                    //Set the active menus of this manager.
                    menuScripts[v].setActiveMenuItems(_state.menus[i].active);
                }
            }
        }

        for (int i = 0; i < _state.combinations.Count; i++)
        {
            for (int v = 0; v < energiesCombinations.Count; v++)
            {
                //If the same object, set the initial lock.
                if (energiesCombinations[v].getObjectID() == _state.combinations[i].id)
                {
                    //Prep the object to be opposite what it needs to turn to.
                    //interactionablesEnergy[v].setIsOn(!_state.energy[i].isOn);
                    energiesCombinations[v].setCurrentNumber((int)_state.combinations[i].currentNum);
                }
            }
        }

        //Debug.Log("Before change: " + GameObject.FindGameObjectWithTag("Player").transform.position);

        //Change all entities to the current world state.
        for (int i = 0; i < _state.entities.Count; i++)
        {
            for (int v = 0; v < entities.Count; v++)
            {
                //See if entity is an fps controller.
                if (entities[v].GetComponent<FPSController>())
                {
                    if (entities[v].GetComponent<FPSController>().getPlayerID() == _state.entities[i].entityID)
                    {
                        entities[v].transform.position = _state.entities[i].position;
                        entities[v].transform.rotation = _state.entities[i].rotation;
                    }
                }
            }
        }

        //Change all event scripts to the current world state.
        for (int i = 0; i < _state.events.Count; i++)
        {
            for (int v = 0; v < eventsScripts.Count; v++)
            {
                //See if entity is an fps controller.
                if (eventsScripts[v].getEventID() == _state.events[i].id)
                {
                    //Debug.Log("Run?" + _state.events[i].isUsed);
                    eventsScripts[v].setIsPlayed(_state.events[i].isUsed);
                }
            }
        }

        //Do all generators.
        for(int i = 0; i < _state.generators.Count; i++)
        {
            for(int v = 0; v < interactionablesGenerator.Count; v++)
            {
                //See if the same id.
                if(interactionablesGenerator[v].getObjectID() == _state.generators[i].id)
                {
                    //Set the is on.
                    interactionablesGenerator[v].setIsFixed(_state.generators[i].isFixed);
                    interactionablesGenerator[v].Interact(_state.generators[i].isOn);

                    SystemManager connObj = null;

                    //Now set the manager.
                    if (_state.generators[i].systemId >= 0)
                    {
                        int count = 0;
                        //Go through every object in list and find the matching id.
                        while (count < managers.Length && !connObj)
                        {
                            if (managers[count].getSystemId() == _state.generators[i].systemId)
                            {
                                connObj = managers[count].GetComponent<SystemManager>();
                            }

                            count++;
                        }

                        //If has a connected object at end, then set it to the generator.
                        interactionablesGenerator[v].setManager(connObj);
                    } else
                    {
                        interactionablesGenerator[v].setManager(null);
                    }
                }
            }
        }

        //Do all grid switches.
        for(int i = 0; i < _state.gridsSwitches.Count; i++)
        {
            for(int v = 0; v < interactionablesGrid.Count; v++)
            {
                if(interactionablesGrid[v].getObjectID() == _state.gridsSwitches[i].id)
                {
                    interactionablesGrid[v].Interact(_state.gridsSwitches[i].isOn);
                }
            }
        }

        //Debug.Log("After change: " + GameObject.FindGameObjectWithTag("Player").transform.position);

        GetComponent<DayNightManager>().getSun().transform.position = _state.world.sunPos;
        GetComponent<DayNightManager>().getSun().transform.rotation = _state.world.sunRot;
        GetComponent<GameManager>().setDay(_state.world.isDay);
        GetComponent<EventManager>().setEventToken(_state.world.storyPoint);
    }

    private void getObjectLists()
    {
        InteractionClass[] interactables = GameObject.FindObjectsByType<InteractionClass>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        LockObjectClass[] lockables = GameObject.FindObjectsByType<LockObjectClass>(FindObjectsSortMode.None);
        FPSController[] playerEntities = GameObject.FindObjectsByType<FPSController>(FindObjectsSortMode.None);
        EnergyObjectClass[] energyObjects = GameObject.FindObjectsByType<EnergyObjectClass>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        EventScript[] eventObjects = GameObject.FindObjectsByType<EventScript>(FindObjectsSortMode.None);
        MenuManager[] menus = GameObject.FindObjectsByType<MenuManager>(FindObjectsSortMode.None);
        interactionablesItems = new List<HoldInteractionClass>();
        interactionablesLocks = new List<LockObjectClass>();
        interactionablesEnergy = new List<EnergyInteractionClass>();
        interactionablesGenerator = new List<GeneratorInteractionClass>();
        interactionablesGrid = new List<GridInteractionClass>();
        energiesComputers = new List<ComputerObjectClass>();
        energiesCombinations = new List<CombinationManagerClass>();
        eventsScripts = new List<EventScript>();
        menuScripts = new List<MenuManager>();

        entities = new List<GameObject>();

        //Save the player objects into the entities list.
        for(int i = 0; i < playerEntities.Length; i++)
        {
            entities.Add(playerEntities[i].gameObject);
        }

        //Go through and save data of moveable objects.
        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].GetComponent<HoldInteractionClass>() &&
               (interactables[i].GetComponent<HoldInteractionClass>().isInteractionType(InteractionClass.interactionType.player) ||
                interactables[i].GetComponent<HoldInteractionClass>().isInteractionType(InteractionClass.interactionType.playerHold)))
            {
                interactionablesItems.Add(interactables[i].GetComponent<HoldInteractionClass>());
            }
        }

        //Go through and save data of lockable objects.
        for (int i = 0; i < lockables.Length; i++)
        {
            interactionablesLocks.Add(lockables[i]);
        }

        //Go through energyInteraction classes.
        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].GetComponent<EnergyInteractionClass>())
            {
                interactionablesEnergy.Add(interactables[i].GetComponent<EnergyInteractionClass>());
            }
        }

        for(int i = 0; i < energyObjects.Length; i++)
        {
            if (energyObjects[i].GetComponent<ComputerObjectClass>()) 
            {
                energiesComputers.Add(energyObjects[i].GetComponent<ComputerObjectClass>());
            }
        }

        for(int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].GetComponent<CombinationManagerClass>())
            {
                energiesCombinations.Add(interactables[i].GetComponent<CombinationManagerClass>());
            }
        }

        for(int i = 0; i < eventObjects.Length; i++)
        {
            eventsScripts.Add(eventObjects[i]);
        }

        for(int i = 0; i < menus.Length; i++)
        {
            if (!menus[i].gameObject.CompareTag("Player"))
            {
                menuScripts.Add(menus[i]);
            }
        }

        for(int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].GetComponent<GeneratorInteractionClass>())
            {
                interactionablesGenerator.Add(interactables[i].GetComponent<GeneratorInteractionClass>());
            }
        }

        for(int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].GetComponent<GridInteractionClass>())
            {
                interactionablesGrid.Add(interactables[i].GetComponent<GridInteractionClass>());
            }
        }
    }

    public void generateInitialState()
    {
        saveWorldState();

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        //Get path for the file manager.
        string p = string.Concat(Application.persistentDataPath, "/saveFiles/fileManager" + sceneName + ".txt");

        //If the manager file doesn't exist, create one.
        if (!System.IO.File.Exists(p))
        {
            createNewManager(p);
        }

        //Get a json text of all datas.
        string text = JsonUtility.ToJson(_state, true);

        System.IO.File.WriteAllText(Application.persistentDataPath + "/saveFiles/" + sceneName + "InitialState.json", text);


    }

    public void saveNewState()
    {
        saveWorldState();

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        //Get path for the file manager.
        string p = string.Concat(Application.persistentDataPath, "/saveFiles/fileManager" + sceneName + ".txt");

        //If the manager file doesn't exist, create one.
        if (!System.IO.File.Exists(p))
        {
            createNewManager(p);
        }

        //Get a json text of all datas.
        string text = JsonUtility.ToJson(_state, true);

        //Find the current world index.
        int count = getSaveAmount(p);

        System.IO.File.WriteAllText(Application.persistentDataPath + "/saveFiles/" + sceneName + "SaveState" + count + ".json", text);

        //Write the file path to the manager.
        System.IO.StreamWriter wr = new System.IO.StreamWriter(p, true);

        wr.WriteLine(Application.persistentDataPath + "/saveFiles/" + sceneName + "SaveState" + count + ".json");

        wr.Close();

    }

    public int getSaveCount()
    {
        return saveCount;
    }

    public int getObjectAmount()
    {
        return interactionablesItems.Count;
    }

    public int getEnergyAmount()
    {
        return interactionablesEnergy.Count;
    }

    public HoldInteractionClass getInteractionObjects(int i)
    {
        return interactionablesItems[i];
    }

    public EnergyInteractionClass getInteractionEnergy(int i)
    {
        return interactionablesEnergy[i];
    }

    public int getSaveAmount(string path)
    {
        System.IO.StreamReader re = new System.IO.StreamReader(path);

        int count = 0;
        while(re.ReadLine() != null)
        {
            count++;
        }

        re.Close();

        return count;
    }
}

[System.Serializable]
public class worldState
{
    public List<entityData> entities;
    //public cameraData cam;
    public List<itemData> items;
    public List<lockData> locks;
    public List<energyInteractionData> energy;
    public List<screenData> screens;
    public List<combinationData> combinations;
    public List<EventData> events;
    public List<MenuData> menus;
    public List<generatorData> generators;
    public List<gridData> gridsSwitches;

    public WorldData world;
    //public List<positionData> positions;
    //public List<energyData> energy;

    //Set one entity into the save file.
    public void setEntity(string n, float i, Vector3 pos, Quaternion rot)
    {
        entityData d = new entityData(n, i, pos, rot);

        if (entities == null)
        {
            entities = new List<entityData>();
        }
        entities.Add(d);
    }

    //Set the item data of this state.
    public void setItem(string i, float ident, Vector3 pos, Quaternion rot, float ci)
    {
        itemData ib = new itemData(i, ident, pos, rot, ci);

        if (items == null)
        {
            items = new List<itemData>();
        }
        items.Add(ib);
    }

    //Set lock data of this state.
    public void setLocks(string i, float ident, bool init)
    {
        lockData il = new lockData(i, ident, init);

        if(locks == null)
        {
            locks = new List<lockData>();
        }
        locks.Add(il);
    }

    //Set the energy interaction data for this state.
    public void setEnergyInteractions(string i, float ident, bool init)
    {
        energyInteractionData il = new energyInteractionData(i, ident, init);

        if(energy == null)
        {
            energy = new List<energyInteractionData>();
        }

        energy.Add(il);
    }

    //Set a screen object data. This does not include if it is on or not.
    public void setScreenData(string i, float ident, string s, bool b)
    {
        screenData il = new screenData(i, ident, s, b);

        if (screens == null)
        {
            screens = new List<screenData>();
        }

        screens.Add(il);
    }

    //Set the current data of a given combination lock.
    public void setCombinationData(string i, float ident, float data)
    {
        combinationData il = new combinationData(i, ident, data);

        if(combinations == null)
        {
            combinations = new List<combinationData>();
        }

        combinations.Add(il);
    }

    public void setEventData(string n, float i, bool b)
    {
        EventData il = new EventData(n, i, b);

        if(events == null)
        {
            events = new List<EventData>();
        }

        events.Add(il);
    }

    public void setWorldData(Vector3 p, Quaternion r, bool b, int i)
    {
        world = new WorldData(p, r, b, i);
    }

    public void setMenuData(string s, float i, string a)
    {
        MenuData il = new MenuData(s, i, a);

        if(menus == null)
        {
            menus = new List<MenuData>();
        }

        menus.Add(il);
    }

    public void setGeneratorData(string s, float i, bool b, bool bi, float a)
    {
        generatorData il = new generatorData(s, i, b, bi, a);

        if(generators == null)
        {
            generators = new List<generatorData>();
        }

        generators.Add(il);
    }

    public void setGridData(string s, float i, bool b)
    {
        gridData il = new gridData(s, i, b);

        if (gridsSwitches == null)
        {
            gridsSwitches = new List<gridData>();
        }

        gridsSwitches.Add(il);
    }
}

//For all entities in the game.
[System.Serializable]
public class entityData
{
    public string name;
    public float entityID;
    public Vector3 position;
    public Quaternion rotation;

    public entityData(string n, float i, Vector3 pos, Quaternion rot)
    {
        name = n;
        entityID = i;
        position = pos;
        rotation = rot;
    }
}

//For items that can be held or moved.
[System.Serializable]
public class itemData
{
    public string name;
    public float id;
    public Vector3 position;
    public Quaternion rotation;
    public float connectedObjectId;

    public itemData(string i, float ident, Vector3 pos, Quaternion rot, float ci)
    {
        name = i;
        id = ident;
        position = pos;
        rotation = rot;
        connectedObjectId = ci;
    }
}

//For locked energy objects.
[System.Serializable]
public class lockData
{
    public string name;
    public float id;
    public bool initialLock;

    public lockData(string i, float ident, bool init)
    {
        name = i;
        initialLock = init;
        id = ident;
    }
}

[System.Serializable]
public class energyInteractionData
{
    public string name;
    public float id;
    public bool isOn;

    public energyInteractionData(string i, float ident, bool init)
    {
        name = i;
        id = ident;
        isOn = init;
    }
}

[System.Serializable]
public class generatorData
{
    public string name;
    public float id;
    public bool isOn;
    public bool isFixed;
    public float systemId;

    public generatorData(string i, float ident, bool b, bool bi, float other)
    {
        name = i;
        id = ident;
        isOn = b;
        isFixed = bi;
        systemId = other;
    }
}

[System.Serializable]
public class gridData
{
    public string name;
    public float id;
    public bool isOn;

    public gridData(string i, float ident, bool b)
    {
        name = i;
        id = ident;
        isOn = b;
    }
}

[System.Serializable]
public class screenData
{
    public string name;
    public float id;
    public string currentData;
    public bool on;

    public screenData(string i, float ident, string s, bool b)
    {
        name = i;
        id = ident;
        currentData = s;
        on = b;
    }
}

[System.Serializable]
public class combinationData
{
    public string name;
    public float id;
    public float currentNum;

    public combinationData(string i, float ident, float data)
    {
        name = i;
        id = ident;
        currentNum = data;
    }
}

[System.Serializable]
public class WorldData
{
    public Vector3 sunPos;
    public Quaternion sunRot;
    public bool isDay;
    public int storyPoint;

    public WorldData(Vector3 p, Quaternion r, bool b, int st)
    {
        sunPos = p;
        sunRot = r;
        isDay = b;
        storyPoint = st;
    }
}

[System.Serializable]
public class EventData
{
    public string name;
    public float id;
    public bool isUsed;

    public EventData(string n, float i, bool b)
    {
        name = n;
        id = i;
        isUsed = b;
    }
}

[System.Serializable]
public class MenuData
{
    public string name;
    public float id;
    public string active;

    public MenuData(string n, float i, string s)
    {
        name = n;
        id = i;
        active = s;
    }
}

//For the camera data that needs to be saved.
/*[System.Serializable]
public class cameraData
{
    public int currentCam;
}*/

//For positions that can hold an item.
/*public class positionData
{
    public int id;

    public positionData(int i)
    {
        id = i;
    }
}*/

/*[System.Serializable]
//For energy items that can be turned on or off.
public class energyData
{
    public int id;
    public bool isOn;

    public energyData(int i, bool b)
    {
        id = i;
        isOn = b;
    }
}*/

//For gridManagers.
