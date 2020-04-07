using UnityEngine;

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