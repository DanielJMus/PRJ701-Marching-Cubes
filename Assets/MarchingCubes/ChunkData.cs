using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData : MonoBehaviour
{
    // PRIVATE METHODS
    // Chunk point data
    private Voxel[,,] data;

    // Chunk global position offset
    private Vector3Int position;

    // Chunk mesh filter
    private MeshFilter meshFilter;

    // If the data is outdated
    private bool isDirty = false;


    // PUBLIC PROPERTIES
    public Voxel[,,] Data
    {
        get { return data; }
        set { data = value; }
    }

    public Vector3Int Position 
    {
        get { return position; }
        set { position = value; }
    }

    public Mesh MeshData
    {
        get { return meshFilter.mesh; }
        set { meshFilter.mesh = value; }
    }

    public bool IsDirty
    {
        get { return isDirty; }
        set { isDirty = value; }
    }


    //  CONSTRUCTORS
    public ChunkData () {}
    public ChunkData (Vector3Int p)
    {
        position = p;
    }


    // PRIVATE METHODS
    void OnEnable ()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    // PUBLIC METHODS

    public bool HasMesh ()
    {
        return meshFilter.mesh != null;
    }

    // Returns the block at the local position specified
    public Voxel GetLocalVoxel (Vector3Int v)
    {
        return data[v.x, v.y, v.z];
    }

    // Returns the global coordinate of the specified voxel
    public Vector3Int GetGlobalVoxel (Vector3Int v)
    {
        return new Vector3Int(
            position.x + v.x,
            position.y + v.y,
            position.z + v.z
        );
    }


}
