using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct TerrainTextureMap
{
    [Range(0, 1)]
    public float minHeightThreshold;
    [Range(0, 1)]
    public float maxHeightThreshold;
}

public class TerrainAlphaMapSetter : MonoBehaviour
{
    public Terrain t;
    public TerrainTextureMap[] textureMaps;
    // Blend the two terrain textures according to the steepness of
    // the slope at each point.
    [ContextMenu("SetAlphaMaps")]
    void SetAlphaMaps()
    {
        float[,,] map = new float[t.terrainData.alphamapWidth, t.terrainData.alphamapHeight, textureMaps.Length];

        // For each point on the alphamap...
        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
            {
                // Get the normalized terrain coordinate that
                // corresponds to the point.
                float normX = x * 1.0f / (t.terrainData.alphamapWidth - 1);
                float normY = y * 1.0f / (t.terrainData.alphamapHeight - 1);



                float height = t.terrainData.GetInterpolatedHeight(normX, normY);
                float heightNorm = height / t.terrainData.heightmapScale.y;

                var angle = t.terrainData.GetSteepness(normX, normY);
                var frac = angle / 90.0;

                // Steepness is given as an angle, 0..90 degrees. Divide
                // by 90 to get an alpha blending value in the range 0..1.
                //map[x, y, 0] = (float)frac;
                //map[x, y, 1] = (float)(1 - frac);
                //map[x, y, 2] = 0;

                for (int i = 0; i < textureMaps.Length; i++)
                {
                    if (heightNorm >= textureMaps[i].minHeightThreshold && heightNorm <= textureMaps[i].maxHeightThreshold)
                    {
                        map[x, y, i] = 1;
                    }
                    else
                    {
                        map[x, y, i] = 0;
                    }
                    //map[x, y, i] *= textureMaps[i].alphaBlend;
                }
            }
        }
        t.terrainData.SetAlphamaps(0, 0, map);
    }
}
