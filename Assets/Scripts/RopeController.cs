using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    [SerializeField]
    int segmentAmount;

    [SerializeField]
    Transform finalDestination;

    [SerializeField]
    GameObject segment;

    [SerializeField]
    Rigidbody firstAnchor;

    public Transform[] segmentPositions;
    LineRenderer line;
    // Start is called before the first frame update
    void Start()
    {
        //Set the first point and list size.
        Rigidbody lastPoint = firstAnchor;
        segmentPositions = new Transform[segmentAmount + 1];
        segmentPositions[0] = firstAnchor.transform;

        //Now, spawn segments and set up their joints to each other.
        for (int i = 1; i < segmentAmount + 1; i++)
        {
            //Get the position of current point and add on the segment distance.
            Vector3 newPos = new Vector3(firstAnchor.transform.position.x + (finalDestination.transform.position.x - firstAnchor.transform.position.x) * (((float)i + 1) / (float)segmentAmount),
                                         firstAnchor.transform.position.y + (finalDestination.transform.position.y - firstAnchor.transform.position.y) * (((float)i + 1) / (float)segmentAmount),
                                         firstAnchor.transform.position.z + (finalDestination.transform.position.z - firstAnchor.transform.position.z) * (((float)i + 1) / (float)segmentAmount));

            ConfigurableJoint joint = Instantiate(segment, newPos, Quaternion.identity, this.transform.parent).GetComponent<ConfigurableJoint>();

            joint.connectedBody = lastPoint;
            joint.projectionDistance = 0.01f;
            joint.projectionAngle = 0;

            //Reset the last point, and iterate.
            lastPoint = joint.gameObject.GetComponent<Rigidbody>();

            //Add joint to segments.
            segmentPositions[i] = joint.transform;
        }

        //Set the final destination to be the child of the cable point.
        finalDestination.transform.parent = lastPoint.transform;
        finalDestination.transform.localPosition = Vector3.zero;

        //Now, generate a line between the first anchor to final destination.
        line = GetComponent<LineRenderer>();
        line.positionCount = segmentPositions.Length;
    }

    // Update is called once per frame
    void Update()
    {
        setLinePositions();
    }

    void setLinePositions()
    {
        for (int i = 0; i < segmentPositions.Length; i++)
        {
            line.SetPosition(i, segmentPositions[i].position);
        }
    }
}
