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

        for(int i = 0; i < systemManagers.Length; i++)
        {
            systemManagers[i].setManager();
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
