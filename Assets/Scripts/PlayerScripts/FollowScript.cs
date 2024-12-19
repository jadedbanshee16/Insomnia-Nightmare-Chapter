using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{
    [SerializeField]
    Transform obj;

    [SerializeField]
    float dist;

    public float speed = 0;

    //bool away = false;

    // Update is called once per frame
    void Update()
    {
        //Make speed faster the further you are away.
        speed = Mathf.InverseLerp(0, 0.1f, Vector3.Distance(this.transform.position, obj.position));


        //Get the direction between the 2 objects and move this transform in that direction.
        this.transform.position += (obj.position - this.transform.position).normalized * Time.deltaTime * speed;
        this.transform.rotation = obj.rotation;
    }
}
