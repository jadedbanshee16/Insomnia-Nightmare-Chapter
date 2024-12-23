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

    [SerializeField]
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

        if (screenManager)
        {
            //Debug.Log("Worked: " + currentScreen);
            screenManager.setToMenuGroup(ind);
        }
    }

    //Turn on a given affectedObject.
    public void switchAffectedObject(int index, int offset, bool b)
    {
        if (index < affectedObj.Length && affectedObj[index + offset] != null && affectedObj[index + offset].GetComponent<EnergyObjectClass>())
        {
            //Debug.Log(index + offset);
            energyManager.updateObject(affectedObj[index + offset].GetComponent<EnergyObjectClass>(), b);
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
        //Debug.Log("Worked");

        if (isPowered && isOn)
        {
            tex_.text = "";
            masterScreen.SetMaterials(new List<Material>() { masterScreen.material, screenMatOn });
            changeScreens(currentScreen, false);
        } else
        {
            //Debug.Log("Worked 2");
            tex_.text = offText;
            masterScreen.SetMaterials(new List<Material>() { masterScreen.material, screenMatOff});
            changeScreens("Off", false);
        }
    }

    public override void powerObject(bool b)
    {
        if (energyUsage == 0)
        {
            isPowered = true;
        }
        else
        {
            isPowered = b;
        }

        //For this object only, update this object.
        useObject();

    }

    public override void setIsOn(bool b)
    {
        //Force isOn to always be true.
        isOn = true;
    }

    public override void forceIsOn(bool b)
    {
        //Force to on always, as computers cannot be shut off at the moment, only denied power.
        amountOn = amountInteractionsNeeded;
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
