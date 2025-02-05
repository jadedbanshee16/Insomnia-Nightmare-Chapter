using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StrangeV1Movement : MonoBehaviour
{
    private enum strangerState
    {
        idle,
        running
    };

    Vector3 currentTarget;

    strangerState state;

    NavMeshAgent agent_;
    Animator anim_;

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        agent_ = GetComponent<NavMeshAgent>();
        anim_ = GetComponent<Animator>();

        //Get a random position on navmesh.
        currentTarget = getRandomPosition();
        agent_.SetDestination(currentTarget);

        state = strangerState.running;
        anim_.SetBool("Moving", true);

        timer = Random.Range(1, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if(state == strangerState.running)
        {
            if (Vector3.Distance(currentTarget, this.transform.position) < 0.1)
            {
                currentTarget = getRandomPosition();
                agent_.isStopped = true;

                state = strangerState.idle;
                anim_.SetBool("Moving", false);
            }
        } else if (state == strangerState.idle)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
            } else
            {
                timer = Random.Range(1, 10);
                agent_.SetDestination(currentTarget);
                agent_.isStopped = false;
                state = strangerState.running;
                anim_.SetBool("Moving", true);
            }
        }
    }

    Vector3 getRandomPosition()
    {
        Vector3 returnPosition = Random.insideUnitSphere * 8;

        returnPosition += this.transform.position;

        NavMeshHit hit;

        NavMesh.SamplePosition(returnPosition, out hit, 8, 1);
        return hit.position;
    }
}
