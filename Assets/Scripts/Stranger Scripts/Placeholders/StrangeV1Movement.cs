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

    [SerializeField]
    GameObject shadowPrefab;
    [SerializeField]
    float meshRefreshRate;
    [SerializeField]
    public Transform position;

    Vector3 currentTarget;

    strangerState state;

    NavMeshAgent agent_;
    Animator anim_;
    SkinnedMeshRenderer mesh;
    PoolManager shadowPool;

    float timer;

    private bool isRunning;
    private bool changedToRunning;
    // Start is called before the first frame update
    void Start()
    {
        agent_ = GetComponent<NavMeshAgent>();
        anim_ = GetComponent<Animator>();
        shadowPool = GetComponent<PoolManager>();

        //Get a random position on navmesh.
        currentTarget = getRandomPosition();
        agent_.SetDestination(currentTarget);

        state = strangerState.running;
        isRunning = true;
        anim_.SetBool("Moving", isRunning);

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
                isRunning = false;
                changedToRunning = true;
                anim_.SetBool("Moving", isRunning);
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
                isRunning = true;
                anim_.SetBool("Moving", isRunning);
            }
        }

        if (isRunning && changedToRunning)
        {
            changedToRunning = false;
            StartCoroutine(ActivateTrail());
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

    IEnumerator ActivateTrail()
    {

        while (isRunning)
        {
            if (!mesh)
            {
                mesh = GetComponentInChildren<SkinnedMeshRenderer>();
            }

            shadowPool.makeActiveFromPool(position.position, position.rotation);

            //GameObject obj = Instantiate(shadowPrefab, position.position, position.rotation);

            //MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            //MeshFilter mf = obj.GetComponent<MeshFilter>();

            //Mesh m = new Mesh();

            // mesh.BakeMesh(m);

            // mf.mesh = m;

            yield return new WaitForSeconds(meshRefreshRate);
        }
    }
}
