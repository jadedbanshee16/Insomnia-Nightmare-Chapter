using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindChapterWorldShiftScript : MonoBehaviour
{
    [SerializeField]
    GameObject[] levels;

    public void setLevels(int i)
    {
        if(i > levels.Length)
        {
            i = levels.Length;
        }

        for(int v = 0; v < i; v++)
        {
            levels[v].SetActive(true);
        }
    }
}
