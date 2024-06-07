using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToHandleScript : MonoBehaviour
{
    // Start is called before the first frame update
    Transform target;

    Vector3 targetDir;

    void Start()
    {
        target = GetComponentInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        targetDir = target.position - this.transform.position;

        targetDir.x = 0;
        targetDir.y = 0;

        this.transform.rotation = Quaternion.LookRotation(targetDir);
    }
}
