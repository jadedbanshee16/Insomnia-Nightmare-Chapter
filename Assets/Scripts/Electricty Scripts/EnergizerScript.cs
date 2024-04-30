using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergizerScript : MonoBehaviour
{
    [SerializeField]
    GameObject[] toPower;

    bool energized;

    private void Start()
    {
        energized = false;
    }

    //Energize every item connected to this power source.
    public void energize()
    {
        for(int i = 0; i < toPower.Length; i++)
        {
            //Test if object is Keypad. If so, can turn on or off.
            if(toPower[i] && toPower[i].GetComponent<KeyPadScreen>())
            {
                toPower[i].GetComponent<KeyPadScreen>().energizeItem(true);
            }

            //Test if object is door object.
            if (toPower[i] && toPower[i].GetComponent<DoorClass>())
            {
                toPower[i].GetComponent<DoorClass>().energizeItem(true);
            }
        }
    }

    //Remove energy to everything connected to this power source.
    public void deEnergize()
    {
        /*
         * Same as with energize.
         */
        for (int i = 0; i < toPower.Length; i++)
        {
            if (toPower[i] && toPower[i].GetComponent<KeyPadScreen>())
            {
                toPower[i].GetComponent<KeyPadScreen>().energizeItem(false);
            }

            if (toPower[i] && toPower[i].GetComponent<DoorClass>())
            {
                toPower[i].GetComponent<DoorClass>().energizeItem(false);
            }
        }
    }

}
