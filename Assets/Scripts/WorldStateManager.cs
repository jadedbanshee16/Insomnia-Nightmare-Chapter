using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private string initialFileName;
    [SerializeField]
    private string currentFileManager;

    private worldState _state;

    public void loadWorld()
    {
        //Collected all interactables, positions and energy interactions in a level.
        interactionablesItems = GameObject.FindObjectsOfType<HoldInteractionClass>();
        interactablePositions = GameObject.FindObjectsOfType<PositionInteractionClass>();
        energyInteractions = GameObject.FindObjectsOfType<EnergyInteractionClass>();
        generatorInteractions = GameObject.FindObjectsOfType<GeneratorInteractionClass>();
        gridInteractions = GameObject.FindObjectsOfType<GridInteractionClass>();

        //Sort interactables.
        List<HoldInteractionClass> newList = new List<HoldInteractionClass>();

        for(int i = 0; i < interactionablesItems.Length; i++)
        {
            if (interactionablesItems[i].isInteractionType(InteractionClass.interactionType.player) || interactionablesItems[i].isInteractionType(InteractionClass.interactionType.playerHold))
            {
                newList.Add(interactionablesItems[i]);
            }
        }

        //Change lists.
        interactionablesItems = new HoldInteractionClass[newList.Count];

        for(int i = 0; i < interactionablesItems.Length; i++)
        {
            interactionablesItems[i] = newList[i];
        }
        
        
        //Try to load data.
        string p = string.Concat(Application.persistentDataPath, "/saveFiles/" + currentFileManager + ".txt");

        //Debug.Log(p);
        _state = new worldState();

        //If the manager file doesn't exist, create that file and a new file with a current final final.
        if (!System.IO.File.Exists(p))
        {
            p = string.Concat(Application.persistentDataPath, "/saveFiles/" + initialFileName + ".json");

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
            }
        }

        //Find the file path of the newest save and load that data.
        System.IO.StreamReader re = new System.IO.StreamReader(Application.persistentDataPath + "/saveFiles/" + currentFileManager + ".txt");

        string lastPath = "";
        string currentLine;
        while ((currentLine = re.ReadLine()) != null)
        {
            //This just reads all the lines.
            lastPath = currentLine;
        }

        re.Close();

        string jsonString = System.IO.File.ReadAllText(lastPath);
        //Now use this path and turn to state.
        _state = JsonUtility.FromJson<worldState>(jsonString);

        loadWorldState();
    }

    //Find and save the current data of connected objects to this work state.
    public void saveIntoJson(int fileIndex, string currentPath, string fileName)
    {
        saveWorldState();

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

        //Now save the new current file name to the list of names in manager.
        System.IO.StreamWriter wr = new System.IO.StreamWriter(Application.persistentDataPath + "/saveFiles/" + currentFileManager + ".txt", true);

        if(fileIndex < 0)
        {
            //Write new line.
            wr.WriteLine(currentPath + fileName + ".json");
        } else
        {
            //Write new line.
            wr.WriteLine(currentPath + fileName + fileIndex + ".json");
        }

        wr.Close();
    }

    //A function to save the current world state.
    public void saveWorldState()
    {
        //Save entities.
        for(int i = 0; i < entities.Length; i++)
        {
            _state.setEntity(entities[i].GetInstanceID(), entities[i].transform.position, entities[i].transform.rotation);
        }

        //Save the camera used.
        _state.setCurrentCam(currentCam);

        //Save all held items that need to be saved.
        for(int i = 0; i < interactionablesItems.Length; i++)
        {
            int newId = 0;

            if (interactionablesItems[i].getCurrentHolder())
            {
                newId = interactionablesItems[i].getCurrentHolder().transform.GetInstanceID();
            } else
            {
                newId = 0;
            }

            _state.setItem(interactionablesItems[i].GetInstanceID(), interactionablesItems[i].gameObject.activeSelf, interactionablesItems[i].transform.position,
                interactionablesItems[i].transform.rotation, newId);
        }

        for(int i = 0; i < energyInteractions.Length; i++)
        {
            //_state.setEnergy(energyInteractions[i].gameObject.GetInstanceID(), energyInteractions[i].GetComponent<EnergyInteractionClass>().getIsOn());
        }

        for(int i = 0; i < generatorInteractions.Length; i++)
        {
            //_state.setEnergy(generatorInteractions[i].gameObject.GetInstanceID(), generatorInteractions[i].GetComponent<GeneratorInteractionClass>().getIsOn());
        }

        for(int i = 0; i < gridInteractions.Length; i++)
        {
           // _state.setEnergy(gridInteractions[i].gameObject.GetInstanceID(), gridInteractions[i].GetComponent<GridInteractionClass>().getIsOn());
        }

        //Save the position data of things that need to be saved.
        /*for(int i = 0; i < interactablePositions.Length; i++)
        {
            _state.setPosition(interactablePositions[i].gameObject.GetInstanceID());
        }*/
    }

    //A function to load the world based on the current state in the manager.
    private void loadWorldState()
    {
        //Load all the entities that match with what needs to be loaded in world manager.
        for(int i = 0; i < entities.Length; i++)
        {
            for(int j = 0; j < _state.entities.Count; j++)
            {
                //If they are the same object, then set object.
                if (entities[i].GetInstanceID() == _state.entities[j].instanceId)
                {
                    entities[i].transform.position = _state.entities[j].position;
                    entities[i].transform.rotation = _state.entities[j].rotation;
                }
            }
        }

        currentCam = _state.cam.currentCam;

        //Load camera.
        camController = GameObject.FindGameObjectWithTag("Player").GetComponent<CameraManager>();

        camController.setCamera(currentCam);

        //Load all the entities that match with what needs to be loaded in world manager.
        for (int i = 0; i < interactionablesItems.Length; i++)
        {
            for (int j = 0; j < _state.items.Count; j++)
            {
                //If they are the same object, then set object.
                if (interactionablesItems[i].GetInstanceID() == _state.items[j].id)
                {
                    interactionablesItems[i].transform.position = _state.items[j].position;
                    interactionablesItems[i].transform.rotation = _state.items[j].rotation;
                    interactionablesItems[i].gameObject.SetActive(_state.items[j].isActive);

                    //Check if connected object id is connected to this object.
                    for(int v = 0; v < interactablePositions.Length; v++)
                    {
                        if(_state.items[j].connectedObjectId == interactablePositions[v].transform.GetInstanceID())
                        {
                            //Debug.Log("Found state! " + interactionablesItems[i].gameObject.name + " | " + interactablePositions[v].gameObject.name);
                            //This means a connected position has occured. set interaction point.
                            interactablePositions[v].Interact(interactionablesItems[i].gameObject);
                        }
                    }
                }
            }
        }

        //Load energy objects now.
        /*for(int i = 0; i < energyInteractions.Length; i++)
        {
            for(int j = 0; j < _state.energy.Count; j++)
            {
                if(energyInteractions[i].gameObject.GetInstanceID() == _state.energy[j].id)
                {
                    if (_state.energy[j].isOn)
                    {
                        energyInteractions[i].Interact();
                    }
                }
            }
        }

        //Load energy objects now.
        for (int i = 0; i < gridInteractions.Length; i++)
        {
            for (int j = 0; j < _state.energy.Count; j++)
            {
                if (gridInteractions[i].gameObject.GetInstanceID() == _state.energy[j].id)
                {
                    if (_state.energy[j].isOn)
                    {
                        energyInteractions[i].Interact();
                    }
                }
            }
        }

        //Load energy objects now.
        for (int i = 0; i < generatorInteractions.Length; i++)
        {
            for (int j = 0; j < _state.energy.Count; j++)
            {
                if (generatorInteractions[i].gameObject.GetInstanceID() == _state.energy[j].id)
                {
                    if (_state.energy[j].isOn)
                    {
                        energyInteractions[i].Interact();
                    }
                }
            }
        }*/
    }

    public int getSaveAmount()
    {
        System.IO.StreamReader re = new System.IO.StreamReader(Application.persistentDataPath + "/saveFiles/" + currentFileManager + ".txt");

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
    public cameraData cam;
    public List<itemData> items;
    //public List<positionData> positions;
    public List<energyData> energy;

    //Set one entity into the save file.
    public void setEntity(int n, Vector3 pos, Quaternion rot)
    {
        entityData d = new entityData(n, pos, rot);

        if (entities == null)
        {
            entities = new List<entityData>();
        }
        entities.Add(d);
    }

    //Set the camera of this state.
    public void setCurrentCam(int i)
    {
        cam = new cameraData();

        cam.currentCam = i;
    }

    //Set the item data of this state.
    public void setItem(int i, bool a, Vector3 pos, Quaternion rot, int ci)
    {
        itemData ib = new itemData(i, a, pos, rot, ci);

        if (items == null)
        {
            items = new List<itemData>();
        }
        items.Add(ib);
    }

    public void setEnergy(int i, bool a)
    {
        energyData e = new energyData(i, a);

        if(energy == null)
        {
            energy = new List<energyData>();
        }

        energy.Add(e);
    }

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
    public int instanceId;
    public Vector3 position;
    public Quaternion rotation;

    public entityData(int n, Vector3 pos, Quaternion rot)
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
    public int id;
    public bool isActive;
    public Vector3 position;
    public Quaternion rotation;
    public int connectedObjectId;

    public itemData(int i, bool a, Vector3 pos, Quaternion rot, int ci)
    {
        id = i;
        isActive = a;
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
