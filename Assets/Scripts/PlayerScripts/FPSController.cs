using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    private enum playerStatus
    {
        idle,
        crouch,
        walk,
        run,
        die
    }


    [SerializeField]
    private Vector3 speedVariations;
    private float moveSpeed;

    [SerializeField]
    private float reach = 0.3f;

    [SerializeField]
    private Vector2 colliderSizes;
    private float colliderSize;

    [SerializeField]
    private FPSCamera m_camera;
    [SerializeField]
    private GameObject hitBox;
    private CapsuleCollider m_collider;
    private Rigidbody m_rig;

    [SerializeField]
    private playerStatus currentStatus;

    [SerializeField]
    private Transform playerHand;
    [SerializeField]
    private Transform playerHead;
    public HoldInteractionClass holdingItem;

    public GameObject lockingObject;

    private float interactionCooldown = 0.5f;
    private float interactionTimer = 0;

    private bool movementLocked;
    private bool interactionLocked;

    // Start is called before the first frame update
    void Start()
    {
        m_rig = GetComponent<Rigidbody>();
        m_collider = GetComponent<CapsuleCollider>();
        colliderSize = colliderSizes.y;

        currentStatus = playerStatus.idle;
        //Set the default move speed.
        moveSpeed = speedVariations.y;

        movementLocked = false;
        interactionLocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Ensure movement is only moved when not locked.
        if (!movementLocked)
        {
            Move();
        }

        //Ensure interaction is only interacted when not locked.
        if (!interactionLocked)
        {
            makeInteraction();
        }

        //If holding the item, set the item to current playerHand position.
        if (holdingItem)
        {
            holdingItem.GetComponent<InteractionClass>().Interact(playerHand.position, playerHand.rotation, playerHand);
        }

        //Work with the exit input to get out of locking positions without touching the interaction.
        if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Mouse0))
        {
            if (lockingObject && lockingObject.GetComponent<PlayerControlInteractionClass>())
            {
                if (interactionTimer == 0)
                {
                    interactionTimer = interactionCooldown;
                    lockingObject.GetComponent<InteractionClass>().Interact(this.gameObject);
                }
            }
        }
    }

    //Keep all player movement.
    private void Move()
    {
        //Get an input from keyboard. If so, make a move. For forward and back
        if (Input.GetKey(KeyCode.W))
        {
            m_rig.position += transform.forward * Time.deltaTime * moveSpeed;
            currentStatus = playerStatus.walk;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            m_rig.position -= transform.forward * Time.deltaTime * moveSpeed;
            currentStatus = playerStatus.walk;
        }
        //Get input from keyboard. Id so, make a move. For left and right.
        if (Input.GetKey(KeyCode.A))
        {
            m_rig.position -= transform.right * Time.deltaTime * moveSpeed;
            currentStatus = playerStatus.walk;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            m_rig.position += transform.right * Time.deltaTime * moveSpeed;
            currentStatus = playerStatus.walk;
        }

        //Create the crouch buttons.
        if (Input.GetKey(KeyCode.LeftControl))
        {
            moveSpeed = speedVariations.x;
            currentStatus = playerStatus.crouch;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            currentStatus = playerStatus.walk;
        }

        //Set up the running.
        if (Input.GetKey(KeyCode.LeftShift) && currentStatus != playerStatus.crouch)
        {
            moveSpeed = speedVariations.z;
            currentStatus = playerStatus.run;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = speedVariations.y;
            currentStatus = playerStatus.walk;
        }

        if (currentStatus == playerStatus.crouch && colliderSize > colliderSizes.x)
        {
            colliderSize -= Time.fixedDeltaTime;
        }

        if (currentStatus != playerStatus.crouch && colliderSize < colliderSizes.y)
        {
            colliderSize += Time.fixedDeltaTime;
        }

        m_camera.changeHeadBob((int)currentStatus);

        m_collider.height = colliderSize;
    }

    //Keep all player interaction.
    private void makeInteraction()
    {
        //Go through interaction timer if there is an interaction.
        if (interactionTimer == 0)
        {

            //Make the controls for the mouse button.
            if (Input.GetKey(KeyCode.Mouse0))
            {
                isInInteraction();

                //This is to ensure that if camera and player is locked, and player cannot interact with the lockingobject again,
                //then this will force an interaction if the click is ever made.
                /*if (lockingObject && lockingObject.GetComponent<PlayerControlInteractionClass>())
                {
                    lockingObject.GetComponent<InteractionClass>().Interact(this.gameObject);
                }*/
            }

            //Controls for dropping items in held hand.
            if (Input.GetKey(KeyCode.G) && holdingItem)
            {
                holdingItem.Interact(frontPosition(), Quaternion.identity, null);
                holdingItem.removeHeld();
                removeHeldItem();
            }
        }
        else
        {
            //If interaction made, then run cooldown until the timer is 0.
            interactionTimer -= Time.deltaTime;

            if (interactionTimer < 0)
            {
                interactionTimer = 0;
            }
        }

        //If currently locked onto an object, attempt to find an input. If an input is found, then put string input character into that input class.
        if (lockingObject && lockingObject.GetComponent<InputInteractionClass>())
        {
            //Get input from user.
            string input = Input.inputString;

            if (!String.Equals(input, ""))
            {
                lockingObject.GetComponent<InputInteractionClass>().setInput(input);
                lockingObject.GetComponent<InputInteractionClass>().Interact();
            }
        }
    }

    //Check if reaction is possible. If so, then make interaction.
    void isInInteraction()
    {
        RaycastHit hitPoint;

        //If the ray hits something within reach, see if it has button. If so, complete whatever interaction button is set.
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitPoint, reach))
        {
            //Debug.Log(hitPoint.collider.gameObject.name);
            //Check if an item first.
            /*if (hitPoint.collider.CompareTag("HandItem"))
            {
                //Figure out if the interaction should move object to right hand or closer to the player face.
                if (hitPoint.collider.GetComponent<InteractionClass>().isInteractionType(InteractionClass.interactionType.player))
                {

                    //If this, then move the player head to starting position on the hitpoint transform.
                    playerHead.position = hitPoint.transform.position;

                    hitPoint.collider.GetComponent<InteractionClass>().Interact(this.transform, playerHead);
                } else
                {
                    hitPoint.collider.GetComponent<InteractionClass>().Interact(this.transform, playerHand);
                }

                if (holdingItem)
                {
                    //holdingItem.GetComponent<HoldItemClass>().release(this.transform);
                }

                holdingItem = hitPoint.collider.gameObject;

                interactionTimer = interactionCooldown;

            //If interaction with something that holds items.
            } else if (hitPoint.collider.CompareTag("ItemPort"))
            {
                //Ensure an item is held.
                if (holdingItem)
                {
                    hitPoint.collider.GetComponent<InteractionClass>().Interact(hitPoint.transform, holdingItem.transform);

                    interactionTimer = interactionCooldown;
                }

            //If nothing else, see if item can be interacted to.
            } else*/

            //This is for interactions with holdable items.
            if (hitPoint.collider.GetComponent<HoldInteractionClass>())
            {
                interactionTimer = interactionCooldown;

                if (!holdingItem)
                {
                    //Ensure item can be touched by the player.
                    if (hitPoint.collider.GetComponent<InteractionClass>().isInteractionType(InteractionClass.interactionType.player))
                    {
                        //Make the interact happen.
                        hitPoint.collider.GetComponent<InteractionClass>().Interact(playerHand.position, playerHand.rotation, playerHand);
                        setHeldItem(hitPoint.collider.GetComponent<HoldInteractionClass>());
                    }
                }

                //This is interactions with position items that can hold holdable items.
            }
            else if (hitPoint.collider.GetComponent<PositionInteractionClass>())
            {
                interactionTimer = interactionCooldown;

                //Make the interact happen.
                if (holdingItem)
                {
                    if (hitPoint.collider.GetComponent<PositionInteractionClass>().canHoldItem(holdingItem.gameObject))
                    {
                        hitPoint.collider.GetComponent<InteractionClass>().Interact(holdingItem.gameObject);
                        removeHeldItem();
                    }
                }

            //Interactions that change the player controller.
            } else if (hitPoint.collider.GetComponent<PlayerControlInteractionClass>())
            {
                interactionTimer = interactionCooldown;

                //Ensure item can be touched by the player.
                if (hitPoint.collider.GetComponent<InteractionClass>().isInteractionType(InteractionClass.interactionType.player))
                {
                    //Make the interact happen.
                    hitPoint.collider.GetComponent<InteractionClass>().Interact(this.gameObject);
                }

                //This is for interactions with everything else that doesn't deal with the hold or position systems.
            } else if (hitPoint.collider.GetComponent<InteractionClass>())
            {
                interactionTimer = interactionCooldown;

                //Ensure item can be touched by the player.
                if (hitPoint.collider.GetComponent<InteractionClass>().isInteractionType(InteractionClass.interactionType.player))
                {
                    //Make the interact happen.
                    hitPoint.collider.GetComponent<InteractionClass>().Interact();
                }
            }
        }
    }

    //A function to return the current held item if any.
    public HoldInteractionClass getHeldItem()
    {
        return holdingItem;
    }

    //A function to make holding item equal to an object.
    private void setHeldItem(HoldInteractionClass obj)
    {
        holdingItem = obj;
    }

    //This removes held item and sets the held item to it's unheld state.
    private void removeHeldItem()
    {
        holdingItem = null;
    }

    //A function that will look for the next closest position, either within reach or when it hits something.
    private Vector3 frontPosition()
    {
        Vector3 trans = playerHead.position;

        RaycastHit hitPoint;
        Ray hitRay = new Ray(trans, playerHead.forward);

        //If the ray hits something within reach, see if it hits an object.
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitPoint, reach))
        {
            trans = hitRay.GetPoint(hitPoint.distance);
        } else
        {
            trans = hitRay.GetPoint(reach / 2);
        }


        return trans;
    }

    //A function to set the locked section.
    public void setLock(bool b, GameObject obj)
    {
        movementLocked = b;

        lockingObject = obj;
    }

    public bool getLock()
    {
        return movementLocked;
    }
}
