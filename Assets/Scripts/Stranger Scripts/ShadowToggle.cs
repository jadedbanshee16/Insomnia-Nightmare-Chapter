using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowToggle : MonoBehaviour
{
    private float timer;

    private Quaternion oldRot;
    private Quaternion newRot;

    PoolObject obj;

    // Start is called before the first frame update
    void Start()
    {
        timer = 1;

        oldRot = this.transform.rotation;

        Vector3 dir = Random.insideUnitSphere;

        newRot = Quaternion.LookRotation(dir, Vector3.up);

        obj = GetComponent<PoolObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            this.transform.rotation = Quaternion.Lerp(newRot, oldRot, timer);
        } else
        {
            timer = 1;
            obj.findManager().makeInactiveFromPool(obj.getIndex());
        }
    }
}
