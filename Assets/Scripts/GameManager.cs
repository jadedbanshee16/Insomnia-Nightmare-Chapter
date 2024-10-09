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
