using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LightManager : MonoBehaviour
{
    [SerializeField]
    private GameObject activeLight;

    [SerializeField]
    float maxDist;

    [SerializeField]
    float minDist;

    Transform player;

    [SerializeField]
    private float currentLevel;

    // Start is called before the first frame update
    void Start()
    {

        player = null;

        currentLevel = 1;
    }

    private void Update()
    {
        if(player != null)
        {
            currentLevel = findSaturationLevel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = null;
            currentLevel = 1;
        }
    }

    private float findSaturationLevel()
    {
        //Get distance to player, then return a percentage from the minimum distance.
        if (Vector3.Distance(player.position, this.transform.position) < maxDist)
        {
            float per = Mathf.Clamp((Vector3.Distance(player.position, this.transform.position) - minDist) / (maxDist - minDist), 0, 1);

            //Debug.Log("Percentage: " + per);
            //Ensure light object is set properly.
            if (activeLight && activeLight.activeSelf)
            {
                return per;
            } else
            {
                return 1;
            }
        }

        return 1;
    }

    public float getCurrentLevel()
    {
        return currentLevel;
    }
}