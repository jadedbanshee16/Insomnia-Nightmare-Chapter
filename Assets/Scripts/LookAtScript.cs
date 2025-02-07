using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtScript : MonoBehaviour
{
    [SerializeField]
    Transform obj;
    // Start is called before the first frame update
    void Start()
    {
        if (!obj)
        {
            obj = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(obj);
    }
}
