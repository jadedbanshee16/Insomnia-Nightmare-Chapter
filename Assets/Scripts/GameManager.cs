using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    /*Scripts*/
    public bool isDay;

    [SerializeField]
    SystemManager[] systemManagers;

    private void Awake()
    {
        setInteractables();

        //Set up audio to happen in the awake section.
        GetComponent<AudioManager>().setUpManager();

        if (systemManagers.Length > 0)
        {
            for (int i = 0; i < systemManagers.Length; i++)
            {
                systemManagers[i].setManager();
            }
        } else
        {
            Debug.LogWarning("Game Manager does not have any systems to manage.");
        }

        //Initiate the first state of the world.
        GetComponent<WorldStateManager>().loadWorld();

    }

    public void setInteractables()
    {
        InteractionClass[] interactables = GameObject.FindObjectsByType<InteractionClass>(FindObjectsSortMode.None);
        EnergyObjectClass[] energies = GameObject.FindObjectsByType<EnergyObjectClass>(FindObjectsSortMode.None);

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
                if(interactables[v].getObjectID() == value)
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


    public bool getDay()
    {
        return isDay;
    }

    public void setDay(bool b)
    {
        isDay = b;
    }
}
