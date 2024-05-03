using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPortClass : ItemPositionClass
{
    [SerializeField]
    EnergizerScript powerPort;

    [SerializeField]
    float energyProvided = 0;

    private void Start()
    {
        powerPort = GetComponent<EnergizerScript>();
    }

    public override void interact(Transform holder, Transform obj)
    {

        //If the added obj is a permitted object, then set new obj to new position.
        if (string.Equals(obj.gameObject.name, permittedItem))
        {
            obj.gameObject.GetComponent<InteractionClass>().interact(this.transform, holdPosition);
            obj.gameObject.GetComponent<HoldItemClass>().forceAddColliders();

            hasItem = true;

            //Power up the current powerport.
            powerPort.setPowersource(energyProvided, true);
        }
    }

    public override void removeHeldItem()
    {
        hasItem = false;
        powerPort.setPowersource(energyProvided, false);
    }
}
