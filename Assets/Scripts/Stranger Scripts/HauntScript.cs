using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HauntScript : MonoBehaviour
{
    NavMeshAgent agent;

    Vector3 targetPos;
    Vector3 lastPos;

    float steps = 1;

    float eventTimer = 0.5f;

    float stepTimer = 1;

    [SerializeField]
    private List<HoldInteractionClass> interactions;

    private InteractionControlClass controller;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<InteractionControlClass>();

        //Get a random position on navmesh.
        targetPos = getRandomPosition();
        agent.SetDestination(targetPos);

        lastPos = Vector3.zero;

        interactions = new List<HoldInteractionClass>();

        controller.playInbuiltAudio(0, true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(targetPos, this.transform.position) < 0.1)
        {
            targetPos = getRandomPosition();
            agent.SetDestination(targetPos);
        }

        //Something to ensure it doesn't get stuck.
        if(Vector3.Distance(lastPos, this.transform.position) < 0.01)
        {
            if(steps > 0)
            {
                steps -= Time.deltaTime;
            } else
            {
                steps = 1;
                targetPos = getRandomPosition();
                agent.SetDestination(targetPos);
            }
        } else
        {
            steps = 1;
            lastPos = this.transform.position;
        }

        //Throw an object when near.
        if(interactions.Count > 0)
        {
            if(eventTimer > 0)
            {
                eventTimer -= Time.deltaTime;
            } else
            {
                eventTimer = 0.5f;

                int rand = Random.Range(0, interactions.Count - 1);

                Vector3 randDirection = Random.insideUnitSphere * 4;

                //Check if door.
                if (interactions[rand].GetComponent<HingeJoint>())
                {
                    //Get random angle and set that angle.
                    //All door objects have an anchor, I think.
                    randDirection.y = interactions[rand].getAnchor().position.y;
                    interactions[rand].GetComponent<InteractionControlClass>().setAngle(randDirection, interactions[rand].getAnchor());
                }
                else
                {
                    interactions[rand].GetComponent<Rigidbody>().AddForce(randDirection * 60);
                }
            }
        }

        //Now make sounds.
        if(stepTimer > 0)
        {
            stepTimer -= Time.deltaTime;
        } else
        {
            stepTimer = 1;
            controller.playInteractionAudio(Random.Range(0, controller.getAudioLength() - 1));
        }
    }

    Vector3 getRandomPosition()
    {
        Vector3 returnPosition = Random.insideUnitSphere * 2;

        returnPosition += this.transform.position;

        NavMeshHit hit;

        NavMesh.SamplePosition(returnPosition, out hit, 2, 1);
        return hit.position;
    }

    void kill()
    {
        //Get event manager and set to kill.
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<EventManager>().endByKill();
    }


    //Make triggers to add hild interactions to this list.
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HoldInteractionClass>() && !other.GetComponent<HoldInteractionClass>().getCurrentHolder())
        {
            interactions.Add(other.GetComponent<HoldInteractionClass>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<HoldInteractionClass>())
        {
            for (int i = 0; i < interactions.Count; i++)
            {
                if(interactions[i] == other.GetComponent<HoldInteractionClass>())
                {
                    interactions.RemoveAt(i);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            kill();
        }
    }
}
