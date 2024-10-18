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
    HoldInteractionClass[] interactionablesItems;
    [SerializeField]
    PositionInteractionClass[] interactablePositions;
    [SerializeField]
    EnergyInteractionClass[] energyInteractions;
    [SerializeField]
    GeneratorInteractionClass[] generatorInteractions;
    [SerializeField]
    GridInteractionClass[] gridInteractions;

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
        System.IO.StreamWriter wr = new System.IO.StreamWriter(path, true);

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        wr.WriteLine(Application.persistentDataPath + "/saveFiles/" + sceneName + "InitialState.json");

        wr.Close();
    }

    //A function to save the current world state.
    public void saveWorldState()
    {
        InteractionClass[] interactables = GameObject.FindObjectsOfType<InteractionClass>();

        _state = new worldState();

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
                    _state.setItem(interactables[i].name, interactables[i].transform.position, interactables[i].transform.rotation, interactables[i].GetComponent<HoldInteractionClass>().getCurrentHolder().gameObject.name);
                }
                else
                {
                    _state.setItem(interactables[i].name, interactables[i].transform.position, interactables[i].transform.rotation, null);
                }

            }
        }
    }

    //A function to load the world based on the current state in the manager.
    private void loadWorldState()
    {

        //Change all entities to the current world state.
        for (int i = 0; i < _state.entities.Count; i++)
        {
            GameObject obj = GameObject.Find(_state.entities[i].instanceId);

            if (obj)
            {
                obj.transform.position = _state.entities[i].position;
                obj.transform.rotation = _state.entities[i].rotation;
            }
        }

        //Change holdable or touchable hold time to the new position... or actual position.
        for(int i = 0; i < _state.items.Count; i++)
        {
            GameObject obj = GameObject.Find(_state.items[i].name);

            if (obj)
            {
                obj.transform.position = _state.items[i].position;
                obj.transform.rotation = _state.items[i].rotation;
                //Now make a connection.
                GameObject connObj = GameObject.Find(_state.items[i].connectedObjectId);

                if (connObj)
                {
                    connObj.GetComponent<PositionInteractionClass>().Interact(obj);
                }
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

    /*public int getSaveAmount()
    {
        System.IO.StreamReader re = new System.IO.StreamReader(Application.persistentDataPath + "/saveFiles/fileManager" + ".txt");

        int count = 0;
        while(re.ReadLine() != null)
        {
            count++;
        }

        re.Close();

        return count;
    }*/
}

[System.Serializable]
public class worldState
{
    public List<entityData> entities;
    //public cameraData cam;
    public List<itemData> items;
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

    //Set the camera of this state.
    /*public void setCurrentCam(int i)
    {
        cam = new cameraData();

        cam.currentCam = i;
    }*/

    //Set the item data of this state.
    public void setItem(string i, Vector3 pos, Quaternion rot, string ci)
    {
        itemData ib = new itemData(i, pos, rot, ci);

        if (items == null)
        {
            items = new List<itemData>();
        }
        items.Add(ib);
    }

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

//For the camera data that needs to be saved.
[System.Serializable]
public class cameraData
{
    public int currentCam;
}

//For items that can be held or moved.
[System.Serializable]
public class itemData
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public string connectedObjectId;

    public itemData(string i, Vector3 pos, Quaternion rot, string ci)
    {
        name = i;
        position = pos;
        rotation = rot;
        connectedObjectId = ci;
    }
}

//For positions that can hold an item.
/*public class positionData
{
    public int id;

    public positionData(int i)
    {
        id = i;
    }
}*/

[System.Serializable]
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
}

//For gridManagers.
