using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterToggle : MonoBehaviour
{
    private float timer;

    private Quaternion oldRot;
    private Quaternion[] newRots;

    [SerializeField]
    Transform[] shatterPieces;

    [SerializeField]
    Camera cam;

    PoolObject obj;

    [SerializeField]
    float time;

    // Start is called before the first frame update
    void Start()
    {
        timer = time;

        oldRot = this.transform.rotation;

        newRots = new Quaternion[shatterPieces.Length];

        //Create and set a given render texture using the given camera.
        RenderTexture myRenderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        //myRenderTexture.Create();
        myRenderTexture.name = "CamRender";
        cam.targetTexture = myRenderTexture;

        for (int i = 0; i < shatterPieces.Length; i++)
        {
            float randomX = Random.Range(0, 60);
            float randomY = Random.Range(0, 60);

            newRots[i] = Quaternion.AngleAxis(randomX, Vector3.up) * Quaternion.AngleAxis(randomY, Vector3.right);

            //Set the shattered pieces with this new renderTexture.
            shatterPieces[i].GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTexture", myRenderTexture);
        }


        obj = GetComponent<PoolObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            for(int i = 1; i < shatterPieces.Length; i++)
            {
                shatterPieces[i].rotation = Quaternion.Lerp(newRots[i], oldRot, timer / 2);
            }
        } else
        {
            timer = time;
            obj.findManager().makeInactiveFromPool(obj.getIndex());
        }
    }
}
