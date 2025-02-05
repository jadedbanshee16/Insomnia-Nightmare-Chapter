using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    List<GameObject> pool;

    public void makeInactiveFromPool(int i)
    {
        pool[i].SetActive(false);
    }

    public void addToPool(GameObject g, Vector3 pos, Quaternion rot)
    {
        //Check if there is space in pool to add the new object.
        int i = 0;
        int foundPos = -1;

        while(i < pool.Count)
        {
            //Ensure that this is calling the right object.
            if(string.Equals(pool[i].name, g.name))
            {

            }
        }
    }
}
