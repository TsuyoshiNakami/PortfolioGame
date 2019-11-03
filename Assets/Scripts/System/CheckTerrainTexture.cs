using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTerrainTexture : MonoBehaviour
{

    public Transform playerTransform;
    public Terrain t;

    public int posX;
    public int posZ;
    public float[] textureValues;

    static CheckTerrainTexture checkTerrainTexture;
    public static CheckTerrainTexture Instance {
        get {

            if(checkTerrainTexture == null)
            {
                checkTerrainTexture = GameObject.Find("Player").GetComponent<CheckTerrainTexture>();
            }
            return checkTerrainTexture;
        }
        }

    void Start()
    {
        t = Terrain.activeTerrain;
        playerTransform = gameObject.transform;
        textureValues = new float[4];
    }

    void Update()
    {
        // For better performance, move this out of update 
        // and only call it when you need a footstep.
        GetTerrainTexture();
    }

    public void GetTerrainTexture()
    {
        ConvertPosition(playerTransform.position);
        CheckTexture();
    }

    void ConvertPosition(Vector3 playerPosition)
    {
        Vector3 terrainPosition = playerPosition - t.transform.position;

        Vector3 mapPosition = new Vector3
        (terrainPosition.x / t.terrainData.size.x, 0,
        terrainPosition.z / t.terrainData.size.z);

        float xCoord = mapPosition.x * t.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * t.terrainData.alphamapHeight;

        posX = (int)xCoord;
        posZ = (int)zCoord;
    }

   public int GetMainGroundNum()
    {
        float maxValue = 0;
        int tmpNum = 0;
        for(int i = 0; i < textureValues.Length; i++) 
        {
            if(textureValues[i] > maxValue)
            {
                maxValue = textureValues[i];
                tmpNum = i;
            }
        }
        if(maxValue == 0)
        {
            return -1;
        }
        return tmpNum;
    }

    void CheckTexture()
    {
        float[,,] aMap = t.terrainData.GetAlphamaps(posX, posZ, 1, 1);
        textureValues[0] = aMap[0, 0, 0];
        textureValues[1] = aMap[0, 0, 1];
        textureValues[2] = aMap[0, 0, 2];
        //textureValues[3] = aMap[0, 0, 3];
    }
}
