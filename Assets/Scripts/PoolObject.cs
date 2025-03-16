using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PoolObject : MonoBehaviour
{
    [SerializeField]
    int objectIndex;

    [SerializeField]
    string objectID;

    public int getIndex()
    {
        return objectIndex;
    }

    public void setIndex(int i)
    {
        objectIndex = i;
    }

    public PoolManager findManager()
    {
        PoolManager[] managers = FindObjectsByType<PoolManager>(FindObjectsSortMode.None);

        int i = 0;

        while(i < managers.Length)
        {
            if(string.Equals(managers[i].getPoolID(), objectID))
            {
                return managers[i];
            }

            i++;
        }

        return null;
    }
}
