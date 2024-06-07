using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugClass : HoldItemClass
{

    [SerializeField]
    RopeController _cable;

    public override void updateSubClass(bool isFixedPosition)
    {
        if (!fixedPosition)
        {
            _body.isKinematic = false;
        } else
        {
            _body.isKinematic = true;
        }
        testRope();
    }

    //A function to test the rope position to make it not stretch.
    private void testRope()
    {
        if (_cable.testLinePositions())
        {
            //Debug.Log("Cable Fail");
            release(currentHolder);
        }
    }

    /*public override void forceAddColliders()
    {
        _cable.addColliders();
    }*/
}
