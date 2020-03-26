using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private Voxel[,,] Data;

    public Voxel[,,] GetData () {
        return Data;
    }
}
