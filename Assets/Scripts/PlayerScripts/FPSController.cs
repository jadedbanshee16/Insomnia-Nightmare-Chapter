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
    public GameObject holdingItem;

    // Start is called before the first frame update
    void Start()
    {
        m_rig = GetComponent<Rigidbody>();
        m_collider = GetComponent<CapsuleCollider>();
        colliderSize = colliderSizes.y;

        currentStatus = playerStatus.idle;
        //Set the default move speed.
        moveSpeed = speedVariations.y;
    }

    // Update is called once per frame
    void Update()
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
        } else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            currentStatus = playerStatus.walk;
        }

        //Set up the running.
        if (Input.GetKey(KeyCode.LeftShift) && currentStatus != playerStatus.crouch)
        {
            moveSpeed = speedVariations.z;
            currentStatus = playerStatus.run;
        } else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = speedVariations.y;
            currentStatus = playerStatus.walk;
        }

        if (currentStatus == playerStatus.crouch && colliderSize > colliderSizes.x)
        {
            colliderSize -= Time.fixedDeltaTime;
        }

        if(currentStatus != playerStatus.crouch && colliderSize < colliderSizes.y)
        {
            colliderSize += Time.fixedDeltaTime;
        }

        m_camera.changeHeadBob((int)currentStatus);

        m_collider.height = colliderSize;


        //Make the controls for the mouse button.
        if (Input.GetKey(KeyCode.Mouse0))
        {
            isInInteraction();
        }

        if (Input.GetKey(KeyCode.G) && holdingItem)
        {
            holdingItem.GetComponent<HoldItemClass>().release();
            removeHeldItem();
        }
    }

    //Check if reaction is possible. If so, then make interaction.
    void isInInteraction()
    {
        RaycastHit hitPoint;

        //If the ray hits something within reach, see if it has button. If so, complete whatever interaction button is set.
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitPoint, reach))
        {
            Debug.Log(hitPoint.collider.gameObject.name);
            //Check if an item first.
            if (hitPoint.collider.CompareTag("HandItem"))
            {
                hitPoint.collider.GetComponent<InteractionClass>().interact(playerHand);
                holdingItem = hitPoint.collider.gameObject;
            } else if (hitPoint.collider.CompareTag("ItemPort"))
            {
                //Ensure an item is held.
                if (holdingItem)
                {
                    hitPoint.collider.GetComponent<InteractionClass>().interact(holdingItem.transform);
                }
                removeHeldItem();
            } else if (hitPoint.collider.GetComponent<InteractionClass>())
            {
                hitPoint.collider.GetComponent<InteractionClass>().interact();
            }
        }
    }

    //A function to return the current held item if any.
    public GameObject getHeldItem()
    {
        return holdingItem;
    }

    public void removeHeldItem()
    {
        holdingItem = null;
    }
}
