using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //Search for duplicate and ensure that this object will be destroyed if there is already an item in this.
        SteamManager[] man_ = GameObject.FindObjectsByType<SteamManager>(FindObjectsSortMode.None);

        if(man_.Length > 1)
        {
            Destroy(this.gameObject);
        } else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
