using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComputerClass : EnergyObjectClass
{
    //To manage the screens, will use a menu manager.
    [SerializeField]
    MenuManager screenManager;

    [SerializeField]
    private GameObject[] affectedObj;

    private string currentScreen;


    //Text for when this is offline.
    [SerializeField]
    TextMeshProUGUI tex_;
    [SerializeField]
    private MeshRenderer masterScreen;
    [SerializeField]
    private string offText;
    [SerializeField]
    private Material screenMatOn;
    [SerializeField]
    private Material screenMatOff;

    //Change the screen.
    public void changeScreens(string ind, bool keepScreen)
    {
        if (keepScreen)
        {
            currentScreen = ind;
        }
        screenManager.setToMenuGroup(ind);
    }

    //Turn on a given affectedObject.
    public void switchAffectedObject(int index, int offset, bool b)
    {
        if (index < affectedObj.Length && affectedObj[index] != null && affectedObj[index].GetComponent<EnergyObjectClass>())
        {
            energyManager.updateObject(affectedObj[index].GetComponent<EnergyObjectClass>(), b);
            //screenObject.displayText(messages[comparedCode], isPowered, isOn);
        }
    }

    public void switchAllObjects(bool b)
    {
        //Start all objects in off.
        for (int i = 0; i < affectedObj.Length; i++)
        {
            if (affectedObj[i] != null && affectedObj[i].GetComponent<EnergyObjectClass>())
            {
                energyManager.updateObject(affectedObj[i].GetComponent<EnergyObjectClass>(), b);
            }
        }
    }

    //What happens when this object is used.
    //This should work when object is powered or isOn.
    public override void useObject()
    {
        if (isPowered && isOn)
        {
            tex_.text = "";
            masterScreen.material = screenMatOn;
            changeScreens("Off", true);
        } else
        {
            tex_.text = offText;
            masterScreen.material = screenMatOff;
            changeScreens(currentScreen, false);
        }
    }

    public override void setIsOn(bool b)
    {
        //Force isOn to always be true.
        isOn = true;
    }

    public override void setEnergyManager(GridManager man)
    {
        energyManager = man;

        //When manager is set, set on depending on energyObject type. Default is off.
        //isOn = false;

        //Set any components that needed to be made.
        controller = GetComponent<InteractionControlClass>();
    }


}
