using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawRayScript : MonoBehaviour
{
    [SerializeField]
    Transform other;

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(this.transform.position, other.position - this.transform.position, Color.white, 1f);
    }
}
