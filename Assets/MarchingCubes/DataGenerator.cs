using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGenerator
{

    private Voxel[,,] data;

    public DataGenerator () 
    {
        data = new Voxel[TerrainData.ChunkSize.x, TerrainData.ChunkSize.y, TerrainData.ChunkSize.z];
    }

    // public static int seed;

    public void Generate (ChunkData Chunk)
    {
        for(int x = 0; x < TerrainData.ChunkSize.x; x++)
        for(int y = 0; y < TerrainData.ChunkSize.y; y++)
        for(int z = 0; z < TerrainData.ChunkSize.z; z++)
        {
            data[x,y,z] = new Voxel(
                new Vector3Int(x, y, z),
                Noise(Chunk.Position.x + x, Chunk.Position.y + y, Chunk.Position.z + z),
                (int)Mathf.Round(Random.value * 255)
            );
        }
        Chunk.Data = data;
        Chunk.IsDirty = true;
    }

    // static float seaLevel = 8;

    public static float Noise (float x, float y, float z)
    {
        float value = 0;
        // Get the general surface level height using perlin noise (noise * seaLevelHeight)
        // If y is below that height, make it solid
        float surfaceHeight = Mathf.PerlinNoise(x / 64, z / 64) * 8;
        surfaceHeight -= Mathf.Abs(Mathf.PerlinNoise(x / 32, z / 32) * 16);
        surfaceHeight += Mathf.PerlinNoise(x / 16, z / 16) * 8;
        // surfaceHeight += Mathf.PerlinNoise(x / 128, z / 128) * 128;
        value = smoothstep(surfaceHeight - 10, surfaceHeight, y);

        // value += Utils.Perlin3D(x / 4, y / 4, z / 4);

        value = smoothstep(0, 1, value);
        // if(y < surfaceHeight) value = 0;
        // else value = 1;
        return value;
    }

    static float smoothstep(float edge0, float edge1, float x) {
        // Scale, bias and saturate x to 0..1 range
        x = Mathf.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f); 
        // Evaluate polynomial
        return x * x * (3 - 2 * x);
    }
}
