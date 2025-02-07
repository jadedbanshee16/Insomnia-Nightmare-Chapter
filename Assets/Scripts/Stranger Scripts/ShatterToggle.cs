using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterToggle : MonoBehaviour
{
    private float timer;

    private Vector3 oldRot;
    private Vector3[] newRots;
    private Vector3 oldScale;
    private Vector3 newScale;

    [SerializeField]
    Transform[] shatterPieces;
    [SerializeField]
    Transform player;

    [SerializeField]
    Camera cam;

    PoolObject obj;

    [SerializeField]
    float time;
    [SerializeField]
    float scaled;

    [SerializeField]
    bool isFacingPlayer;

    [SerializeField]
    float shatterStrength;

    PoolManager poolMan_;

    // Start is called before the first frame update
    void Start()
    {
        timer = time;

        float randomX = Random.Range(shatterPieces[0].localPosition.x - (shatterStrength / 50), shatterPieces[0].localPosition.x + (shatterStrength / 50));
        float randomY = Random.Range(shatterPieces[0].localPosition.y - (shatterStrength / 50), shatterPieces[0].localPosition.y + (shatterStrength / 50));
        float randomZ = Random.Range(shatterPieces[0].localPosition.z - (shatterStrength / 50), shatterPieces[0].localPosition.z + (shatterStrength / 50));

        oldRot = new Vector3(randomX, randomY, randomZ);
        oldScale = shatterPieces[0].localScale;

        newScale = oldScale * scaled;

        newRots = new Vector3[shatterPieces.Length];

        RenderTexture myRenderTexture = null;

        if (cam)
        {
            //Create and set a given render texture using the given camera.
            myRenderTexture = new RenderTexture(128, 128, 16, RenderTextureFormat.ARGB32);
            //myRenderTexture.Create();
            myRenderTexture.name = "CamRender";
            cam.targetTexture = myRenderTexture;
            cam.Render();
            cam.gameObject.SetActive(false);
        }

        for (int i = 0; i < shatterPieces.Length; i++)
        {
            randomX = Random.Range(shatterPieces[i].localPosition.x - (shatterStrength / 10), shatterPieces[i].localPosition.x + (shatterStrength / 10));
            randomY = Random.Range(shatterPieces[i].localPosition.y - (shatterStrength / 10), shatterPieces[i].localPosition.y + (shatterStrength / 10));
            randomZ = Random.Range(shatterPieces[i].localPosition.z - (shatterStrength / 10), shatterPieces[i].localPosition.z + (shatterStrength / 10));

            //Vector3 dir = Random.insideUnitSphere;

            Vector3 dir = new Vector3(randomX, randomY, randomZ);

            newRots[i] = dir;

            if (cam)
            {
                //Set the shattered pieces with this new renderTexture.
                shatterPieces[i].GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTexture", myRenderTexture);
                shatterPieces[i].GetComponentInChildren<MeshRenderer>().material.SetFloat("_Alpha", 1);
            }
        }


        obj = GetComponent<PoolObject>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        poolMan_ = obj.findManager();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            for(int i = 0; i < shatterPieces.Length; i++)
            {
                shatterPieces[i].localPosition = Vector3.Lerp(oldRot, newRots[i], 1 - (timer / time));
                shatterPieces[i].localScale = Vector3.Lerp(oldScale, newScale, 1 - (timer / time));

                if (isFacingPlayer)
                {
                    shatterPieces[i].LookAt(player);
                } else
                {
                    shatterPieces[i].LookAt(this.transform);
                }

                if (!poolMan_.getIsUsingMasterTime() && timer <= 1)
                {
                    shatterPieces[i].GetComponentInChildren<MeshRenderer>().material.SetFloat("_Alpha", timer);
                }
            }

        } else
        {
            if (!poolMan_.getIsUsingMasterTime())
            {
                timer = time;
                for (int i = 0; i < shatterPieces.Length; i++)
                {
                    shatterPieces[i].GetComponentInChildren<MeshRenderer>().material.SetFloat("_Alpha", 1);
                    shatterPieces[i].localScale = oldScale;
                }
                poolMan_.makeInactiveFromPool(obj.getIndex());

            }
        }

        //To use if using master timer in the pool manager.
        if (poolMan_.getIsUsingMasterTime())
        {
            if(poolMan_.getTimer() <= 1)
            {
                for (int i = 0; i < shatterPieces.Length; i++)
                {
                    shatterPieces[i].GetComponentInChildren<MeshRenderer>().material.SetFloat("_Alpha", poolMan_.getTimer());
                }
            } if(poolMan_.getTimer() > 1)
            {
                for (int i = 0; i < shatterPieces.Length; i++)
                {
                    shatterPieces[i].GetComponentInChildren<MeshRenderer>().material.SetFloat("_Alpha", 1);
                }
            }
            
            if(poolMan_.getTimer() <= 0)
            {
                timer = time;
            }
        }
    }
}
