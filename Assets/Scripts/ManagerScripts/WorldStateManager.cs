using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    private worldState _state;

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
    public void saveIntoJson(int fileIndex, string currentPath, string fileName)
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
    }

    public void createNewManager(string path)
    {
        //Now save the new current file name to the list of names in manager.
        System.IO.StreamWriter wr = new System.IO.StreamWriter(path, false);

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        wr.WriteLine(Application.persistentDataPath + "/saveFiles/" + sceneName + "InitialState.json");

        wr.Close();
    }

    //A function to save the current world state.
    public void saveWorldState()
    {
        InteractionClass[] interactables = GameObject.FindObjectsByType<InteractionClass>(FindObjectsSortMode.None);
        LockObjectClass[] lockables = GameObject.FindObjectsByType<LockObjectClass>(FindObjectsSortMode.None);
        FPSController[] playerEntities = GameObject.FindObjectsByType<FPSController>(FindObjectsSortMode.None);

        _state = new worldState();
        interactionablesItems = new List<HoldInteractionClass>();
        interactionablesLocks = new List<LockObjectClass>();
        interactionablesEnergy = new List<EnergyInteractionClass>();
        entities = new List<GameObject>();

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

        //Now set the day data.
        _state.setDayData(GetComponent<DayNightManager>().getSun().transform.position, GetComponent<DayNightManager>().getSun().transform.rotation);
    }

    //A function to load the world based on the current state in the manager.
    private void loadWorldState()
    {
        //Before loading world, ensure all worl objects are in the given lists.
        getObjectLists();


        //Get eveyr interactable object in one go.
        InteractionClass[] objs = GameObject.FindObjectsByType<InteractionClass>(FindObjectsSortMode.None);

        //Debug.Log(_state.items.Count);

        //Go through each state item and match it to a given interactive reference.
        for (int i = 0; i < _state.items.Count; i++)
        {
            //Go through interaction items to find if correct position.
            for(int v = 0; v < interactionablesItems.Count; v++)
            {
                if(interactionablesItems[v].getObjectID() == _state.items[i].id)
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

                    if (connObj && connObj.GetComponent<PositionInteractionClass>())
                    {
                        connObj.GetComponent<PositionInteractionClass>().Interact(interactionablesItems[v].gameObject);
                    }
                    else if (connObj && connObj.GetComponentInParent<FPSController>())
                    {
                        connObj.GetComponentInParent<FPSController>().setHeldItem(interactionablesItems[v]);
                    }
                }
            }

        }

        //Do all locked objects.
        for(int i = 0; i < _state.locks.Count; i++)
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
                        //Set the isOn.
                        interactionablesLocks[v].setIsOn(interactionablesLocks[v].getInitialLock());
                        interactionablesLocks[v].forceIsOn(true);
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

        GetComponent<DayNightManager>().getSun().transform.position = _state.day.pos;
        GetComponent<DayNightManager>().getSun().transform.rotation = _state.day.rot;
    }

    private void getObjectLists()
    {
        InteractionClass[] interactables = GameObject.FindObjectsByType<InteractionClass>(FindObjectsSortMode.None);
        LockObjectClass[] lockables = GameObject.FindObjectsByType<LockObjectClass>(FindObjectsSortMode.None);
        FPSController[] playerEntities = GameObject.FindObjectsByType<FPSController>(FindObjectsSortMode.None);
        interactionablesItems = new List<HoldInteractionClass>();
        interactionablesLocks = new List<LockObjectClass>();
        interactionablesEnergy = new List<EnergyInteractionClass>();
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

    public dayData day;
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

    public void setDayData(Vector3 p, Quaternion r)
    {
        day = new dayData(p, r);
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
public class dayData
{
    public Vector3 pos;
    public Quaternion rot;

    public dayData(Vector3 p, Quaternion r)
    {
        pos = p;
        rot = r;
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
