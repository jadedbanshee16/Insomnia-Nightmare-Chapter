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

    [SerializeField]
    private bool FreezeXPosition;
    [SerializeField]
    private bool FreezeYPosition;
    [SerializeField]
    private bool FreezeZPosition;

    //bool away = false;

    // Update is called once per frame
    void Update()
    {
        Vector3 positions = obj.position;

        if (Vector3.Distance(this.transform.position, positions) > dist){
            if (FreezeXPosition)
            {
                positions.x = transform.position.x;
            }
            if (FreezeYPosition)
            {
                positions.y = transform.position.y;
            }
            if (FreezeZPosition)
            {
                positions.z = transform.position.z;
            }

            //Make speed faster the further you are away.
            float spd = Mathf.InverseLerp(0, speed, Vector3.Distance(this.transform.position, positions));


            //Get the direction between the 2 objects and move this transform in that direction.
            this.transform.position += (positions - this.transform.position).normalized * Time.deltaTime * spd;
            this.transform.rotation = obj.rotation;
        }
    }
}
