using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : ButtonClass
{
    WorldStateManager worldManager;
    //A function to use the save state functionality.
    public override void useButton()
    {
        if (!worldManager)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("GameManager");
            worldManager = obj.GetComponent<WorldStateManager>();
        }

        worldManager.saveNewState();
    }

    public void saveCurrentOptions(bool b)
    {
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>())
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>().saveControls(b);
        }
    }
}
