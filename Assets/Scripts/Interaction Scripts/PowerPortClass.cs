using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPortClass : ItemPositionClass
{
    EnergizerScript powerPort;

    [SerializeField]
    GameObject currentObj;

    public override void interact(Transform holder, Transform obj)
    {
        currentObj = obj.gameObject;
        //If the added obj is a permitted object, then set new obj to new position.
        if (string.Equals(currentObj.name, permittedItem))
        {
            //Set the given plug class to the new position.
            currentObj.GetComponent<InteractionClass>().interact(this.transform, holdPosition);
            //obj.gameObject.GetComponent<HoldItemClass>().forceAddColliders();

            hasItem = true;

            //Check if plug. So that we don't access the plug class when it is not a plug.
            if (currentObj.GetComponent<PlugClass>())
            {
                //Power up the current powerport.
                powerPort.setPowersource(currentObj.GetComponent<PlugClass>().getMainObject());
            }

        }
    }

    public override void removeHeldItem()
    {
        hasItem = false;
        powerPort.setPowersource(null);
        currentObj.GetComponent<PlugClass>().getMainObject().unSetGrid();
        currentObj = null;

    }

    public void setPowerPort(EnergizerScript script) 
    {
        powerPort = script;
    }
}
