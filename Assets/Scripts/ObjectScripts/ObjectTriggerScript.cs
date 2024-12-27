using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTriggerScript : MonoBehaviour
{
    [SerializeField]
    Vector3 throwDirection;

    [SerializeField]
    float throwStrength;


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HoldInteractionClass>())
        {
            if(other.GetComponent<Rigidbody>() && !other.GetComponent<Rigidbody>().isKinematic)
            {
                other.GetComponent<Rigidbody>().velocity = throwDirection * throwStrength;
            }
        }
    }
}
