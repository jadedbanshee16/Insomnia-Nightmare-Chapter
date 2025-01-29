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

    [SerializeField]
    float breakThreshold;

    [SerializeField]
    private float distance;

    private Transform[] segmentPositions;
    LineRenderer line;
    // Start is called before the first frame update
    void Start()
    {
        //So that plug will not go flying in the first place.
        finalDestination.GetComponent<Rigidbody>().isKinematic = true;

        //Set the first point and list size.
        Rigidbody lastPoint = firstAnchor;
        segmentPositions = new Transform[segmentAmount + 2];
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

            joint.anchor = new Vector3(distance, 0, 0);

            //Reset the last point, and iterate.
            lastPoint = joint.gameObject.GetComponent<Rigidbody>();

            //Add joint to segments.
            segmentPositions[i] = joint.transform;
        }

        //Set the final destination to be the child of the cable point.
        finalDestination.gameObject.GetComponent<ConfigurableJoint>().connectedBody = lastPoint;
        segmentPositions[segmentPositions.Length - 1] = finalDestination.transform;

        //Now, generate a line between the first anchor to final destination.
        line = GetComponent<LineRenderer>();
        line.positionCount = segmentPositions.Length;

        //Wait a certain amount of time before releasing the plug head so that the cable has time to settle.
        StartCoroutine(WaitAndRelease(0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        setLinePositions();
    }

    //A function to set new line positions for the line renderer.
    void setLinePositions()
    {
        for (int i = 0; i < segmentPositions.Length; i++)
        {
            line.SetPosition(i, segmentPositions[i].position);
        }
    }

    //Test the new line to ensure all items aren't too far away from each other.
    public bool testLinePositions()
    {
        Vector3 currentPos = segmentPositions[0].position;

        for(int i = 1; i < segmentPositions.Length; i++)
        {
            if(Vector3.Distance(currentPos, segmentPositions[i].position) > breakThreshold)
            {
                return true;
            }

            currentPos = segmentPositions[i].position;
        }

        return false;
    }

    private IEnumerator WaitAndRelease(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            if(finalDestination.GetComponent<HoldInteractionClass>() && !finalDestination.GetComponent<HoldInteractionClass>().getCurrentHolder())
            {
                finalDestination.GetComponent<Rigidbody>().isKinematic = false;
            }
            StopCoroutine(WaitAndRelease(waitTime));
        }
    }

    //Function to make all balls have no colliders while held.
    /*public void removeColliders()
    {
        for (int i = 1; i < segmentPositions.Length - 1; i++)
        {
            segmentPositions[i].gameObject.GetComponent<SphereCollider>().enabled = false;
        }
    }

    //Function to return colliders to cable chain.
    public void addColliders()
    {
        for (int i = 1; i < segmentPositions.Length - 1; i++)
        {
            segmentPositions[i].gameObject.GetComponent<SphereCollider>().enabled = true;
        }
    }*/
}
