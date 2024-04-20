using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRope : MonoBehaviour
{
    [SerializeField]
    int segmentAmount;

    [SerializeField]
    Transform finalDestination;

    [SerializeField]
    GameObject segment;

    [SerializeField]
    Rigidbody firstAnchor;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody lastPoint = firstAnchor;

        //Now, spawn segments and set up their joints to each other.
        for (int i = 0; i < segmentAmount; i++)
        {


            //Get the position of current point and add on the segment distance.
            Vector3 newPos = new Vector3(firstAnchor.transform.position.x + (finalDestination.transform.position.x - firstAnchor.transform.position.x) * (((float)i + 1) / (float)segmentAmount),
                                         firstAnchor.transform.position.y,
                                         firstAnchor.transform.position.z);

            ConfigurableJoint joint = Instantiate(segment, newPos, Quaternion.identity, this.transform).GetComponent<ConfigurableJoint>();

            joint.connectedBody = lastPoint;
            joint.projectionDistance = 0.01f;
            joint.projectionAngle = 0;

            //Reset the last point to current point.
            lastPoint = joint.gameObject.GetComponent<Rigidbody>();


        }

        //Set the final destination to be the child of the cable point.
        finalDestination.transform.parent = lastPoint.transform;
    }
}
