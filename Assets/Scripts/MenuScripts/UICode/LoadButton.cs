using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadButton : ButtonClass
{
    [SerializeField]
    string level;

    LevelManager levelManager_;

    public override void useButton()
    {
        if (!levelManager_)
        {
            levelManager_ = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LevelManager>();
        }
        //Find the LevelManager and use it to load the specific level
        if (levelManager_)
        {
            level = levelManager_.retrieveLevel(level);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(level);
    }
}
