using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Voxel
{
    private Vector3Int position;
    private float value;
    private int materialID = 0;
    private bool hasBeenGenerated = false;
    private List<Vertex> data = new List<Vertex>();

    public Vector3Int Position {
        get { return position; }
    }

    public int Material {
        get { return materialID; }
    }

    public float Value {
        get { return value; }
    }

    public bool HasBeenGenerated {
        get { return hasBeenGenerated; }
        set { hasBeenGenerated = value; }
    }

    public List<Vertex> Data {
        get { return data; }
        set { data = value; }
    }

    public Voxel (Vector3Int p, float v, int mid)
    {
        position = p;
        value = v;
        materialID = mid;
    }

    public int HasVertex (Vector3 v) {
        foreach(Vertex vert in data)
        {
            if(vert.position == v)
            {
                return vert.index;
            }
        }
        return -1;
    }

    public void AddVertex(Vertex v) {
        data.Add(v);
    }
}

public class Vertex
{
    public Vector3 position;
    public int index;

    public Vertex (Vector3 p, int i)
    {
        position = p;
        index = i;
    }
}