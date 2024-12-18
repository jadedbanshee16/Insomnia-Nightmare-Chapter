using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerClass : EnergyObjectClass
{
    //To manage the screens, will use a menu manager.
    [SerializeField]
    MenuManager screenManager;

    [SerializeField]
    private GameObject[] affectedObj;

    //Change the screen.
    public void changeScreens(string ind)
    {
        screenManager.setToMenuGroup(ind);
    }

    public void validateAffectedObject()
    {
        for (int i = 0; i < affectedObj.Length; i++)
        {
            if (affectedObj[i] != null && affectedObj[i].GetComponent<EnergyObjectClass>())
            {
                energyManager.updateObject(affectedObj[i].GetComponent<EnergyObjectClass>(), false);
            }
        }
    }


}
