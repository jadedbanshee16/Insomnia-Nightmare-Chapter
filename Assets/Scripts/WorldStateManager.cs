using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] entities;
    [SerializeField]
    int currentCam;
    CameraManager camController;
    [SerializeField]
    List<HoldInteractionClass> interactionablesItems;
    [SerializeField]
    List<LockObjectClass> interactionablesLocks;

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

        _state = new worldState();
        interactionablesItems = new List<HoldInteractionClass>();
        interactionablesLocks = new List<LockObjectClass>();

        for (int i = 0; i < entities.Length; i++)
        {
            //Return the entity.
            _state.setEntity(entities[i].gameObject.name, entities[i].transform.position, entities[i].transform.rotation);
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

        LockObjectClass[] lockables = GameObject.FindObjectsByType<LockObjectClass>(FindObjectsSortMode.None);

        //Go through and save data of lockable objects.
        for(int i = 0; i < lockables.Length; i++)
        {
            _state.setLocks(lockables[i].gameObject.name, lockables[i].getObjectID(), lockables[i].getInitialLock());
            interactionablesLocks.Add(lockables[i]);
        }
    }

    //A function to load the world based on the current state in the manager.
    private void loadWorldState()
    {
        //Get eveyr interactable object in one go.
        InteractionClass[] objs = GameObject.FindObjectsByType<InteractionClass>(FindObjectsSortMode.None);

        //Change all entities to the current world state.
        for (int i = 0; i < _state.entities.Count; i++)
        {
            for(int v = 0; v < entities.Length; v++)
            {
                if(entities[v].name == _state.entities[i].instanceId)
                {
                    entities[v].transform.position = _state.entities[i].position;
                    entities[v].transform.rotation = _state.entities[i].rotation;
                }
            }
        }

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

        /*InteractionClass[] interactables = GameObject.FindObjectsOfType<InteractionClass>();

        int count = 0;

        //Go through and save data of moveable objects.
        for (int i = 0; i < interactables.Length; i++)
        {
            if (interactables[i].GetComponent<HoldInteractionClass>() &&
               (interactables[i].GetComponent<HoldInteractionClass>().isInteractionType(InteractionClass.interactionType.player)) &&
                interactables[i].gameObject.name == _state.items[count].name)
            {

                interactables[i].transform.position = _state.items[count].position;
                interactables[i].transform.rotation = _state.items[count].rotation;


                GameObject connObj = GameObject.Find(_state.items[count].connectedObjectId);

                count++;

                if (connObj)
                {
                    connObj.GetComponent<PositionInteractionClass>().Interact(interactables[i].gameObject);
                }
            }
        }*/
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

    /*private void populateInteractions()
    {
        //Collected all interactables, positions and energy interactions in a level.
        interactionablesItems = GameObject.FindObjectsOfType<HoldInteractionClass>();
        interactablePositions = GameObject.FindObjectsOfType<PositionInteractionClass>();
        energyInteractions = GameObject.FindObjectsOfType<EnergyInteractionClass>();
        generatorInteractions = GameObject.FindObjectsOfType<GeneratorInteractionClass>();
        gridInteractions = GameObject.FindObjectsOfType<GridInteractionClass>();

        //Sort interactables.
        List<HoldInteractionClass> newList = new List<HoldInteractionClass>();

        for (int i = 0; i < interactionablesItems.Length; i++)
        {
            if (interactionablesItems[i].isInteractionType(InteractionClass.interactionType.player) || interactionablesItems[i].isInteractionType(InteractionClass.interactionType.playerHold))
            {
                newList.Add(interactionablesItems[i]);
            }
        }

        //Change lists.
        interactionablesItems = new HoldInteractionClass[newList.Count];

        for (int i = 0; i < interactionablesItems.Length; i++)
        {
            interactionablesItems[i] = newList[i];
        }
    }*/

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
    //public List<positionData> positions;
    //public List<energyData> energy;

    //Set one entity into the save file.
    public void setEntity(string n, Vector3 pos, Quaternion rot)
    {
        entityData d = new entityData(n, pos, rot);

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

    //Set the camera of this state.
    /*public void setCurrentCam(int i)
    {
        cam = new cameraData();

        cam.currentCam = i;
    }*/

    /*public void setEnergy(int i, bool a)
    {
        energyData e = new energyData(i, a);

        if(energy == null)
        {
            energy = new List<energyData>();
        }

        energy.Add(e);
    }*/

    //Ser position data.
    /*public void setPosition(int i)
    {
        positionData pd = new positionData(i);

        if (positions == null)
        {
            positions = new List<positionData>();
        }
        positions.Add(pd);
    }*/
}

//For all entities in the game.
[System.Serializable]
public class entityData
{
    public string instanceId;
    public Vector3 position;
    public Quaternion rotation;

    public entityData(string n, Vector3 pos, Quaternion rot)
    {
        instanceId = n;
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
