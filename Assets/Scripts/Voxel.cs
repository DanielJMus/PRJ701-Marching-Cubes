using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    // Random float value for each corner of the cube
    public Vector3 Position;
    public float[] values = new float[8];
    // public Vector3[] points = new float[12];

    public Voxel () {
        
        // values[0] = Utils.Perlin3D(Position.x, Position.y, Position.z);
        // values[1] = Utils.Perlin3D(Position.x + 1, Position.y, Position.z);
        // values[2] = Utils.Perlin3D(Position.x + 1, Position.y + 1, Position.z);
        // values[3] = Utils.Perlin3D(Position.x , Position.y + 1, Position.z);
        // values[4] = Utils.Perlin3D(Position.x , Position.y, Position.z + 1);
        // values[5] = Utils.Perlin3D(Position.x + 1 , Position.y, Position.z + 1);
        // values[6] = Utils.Perlin3D(Position.x + 1 , Position.y + 1, Position.z + 1);
        // values[7] = Utils.Perlin3D(Position.x , Position.y + 1, Position.z + 1);

        // points[0] = new Vector3(Position.x, Position.y, Position.z);
        // points[1] = new Vector3(Position.x + 1, Position.y, Position.z);
        // points[2] = new Vector3(Position.x + 1, Position.y + 1, Position.z);
        // points[3] = new Vector3(Position.x , Position.y + 1, Position.z);
        // points[4] = new Vector3(Position.x , Position.y, Position.z + 1);
        // points[5] = new Vector3(Position.x + 1 , Position.y, Position.z + 1);
        // points[6] = new Vector3(Position.x + 1 , Position.y + 1, Position.z + 
        // points[7] = new Vector3(Position.x , Position.y + 1, Position.z + 1);
    }
}
