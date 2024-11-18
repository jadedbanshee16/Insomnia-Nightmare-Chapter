using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceObjectClass : MonoBehaviour
{
    [SerializeField]
    AudioManager.walkingStatus surfaceType;

    [SerializeField]
    bool isDynamic;

    TerrainData terrain;
    private float[,,] splashData;
    private int numTex;

    AudioManager manager_;

    private void Start()
    {
        manager_ = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioManager>();

        if (GetComponent<Terrain>())
        {
            terrain = GetComponent<Terrain>().terrainData;
            splashData = terrain.GetAlphamaps(0, 0, terrain.alphamapWidth, terrain.alphamapHeight);
            numTex = splashData.Length / (terrain.alphamapWidth * terrain.alphamapHeight);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Check if collision is the player.
        if (collision.gameObject.CompareTag("Player"))
        {
            //Check if not dynamic. If not, then use surface type.
            if (!isDynamic)
            {
                manager_.setStatus(surfaceType);
            } else
            {
                if (!terrain)
                {
                    manager_.setStatus(surfaceType);
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isDynamic && collision.gameObject.CompareTag("Player"))
        {
            manager_.setStatus(getTerrainCoordinateTex(collision.GetContact(0).point));
        }
    }

    private AudioManager.walkingStatus getTerrainCoordinateTex(Vector3 pos)
    {
        Vector3 vecRet = Vector3.zero;

        vecRet.x = ((pos.x - transform.position.x) / terrain.size.x) * terrain.alphamapWidth;
        vecRet.z = ((pos.z - transform.position.z) / terrain.size.z) * terrain.alphamapHeight;

        int returnVal = 0;
        float comp = -Mathf.Infinity;

        //Go through and attempted to find the current dominant index of the terrain.
        for(int i = 0; i < numTex; i++)
        {
            if(comp < splashData[(int)vecRet.z, (int)vecRet.x, i])
            {
                comp = splashData[(int)vecRet.z, (int)vecRet.x, i];
                returnVal = i;
            }
        }

        //Finally, return the current status based on index, going through the textures.
        switch (returnVal)
        {
            case 0:
                return AudioManager.walkingStatus.metal;
            case 1:
                return AudioManager.walkingStatus.metal;
            case 2:
                return AudioManager.walkingStatus.metal;
            case 3:
                return AudioManager.walkingStatus.dirt;
            case 4:
                return AudioManager.walkingStatus.grass;
            case 5:
                return AudioManager.walkingStatus.grass;
            case 6:
                return AudioManager.walkingStatus.dirt;
            case 7:
                return AudioManager.walkingStatus.dirt;
            case 8:
                return AudioManager.walkingStatus.dirt;
            case 9:
                return AudioManager.walkingStatus.dirt;
            case 10:
                return AudioManager.walkingStatus.grass;
            case 11:
                return AudioManager.walkingStatus.grass;
            case 12:
                return AudioManager.walkingStatus.grass;
            case 13:
                return AudioManager.walkingStatus.grass;

        }

        //If all else fails, go metal.
        return AudioManager.walkingStatus.metal;
    }
}
