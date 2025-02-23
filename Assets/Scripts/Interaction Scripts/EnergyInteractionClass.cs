using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This is for interactions that will affect turning off and on an energy object in a grid.

Note: This takes another energy screen object as an option that would provide the ability to lock the set object based on the input on the screen.
*/
public class EnergyInteractionClass : InteractionClass
{
    private GridManager powerManager;

    [SerializeField]
    private GridManager seperateGrid;
    [SerializeField]
    private EnergyObjectClass seperateSlot;

    [SerializeField]
    bool isOn = false;

    [SerializeField]
    EnergyObjectClass obj;

    // Start is called before the first frame update
    void Start()
    {
        //isOn = false;

        setController();

        powerManager = GetComponentInParent<GridManager>();
    }

    //This interaction will take the object connect to this and attempt to use it as well as the base controller stuff.
    public override void Interact()
    {
        isOn = !isOn;

        if (!controller)
        {
            setController();

        }

        //This is to function either with a switch, button or indication.
        //Set the animation of the controller.
        controller.setAnimation("Pressed");

        controller.setAnimation("isOn", isOn);

        controller.setIndicator(isOn);

        controller.playInteractionAudio(0);

        setObject();
    }

    //Update power manager with the new system.
    public void setObject()
    {
        if (!powerManager)
        {
            powerManager = GetComponentInParent<GridManager>();
        }

        if (powerManager && obj)
        {
            //Debug.Log("React1: " + isOn + " | " + obj.name);
            powerManager.updateObject(obj, isOn);
        }

        //See if this switch is currently working with a seperate object, due to power points.
        if (seperateGrid && seperateSlot)
        {
            seperateGrid.updateObject(seperateSlot, isOn);
        }
    }

    public override void secondaryInteract()
    {
        controller.playInteractionAudio(2);
    }

    //Turn off the object and set controller to the false position.
    public void turnOffObject()
    {
        isOn = false;

        controller.setAnimation("isOn", isOn);

        controller.setIndicator(isOn);
    }

    public bool getIsOn()
    {
        return isOn;
    }

    public void setIsOn(bool b)
    {
        isOn = b;
    }

    public void setPuppetManager(GridManager man, EnergyObjectClass c)
    {
        seperateGrid = man;
        seperateSlot = c;
    }
}
