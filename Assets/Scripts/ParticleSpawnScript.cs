using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A Script to spawn and remove particles for day and night cycles.
 * These particles will spawn at random points in a circle and after being completed, will be removed.
 */

public class ParticleSpawnScript : MonoBehaviour
{
    public int particleAmount;

    public float TimeBetweenSpawns = 3;
    private float timer;

    public GameObject[] dayParticles;
    public GameObject[] nightParticles;

    private GameObject[] activeParticles;

    private GameManager m_manager;

    private SphereCollider col;

    /*
     * REPLACE WITH A REFERENCE TO THE DAY/NIGHT MANAGER
     */
    public bool isDay = true;

    // Start is called before the first frame update
    void Start()
    {
        //Create an array to keep all active particles at once.
        activeParticles = new GameObject[particleAmount];

        //Get the game manager to work with.
        m_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        col = gameObject.GetComponent<SphereCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        //Only trip with the player.
        if (other.gameObject.CompareTag("Player"))
        {
            //See if a particle will spawn.
            if(Random.Range(0f, 100f) > 50)
            {
                int emptySlot = ArrayHasEmpty();
                if (emptySlot > -1)
                {
                    createParticle(m_manager.getDay(), emptySlot);
                }
            }
        }
    }

    private void createParticle(bool d, int slot)
    {
        int num = 0;

        if (timer <= 0)
        {
            //Create a particle during day and night cycle.
            if (Random.Range(0f, 100f) > 50)
            {
                num = 0;
            }
            else
            {
                num = 1;
            }

            Vector3 Setpos = Random.insideUnitCircle * col.radius;

            Vector3 pos = new Vector3(Setpos.x, 0, Setpos.y);


            if (d)
            {
                //Put an object in the nearest empty.
                activeParticles[slot] = Instantiate(dayParticles[num], transform.position + pos, Quaternion.identity, this.transform);
            } else
            {
                //Due to night stuff happening close to ground, use that.
                pos.y = pos.y - 1;
                //Put an object in the nearest empty.
                activeParticles[slot] = Instantiate(nightParticles[num], transform.position + pos, Quaternion.identity, this.transform);
            }

            //Reset Timer;
            timer = TimeBetweenSpawns;
        } else
        {
            timer -= Time.deltaTime;
        }

    }

    //Check if an array is empty.
    private int ArrayHasEmpty()
    {
        //Ensure active Particles is valid.
        if (activeParticles == null || activeParticles.Length == 0)
        {
            return -1;
        }

        //Iterate through array to find an object.
        for (int i = 0; i < activeParticles.Length; i++)
        {
            //If finding a null, then it has an empty slot.
            if (activeParticles[i] == null)
            {
                return i;
            }
        }

        //If nothing found, return false if empty.
        return -1;
    }
}
