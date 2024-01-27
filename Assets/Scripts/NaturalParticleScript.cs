using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturalParticleScript : MonoBehaviour
{
    public float timer = 5;

    // Update is called once per frame
    void Update()
    {
        //Iterate down until 0 or -;
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        } else
        {
            delete();
        }
    }

    void delete()
    {
        Destroy(this.gameObject);
    }
}
