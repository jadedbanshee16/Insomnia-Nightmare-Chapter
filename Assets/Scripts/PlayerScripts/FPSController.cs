using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    private enum playerStatus
    {
        idle,
        walk,
        crouch,
        run,
        sit,
        die
    }

    public enum controlStatus
    {
        fullControl,
        noMovement,
        mouseOnlyConfined,
        UIOnly,
        noControl
    }

    [SerializeField]
    private float playerID;

    [HeaderAttribute("Character Attributes")]
    [SerializeField]
    private Vector3 speedVariations;
    private float moveSpeed;
    [SerializeField]
    private Vector3 gravity;

    [SerializeField]
    private float reach = 0.3f;

    [SerializeField]
    private Vector2 colliderSizes;
    [SerializeField]
    private Vector3[] camYPositions;
    private float colliderSize;

    [HeaderAttribute("Player Objects")]
    [SerializeField]
    private FPSCamera m_camera;
    [SerializeField]
    private GameObject hitBox;
    private MenuManager menu_;
    private CapsuleCollider m_collider;
    private CharacterController m_controller;
    private Rigidbody m_rig;
    private InventoryScript invent_;
    private Animator anim_;
    [SerializeField]
    private Transform camPos;
    [SerializeField]
    private playerStatus currentStatus;
    private controlStatus currentControl;

    [SerializeField]
    private Transform playerHand;
    [SerializeField]
    private Transform playerHead;
    [SerializeField]
    private HoldInteractionClass holdingItem;
    private GameObject lockingObject;
    [SerializeField]
    private InputInteractionClass inputLockObject;
    [SerializeField]
    private GameObject torchlight;

    //Audio based fields.
    private AudioManager audioManager_;
    private AudioSource playerSource;
    int walkingClipLength;
    int runningClipLength;

    //GameManager variables.
    private OptionsManager options;


    private float footstepTime = 1;
    private float footstepTimer = 0;

    [SerializeField]
    private float interactionCooldown = 0.5f;
    public float interactionTimer = 0;

    //This variable is used pick up items but cannot hold them without holding down the pick up button.
    public bool handLocked;

    // Start is called before the first frame update
    void Start()
    {
        //m_rig = GetComponent<Rigidbody>();
        m_controller = GetComponent<CharacterController>();
        m_rig = GetComponent<Rigidbody>();
        m_collider = GetComponent<CapsuleCollider>();
        options = GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>();
        invent_ = GetComponentInChildren<InventoryScript>();
        anim_ = GetComponentInChildren<Animator>();
        colliderSize = colliderSizes.y;

        menu_ = GetComponentInChildren<MenuManager>();

        currentStatus = playerStatus.idle;
        currentControl = controlStatus.fullControl;
        //Set the default move speed.
        moveSpeed = speedVariations.y;

        //Set the audio system.
        audioManager_ = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioManager>();
        playerSource = GetComponent<AudioSource>();

        walkingClipLength = audioManager_.getCurrentClipLength(0, 0);
        runningClipLength = audioManager_.getCurrentClipLength(0, 1);

        //Set cursor to middle of screen.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Vector3 pos = new Vector3(playerHead.localPosition.x, camTargets.position.y, playerHead.localPosition.z);
        //playerHead.position = new Vector3(camTargets.position.x, camTargets.position.y, camTargets.position.z);

        playerHead.localPosition = new Vector3(playerHead.localPosition.x, camYPositions[0].y, playerHead.localPosition.z);

        //Debug.Log("After change2: " + GameObject.FindGameObjectWithTag("Player").transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        //Run a given framerate
        //Application.targetFrameRate = 60;

        //Debug.Log("Constant change: " + GameObject.FindGameObjectWithTag("Player").transform.position);
        //Ensure movement is only moved when not locked.
        if (currentControl == controlStatus.fullControl)
        {
            Move();
        }

        //Ensure interaction is only interacted when not locked.
        if (currentControl != controlStatus.noControl)
        {
            makeInteraction();
        } else
        {
            //If interaction made, then run cooldown until the timer is 0.
            interactionTimer -= Time.deltaTime;

            if (interactionTimer < 0)
            {
                interactionTimer = 0;
            }
        }

        //If holding the item, set the item to current playerHand position.
        /*if (holdingItem && holdingItem.GetComponent<InteractionClass>())
        {
            holdingItem.GetComponent<InteractionClass>().Interact(playerHand.GetChild(0).position, playerHand.GetChild(0).rotation, playerHand);
        }*/

        if (Input.GetKeyUp(options.getControl(OptionsManager.theControls.interaction)) && handLocked && holdingItem && currentControl != controlStatus.noControl)
        {
            invent_.dropObject(frontPosition());
            holdingItem.removeHeld();
            removeHeldItem();
            handLocked = false;
        }

        //Work with the exit input to get out of locking positions without touching the interaction.
            if ((Input.GetKey(options.getControl(OptionsManager.theControls.exit)) && currentControl != controlStatus.noControl) || (currentControl == controlStatus.mouseOnlyConfined && Input.GetKey(options.getControl(OptionsManager.theControls.interaction))))
        {
            //If a locking object is found then complete path to locking object,
            if (lockingObject && lockingObject.GetComponent<PlayerControlInteractionClass>())
            {
                if (interactionTimer == 0)
                {
                    interactionTimer = interactionCooldown;
                    lockingObject.GetComponent<InteractionClass>().Interact(this.gameObject);
                }
            //If no locking object, then assume menu is opened.
            }

            if (inputLockObject)
            {
                //This is to stop player from using an inputlockingObject once exit is pressed.
                //This is outside of interaction timer because it led to issues of input was still be placed until the next click.
                inputLockObject = null;
            }
        }

        if (Input.GetKey(options.getControl(OptionsManager.theControls.exit)) && !lockingObject && currentControl != controlStatus.noControl)
        {
            if (interactionTimer == 0)
            {
                interactionTimer = interactionCooldown;

                //If menu is already open, then unlock menu.
                if (currentControl == controlStatus.UIOnly)
                {
                    setMenu(false, false);
                    //In case options had been changed, save current options.
                    //This is to ensure that menu changes will be saved if you use a button to click out of the menu, rather than selecting an apply button.
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<OptionsManager>().saveControls(false);
                }
                else
                {
                    setMenu(true, true);
                }
            }
        }

        //Turn on and off the torch.
        if(Input.GetKey(options.getControl(OptionsManager.theControls.torchControl)) && !lockingObject && currentControl != controlStatus.noControl)
        {
            if(interactionTimer == 0)
            {
                interactionTimer = interactionCooldown;

                if (torchlight.activeSelf)
                {
                    torchlight.SetActive(false);
                }
                else
                {
                    torchlight.SetActive(true);
                }
            }
        }


    }

    //Keep all player movement.
    private void Move()
    {
        bool didMove = false;

        Vector3 moveDirection = Vector3.zero;

        //Get an input from keyboard. If so, make a move. For forward and back
        if (Input.GetKey(options.getControl(OptionsManager.theControls.forward)))
        {
            moveDirection += transform.forward;
        }
        else if (Input.GetKey(options.getControl(OptionsManager.theControls.backward)))
        {
            moveDirection -= transform.forward;
        }
        //Get input from keyboard. Id so, make a move. For left and right.
        if (Input.GetKey(options.getControl(OptionsManager.theControls.left)))
        {
            moveDirection -= transform.right;
        }
        else if (Input.GetKey(options.getControl(OptionsManager.theControls.right)))
        {
            moveDirection += transform.right;
        }

        if (Vector3.Distance(moveDirection, Vector3.zero) > 0)
        {
            //this.transform.position += moveDirection.normalized * Time.deltaTime * moveSpeed;
            //m_rig.position += moveDirection.normalized * Time.deltaTime * (1f);

            //m_rig.AddForce((moveDirection.normalized * moveSpeed * 10));
            //Ensure to add gravity to the controller;
            m_controller.SimpleMove((moveDirection.normalized * moveSpeed) + gravity);
            didMove = true;
            currentStatus = playerStatus.walk;
        } else
        {
            currentStatus = playerStatus.idle;
        }

        moveSpeed = speedVariations.y;
        //currentStatus = playerStatus.walk;

        //Create the crouch buttons.
        if (Input.GetKey(options.getControl(OptionsManager.theControls.crouch)))
        {
            moveSpeed = speedVariations.x;
            currentStatus = playerStatus.crouch;

            //didMove = true;
        }
        else if (Input.GetKeyUp(options.getControl(OptionsManager.theControls.crouch)))
        {
            currentStatus = playerStatus.walk;

            footstepTimer = footstepTime;
        }

        //Set up the running.
        if (Input.GetKey(options.getControl(OptionsManager.theControls.run)) && currentStatus != playerStatus.crouch)
        {
            moveSpeed = speedVariations.z;

            if (didMove)
            {
                currentStatus = playerStatus.run;
            }

            //didMove = true;
        }
        else if (Input.GetKeyUp(options.getControl(OptionsManager.theControls.run)))
        {
            moveSpeed = speedVariations.y;
            currentStatus = playerStatus.walk;
        }


        if (currentStatus != playerStatus.crouch && colliderSize < colliderSizes.y && isNothingAbove())
        {
            colliderSize += Time.fixedDeltaTime;
            m_collider.height = colliderSize;
            m_collider.center = new Vector3(0, (colliderSize / 2), 0);
            m_controller.height = colliderSize;
            m_controller.center = new Vector3(0, (colliderSize / 2), 0);

            Vector3 newCamPos = new Vector3(playerHead.localPosition.x, camYPositions[0].y, playerHead.localPosition.z);
            playerHead.localPosition = Vector3.Lerp(playerHead.localPosition, newCamPos, Mathf.InverseLerp(colliderSizes.x, colliderSizes.y, colliderSize));
            //playerColliders[0].enabled = true;
            //playerColliders[1].enabled = false;
            //playerHead.localPosition = new Vector3(playerHead.localPosition.x, camYPositions.y, playerHead.localPosition.z);
            //playerCam.localPosition = new Vector3(playerCam.localPosition.x, 0.584f, playerCam.localPosition.z);
        } else if (!isNothingAbove())
        {
            currentStatus = playerStatus.crouch;
        }

        if (currentStatus == playerStatus.crouch && colliderSize > colliderSizes.x)
        {
            colliderSize -= Time.fixedDeltaTime;
            m_collider.height = colliderSize;
            m_collider.center = new Vector3(0, (colliderSize / 2), 0);
            m_controller.height = colliderSize;
            m_controller.center = new Vector3(0, (colliderSize / 2), 0);
            //playerColliders[0].enabled = false;
            //playerColliders[1].enabled = true;

            Vector3 newCamPos = new Vector3(playerHead.localPosition.x, camYPositions[1].y, playerHead.localPosition.z);
            playerHead.localPosition = Vector3.Lerp(playerHead.localPosition, newCamPos, Mathf.InverseLerp(colliderSizes.y, colliderSizes.x, colliderSize));

            //playerHead.localPosition = new Vector3(playerHead.localPosition.x, camYPositions.x, playerHead.localPosition.z);
            //playerCam.localPosition = new Vector3(playerCam.localPosition.x, 0.18f, playerCam.localPosition.z);
        }

        //If input makes a new direction, movement has been made.
        if (didMove)
        {
            m_camera.changeHeadBob((int)currentStatus);

            //Audio play.
            if (footstepTimer > 0)
            {
                footstepTimer -= Time.deltaTime * moveSpeed * 1.5f;
            }
            else
            {
                makeRandomFootstep();
                footstepTimer = footstepTime;
            }

            //Run the animation. Add crouch and run later.
            float forwardDot = Vector3.Dot(moveDirection.normalized, transform.forward.normalized);
            float sideDot = Vector3.Dot(moveDirection.normalized, transform.right.normalized);

            //Find the direction the player is moving and set the animation.
            //This prioritizes forward walking.
            if(forwardDot > 0.5)
            {
                anim_.SetInteger("Dir", 0);
            } else if (sideDot > 0.5)
            {
                anim_.SetInteger("Dir", 1);
            } else if (sideDot < -0.5)
            {
                anim_.SetInteger("Dir", 3);
            } else
            {
                anim_.SetInteger("Dir", 2);
            }

        } else
        {
            m_camera.changeHeadBob((int)currentStatus);
            //If an idle animation is available in this system, then use this.
            anim_.SetInteger("Dir", 4);
            footstepTimer = 0;
        }

        anim_.SetInteger("WalkState", (int)currentStatus);
    }

    //Keep all player interaction.
    private void makeInteraction()
    {
        //Go through interaction timer if there is an interaction.
        if (interactionTimer == 0)
        {

            //Make the controls for the mouse button.
            if (Input.GetKey(options.getControl(OptionsManager.theControls.interaction)) && (currentControl != controlStatus.noControl && currentControl != controlStatus.UIOnly))
            {
                //Reset the locked input.
                inputLockObject = null;
                isInInteraction();

                //This is to ensure that if camera and player is locked, and player cannot interact with the lockingobject again,
                //then this will force an interaction if the click is ever made.
                /*if (lockingObject && lockingObject.GetComponent<PlayerControlInteractionClass>())
                {
                    lockingObject.GetComponent<InteractionClass>().Interact(this.gameObject);
                }*/
            }

            //Controls for dropping items in held hand.
            if (Input.GetKey(options.getControl(OptionsManager.theControls.drop)) && !inputLockObject && currentControl == controlStatus.fullControl)
            {
                invent_.dropObject(frontPosition());

                //holdingItem.Interact(frontPosition(), Quaternion.identity, null);
                //holdingItem.removeHeld();
                removeHeldItem();
            }

            if (Input.GetKeyDown(options.getControl(OptionsManager.theControls.scrollUp)) && !handLocked && !inputLockObject && currentControl == controlStatus.fullControl)
            {
                invent_.switchObject(1);
            }

            if (Input.GetKeyDown(options.getControl(OptionsManager.theControls.scrollDown)) && !handLocked && !inputLockObject && currentControl == controlStatus.fullControl)
            {
                invent_.switchObject(-1);
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
        if (inputLockObject)
        {
            //Get input from user.
            string input = Input.inputString;

            if (!String.Equals(input, ""))
            {
                inputLockObject.setInput(input);
                inputLockObject.Interact();
            }
        }
    }

    //Check if reaction is possible. If so, then make interaction.
    void isInInteraction()
    {
        RaycastHit hitPoint;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Select an int mask for mask (3) and (12).
        int layerM = 1 << 3;
        layerM += 1 << 12;

        layerM = ~layerM;

        ///Debug.DrawRay(ray.origin, ray.direction, Color.black, 5f);

        //If the ray hits something within reach, see if it has button. If so, complete whatever interaction button is set.
        if (Physics.Raycast(ray, out hitPoint, reach, layerM))
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

            //Send a ray.
            //Debug.DrawRay(Camera.main.transform.position, ray.direction.normalized * reach, Color.cyan, Mathf.Infinity);
            //Debug.Log(hitPoint.collider.name);

            //This is for interactions with holdable items.
            if (hitPoint.collider.GetComponent<HoldInteractionClass>())
            {
                int avSlot = invent_.findAvailableSlot();

                //Debug.Log("Test for holding: " + avSlot + " | " + holdingItem);

                //Update held item to ensure that active hand is free.
                setHeldItem(invent_.getActiveHandObject());

                //Holding item is for items you MUST hold the button to hold and the avSlot is returning of there are available spots in the inventory.
                //Both must be set (available and not holdingItem) in order to interact.
                //Set up new code for holding an item.
                if (avSlot > -1)
                {
                    if (hitPoint.collider.GetComponent<InteractionClass>().isInteractionType(InteractionClass.interactionType.player))
                    {
                        interactionTimer = interactionCooldown;
                        invent_.addObject(hitPoint.collider.GetComponent<HoldInteractionClass>());
                        setHeldItem(invent_.getActiveHandObject());

                    //Could break something.
                    } else if (hitPoint.collider.GetComponent<InteractionClass>().isInteractionType(InteractionClass.interactionType.playerHold) && avSlot == 0 && !holdingItem)
                    {

                        //Do not reset interaction timer as this will be used for holding the button.
                        handLocked = true;

                        playerHand.GetChild(0).transform.position = hitPoint.point;
                        hitPoint.collider.GetComponent<InteractionClass>().Interact(playerHand.GetChild(0).position, playerHand.GetChild(0).rotation, playerHand.GetComponentInChildren<Transform>());
                        hitPoint.collider.GetComponent<HoldInteractionClass>().setPickUpPosition(playerHand.GetChild(0));
                        invent_.addObject(hitPoint.collider.GetComponent<HoldInteractionClass>());

                        //Set holding item to whatever is in the active hand.
                        setHeldItem(invent_.getActiveHandObject());
                    }
                    //If available for a secondary interaction, then interact.
                    else if (hitPoint.collider.GetComponent<InteractionClass>().isInteractionType(InteractionClass.interactionType.secondaryInteraction))
                    {
                        interactionTimer = interactionCooldown;

                        hitPoint.collider.GetComponent<InteractionClass>().secondaryInteract();
                    }
                }
                //This is interactions with position items that can hold holdable items.
            }
            else if (hitPoint.collider.GetComponent<PositionInteractionClass>())
            {
                interactionTimer = interactionCooldown;

                int possibleItem = invent_.findObjectInSlot(0);

                //Debug.Log("Working2? " + possibleItem);

                //Make the interact happen.
                if (possibleItem > -1 && hitPoint.collider.GetComponent<PositionInteractionClass>().canHoldItem(invent_.getObjectAtInvent(possibleItem), false))
                {
                    //Debug.Log("Item is valid: " + invent_.getObjectAtInvent(possibleItem).name);
                    //Make sure this is an interaction type - player so that players CAN interact with this.
                    if (hitPoint.collider.GetComponent<InteractionClass>().isInteractionType(InteractionClass.interactionType.player))
                    {
                        //Make sure you cannot interact with sensor objects.
                        invent_.placeObject(hitPoint.collider.GetComponent<PositionInteractionClass>(), possibleItem);
                        //Drop the item.

                        removeHeldItem();
                    }

                    //Potential to still be interacted with in some way, even if this item cannot be held.
                }
                else
                {
                    if (invent_.findObjectInSlot(0) > -1)
                    {
                        //If not holding an item, try to pick up item from the positionInteraction.
                        if (hitPoint.collider.GetComponent<PositionInteractionClass>().getCurrentHeldItem() &&
                            hitPoint.collider.GetComponent<PositionInteractionClass>().getCurrentHeldItem().isInteractionType(InteractionClass.interactionType.player))
                        {
                            invent_.addObject(hitPoint.collider.GetComponent<PositionInteractionClass>().getCurrentHeldItem());
                            //hitPoint.collider.GetComponent<PositionInteractionClass>().getCurrentHeldItem().Interact(playerHand.position, playerHand.rotation, playerHand);
                        }
                    }

                }

                //Interactions that change the player controller.
            }
            else if (hitPoint.collider.GetComponent<PlayerControlInteractionClass>())
            {
                interactionTimer = interactionCooldown;

                //Ensure item can be touched by the player.
                if (hitPoint.collider.GetComponent<InteractionClass>().isInteractionType(InteractionClass.interactionType.player))
                {
                    //Make the interact happen.
                    hitPoint.collider.GetComponent<InteractionClass>().Interact(this.gameObject);
                }

                //For interaction to an input. If it doesn't have any input, then lock the input.
            } else if (hitPoint.collider.GetComponent<InputInteractionClass>())
            {
                interactionTimer = interactionCooldown;

                //Ensure item can be touched by the player.
                if (hitPoint.collider.GetComponent<InteractionClass>().isInteractionType(InteractionClass.interactionType.player))
                {

                    if (hitPoint.collider.GetComponent<InputInteractionClass>().getKeyboardInput())
                    {
                        inputLockObject = hitPoint.collider.GetComponent<InputInteractionClass>();
                    } else
                    {
                        //Make the interact happen.
                        hitPoint.collider.GetComponent<InteractionClass>().Interact();
                    }
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
                //If available for a secondary interaction, then interact.
                else if (hitPoint.collider.GetComponent<InteractionClass>().isInteractionType(InteractionClass.interactionType.secondaryInteraction))
                {
                    interactionTimer = interactionCooldown;

                    hitPoint.collider.GetComponent<InteractionClass>().secondaryInteract();
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
    public void setHeldItem(HoldInteractionClass obj)
    {
        //Debug.Log(obj);
        holdingItem = obj;
    }

    //This removes held item and sets the held item to it's unheld state.
    public void removeHeldItem()
    {
        holdingItem = null;

        //reset the newPosition back to hand.
        playerHand.GetChild(0).transform.position = playerHand.position;
    }

    //A function that will look for the next closest position, either within reach or when it hits something.
    private Vector3 frontPosition()
    {
        Vector3 trans = playerHead.position;

        RaycastHit hitPoint;
        Ray hitRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        int layerM = 1 << 3;

        layerM = ~layerM;

        //If the ray hits something within reach, see if it hits an object.
        if (Physics.Raycast(hitRay, out hitPoint, reach / 2, layerM))
        {
            trans = hitRay.GetPoint(hitPoint.distance);
        } else
        {
            trans = hitRay.GetPoint(reach / 2);
        }


        return trans;
    }

    //A function to find if any objects can be found above the player head.
    //This is used to ensure a player cannot get up if they crouched under something.
    private bool isNothingAbove()
    {
        Vector3 truePos = playerHead.parent.position;

        //Debug.DrawRay(truePos, Vector3.up.normalized * 0.6f, Color.white, 3);
        //Send a new ray up.
        if (Physics.Raycast(truePos, Vector3.up, 0.6f, ~LayerMask.GetMask(new string[] { "PlayerLayer", "MovementLayer" })))
        {
            //Something is above. if so, return false.
            return false;
        }

        return true;
    }

    private void makeRandomFootstep()
    {
        int randNum = 0;

        if (moveSpeed > 1)
        {
            randNum = (int)UnityEngine.Random.Range(0f, (float)runningClipLength);

            playerSource.PlayOneShot(audioManager_.getAudio(0, 1, randNum));
        } else
        {
            randNum = (int)UnityEngine.Random.Range(0f, (float)walkingClipLength);
            playerSource.PlayOneShot(audioManager_.getAudio(0, 0, randNum));
        }
            
    }

    //A function to set the locked section.
    public void setLock(bool b, GameObject obj, bool usingMouse)
    {
        if (b && usingMouse)
        {
            currentControl = controlStatus.noMovement;
        } else if(b && !usingMouse)
        {
            currentControl = controlStatus.mouseOnlyConfined;
        } else if (!b)
        {
            currentControl = controlStatus.fullControl;
        }

        lockingObject = obj;

        //mouseLocked = !usingMouse;

        if (usingMouse)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        } else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }

    public void setAnimObjectPosition(Transform tar)
    {
        if (tar)
        {
            anim_.transform.position = tar.position;
            anim_.transform.rotation = tar.rotation;
            currentStatus = playerStatus.sit;
            anim_.SetInteger("WalkState", (int)currentStatus);
        } else
        {
            anim_.transform.position = this.transform.position;
            anim_.transform.rotation = this.transform.rotation;
            currentStatus = playerStatus.idle;
            anim_.SetInteger("WalkState", (int)currentStatus);
        }
    }

    public void setMenu(bool b, bool usingMouse)
    {
        //If the prompt menu is locked, do not switch to menu.
        if (!menu_.lockMenu)
        {
            if (b && usingMouse)
            {
                currentControl = controlStatus.UIOnly;
            }
            else if (b && !usingMouse)
            {
                currentControl = controlStatus.mouseOnlyConfined;
            }
            else if (!b)
            {
                currentControl = controlStatus.fullControl;
            }

            //menuLocked = b;

            //interactionLocked = b;

            //mouseLocked = !usingMouse;

            if (usingMouse)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;

                //Set the menu to true.
                //Menu is index 1, stylus is index 0.
                menu_.setToMenuGroup("Menu");
                m_camera.lockHead(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                //Set the menu to true.
                //Menu is index 1, stylus is index 0.
                menu_.setToMenuGroup("Stylus");
                m_camera.lockHead(false);
            }
        }
    }

    /*public bool getLock()
    {
        return movementLocked;
    }*/

    //Player ID functions.
    public float getPlayerID()
    {
        return playerID;
    }

    public void setPlayerID(float ind)
    {
        playerID = ind;
    }

    public void setCurrentControl(controlStatus c)
    {
        currentControl = c;
        interactionTimer = interactionCooldown;
    }

    public controlStatus getCurrentControl()
    {
        return currentControl;
    }
}
