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
    }
}