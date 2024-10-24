using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public void loadScene(int levelInd)
    {
        //Create a coroutine to do something while loading.
        StartCoroutine(LoadSceneAsyncOperation(levelInd));

    }

    IEnumerator LoadSceneAsyncOperation(int levelInd)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(levelInd);

        while (!op.isDone)
        {
            Debug.Log(op.progress);
            yield return null;
        }
    }
}
