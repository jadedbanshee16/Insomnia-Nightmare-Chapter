using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTriggerScript : MonoBehaviour
{
    [SerializeField]
    Vector3 throwDirection;

    [SerializeField]
    float throwStrength;

    [SerializeField]
    Transform teleportPosition;


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HoldInteractionClass>())
        {
            if(other.GetComponent<Rigidbody>() && !other.GetComponent<Rigidbody>().isKinematic)
            {
                if(throwStrength > 0)
                {
                    other.GetComponent<Rigidbody>().linearVelocity = throwDirection * throwStrength;
                } else
                {
                    other.gameObject.transform.position = teleportPosition.position;
                }
            }
        }
    }
}
