using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{

    private Voxel[,,] data;

    public DataManager (int x, int y, int z)
    {
        data = new Voxel[x,y,z];
    }

    // Generate simple randomized terrain data
    public void Generate () {
        for(int x = 0; x < data.GetLength(0); x++) {
            for(int y = 0; y < data.GetLength(1); y++) {
                for(int z = 0; z < data.GetLength(2); z++) {
                    data[x,y,z] = new Voxel(
                        new Vector3Int(x, y, z),
                        Utils.Perlin3D((float)x / 8, (float)y / 8, (float)z / 8),
                        (int)Mathf.Round(Random.value * 255)
                    );
                }
            }
        }
    }

    public Voxel[,,] GetData()
    {
        return data;
    }
}
