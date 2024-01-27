using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            moveZ();
        }
    }
    void moveZ()
    {
        //Get current position.
        float pos = transform.position.z;

        //Add the speed.
        pos += moveSpeed;

        //change position.
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, pos);

        transform.position = newPos;
    }
}
