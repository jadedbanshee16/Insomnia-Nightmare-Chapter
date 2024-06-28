using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugClass : HoldItemClass
{

    [SerializeField]
    RopeController _cable;

    [SerializeField]
    GeneratorClass obj_;

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

    //This works only for generators for now.
    public void setMainObject(GeneratorClass gen)
    {
        obj_ = gen;
    }

    public GeneratorClass getMainObject()
    {
        return obj_;
    }

    //Base interaction of the interaction class.
    public override void interact(Transform holder, Transform newPos)
    {
        if (currentHolder != null)
        {
            //If current holder, find either player or held item class to remove item from that point.
            release(currentHolder);
        }

        //Debug.Log("Working?");
        fixedPosition = true;

        newPosition = newPos;



        currentHolder = holder;
    }



    /*public override void forceAddColliders()
    {
        _cable.addColliders();
    }*/
}
