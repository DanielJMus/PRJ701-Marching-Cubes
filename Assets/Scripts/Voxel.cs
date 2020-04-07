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
    // private List<Vertex> data = new List<Vertex>();
    private Dictionary<Vector3, Vertex> data = new Dictionary<Vector3, Vertex>();

    public Vector3Int Position 
    {
        get { return position; }
    }

    public int Material 
    {
        get { return materialID; }
    }

    public float Value 
    {
        get { return value; }
    }

    public bool HasBeenGenerated 
    {
        get { return hasBeenGenerated; }
        set { hasBeenGenerated = value; }
    }

    public Dictionary<Vector3, Vertex> Data 
    {
        get { return data; }
        set { data = value; }
    }

    public Voxel (Vector3Int p, float v, int mid)
    {
        position = p;
        value = v;
        materialID = mid;
    }

    public int HasVertex (Vector3 v) 
    {
        if(data.ContainsKey(v))
        {
            return data[v].index;
        }
        // for(int i = 0; i < data.Count(); i++)
        // {
        //     if(data[i].position == v)
        //     {
        //         return data[i].index;
        //     }
        // }
        return -1;
    }

    public void AddVertex(Vertex v)
    {
        data.Add(v.position, v);
    }
}