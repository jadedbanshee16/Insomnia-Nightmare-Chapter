using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField]
    List<PoolObject> pool;

    [SerializeField]
    GameObject pooledObject;

    [SerializeField]
    string poolID;

    [SerializeField]
    int poolMaximum;

    [Header("Master time controls")]
    [SerializeField]
    bool useMasterTimer;

    [SerializeField]
    float masterTime;
    [SerializeField]
    float masterTimer;

    public void Start()
    {
        if(poolMaximum < 0)
        {
            poolMaximum = 1000;
        }

        pool = new List<PoolObject>();

        if (useMasterTimer)
        {
            masterTimer = 0;
        }
    }

    public void Update()
    {
        if (useMasterTimer)
        {
            if(masterTimer > 0)
            {
                masterTimer -= Time.deltaTime;
            } else
            {
                for(int i = 0; i < pool.Count; i++)
                {
                    makeInactiveFromPool(i);
                }
            }
        }
    }

    //Set the current object 
    public void makeInactiveFromPool(int i)
    {
        if(i < pool.Count && i >= 0)
        {
            pool[i].gameObject.SetActive(false);
        }
    }

    public void makeActiveFromPool(Vector3 pos, Quaternion rot)
    {
        //If using master time, then thgis is to reset the timer upon making a new item.
        if (useMasterTimer)
        {
            masterTimer = masterTime;
        }

        //Go through pool and see if there is an inactive point.
        int i = 0;
        while(i < pool.Count)
        {
            if (pool[i].gameObject.activeInHierarchy == false)
            {
                pool[i].gameObject.SetActive(true);
                pool[i].transform.position = pos;
                pool[i].transform.rotation = rot;
                return;
            }

            i++;
        }

        //If you got this far, then there are no inactive points in the pool. Either add more or use maximum.
        if(pool.Count < poolMaximum)
        {
            addToPool(pos, rot);
        }
    }

    public void addToPool(Vector3 pos, Quaternion rot)
    {
        if (pooledObject)
        {
            GameObject g = Instantiate(pooledObject, pos, rot);

            PoolObject pObj = g.GetComponent<PoolObject>();

            pObj.setIndex(pool.Count);

            pool.Add(pObj);
        }
    }

    public string getPoolID()
    {
        return poolID;
    }

    public bool getIsUsingMasterTime()
    {
        return useMasterTimer;
    }

    public float getTimer()
    {
        return masterTimer;
    }

    public float getTime()
    {
        return masterTime;
    }
}
