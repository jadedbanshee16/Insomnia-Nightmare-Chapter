using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionControlClass))]
public class SensorInteractionClass : PositionInteractionClass
{
    public override void Interact()
    {
        //Do not make an interaction. This is not how sensors are used.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasPermission(interactionType.senserInteraction) && other.GetComponent<HoldInteractionClass>() && canHoldItem(other.GetComponent<HoldInteractionClass>(), true) &&
                  GetComponentInChildren<AchievementScript>())
        {
            //If true, then doesn't need to set any objects. Just set the connected object.
            GetComponentInChildren<AchievementScript>().triggerAchievement();
            //Debug.Log("Cleaning up");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        bool isOther = false;
        if (currentHeldItem)
        {
            isOther = currentHeldItem.gameObject.GetInstanceID() == other.gameObject.GetInstanceID();
        }

        if (isOther && currentHeldItem.GetComponent<HoldInteractionClass>() && currentHeldItem.GetComponent<HoldInteractionClass>().getType() == interactionType.senserInteraction)
        {
            if (connectedObject.GetComponent<LockObjectClass>() && !connectedObject.GetComponent<InvertedLockObjectClass>())
            {
                connectedObject.GetComponent<EnergyObjectClass>().setIsOn(false);
                connectedObject.GetComponent<EnergyObjectClass>().useObject();
            }

            currentHeldItem.removeHeld();
            setCurrentHeldItem(null);
        }
    }
}
