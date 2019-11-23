using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{

    private float[,,] data;

    public DataManager (int x, int y, int z)
    {
        data = new float[x,y,z];
    }

    // Generate simple randomized terrain data
    public void Generate () {
        for(int x = 0; x < data.GetLength(0); x++) {
            for(int y = 0; y < data.GetLength(1); y++) {
                for(int z = 0; z < data.GetLength(2); z++) {
                    data[x,y,z] = Utils.Perlin3D((float)x / 8, (float)y / 8, (float)z / 8);
                    // float height = Mathf.PerlinNoise((float)x / 32, (float)z / 32) * 20;
                    // data[x,y,z] *= (y > 30 + height)?1.5f:1;
                }
            }
        }
    }

    public float[,,] GetData()
    {
        return data;
    }
}
