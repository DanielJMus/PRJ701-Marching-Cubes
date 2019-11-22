using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    public MeshGenerator ()
    {
        
    }

    private List<Vector3> vertices;
    private List<int> triangles;

    int triCount = 0;

    public Mesh Generate (float[,,] data, float surfaceLevel)
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        vertices = new List<Vector3>();
        triangles = new List<int>();
        for(int x = 0; x < data.GetLength(0); x++)
            for(int y = 0; y < data.GetLength(1); y++)
                for(int z = 0; z < data.GetLength(2); z++)
                {
                    int cubeIndex = GetCubeIndex(x, y, z, data, surfaceLevel);
                    int[] tris = Utils.TriangleTable[cubeIndex];
                    foreach(int edge in tris) {
                        if(edge == -1) continue;
                        // Debug.Log(edge);
                        int indexA = cornerAIndices[edge];
                        int indexB = cornerBIndices[edge];

                        Vector3 vertex = new Vector3(x, y, z) + (GetCubeCorner(indexA) + GetCubeCorner(indexB) / 2);
                        vertices.Add(vertex); 
                        // triangles.Add(triCount + edge);
                        // Debug.Log(triCount + edge);
                    }
                    triCount += tris.Length;
                }
        mesh.SetVertices(vertices);
        mesh.SetIndices(triangles.ToArray(), MeshTopology.Triangles, 0);
        return mesh;
    } 

    private int[] cornerAIndices = new int[12] {0, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3};
    private int[] cornerBIndices = new int[12] {1, 2, 3, 0, 5, 6, 7, 4, 4, 5, 6, 7};

    private Vector3 GetCubeCorner (int i) {
        if(i == 0) return new Vector3(0, 0, 0);
        if(i == 1) return new Vector3(1, 0, 0);
        if(i == 2) return new Vector3(1, 1, 0);
        if(i == 3) return new Vector3(0, 1, 0);
        if(i == 4) return new Vector3(0, 0, 1);
        if(i == 5) return new Vector3(1, 0, 1);
        if(i == 6) return new Vector3(1, 1, 1);
        if(i == 7) return new Vector3(0, 1, 1);
        return Vector3.zero;
    }

    private float GetData (int x, int y, int z, float[,,] data) {
        float val = 0;
        try {
            val = data[x, y, z];
        } catch (System.Exception e) {
            val = 0;
        }
        return val;
    }

    private int GetCubeIndex (int x, int y, int z, float[,,] data, float surfaceLevel)
    {
        int cubeindex = 0;
        if (GetData(x,y,z, data) > surfaceLevel) cubeindex |= 1;
        if (GetData(x+1,y,z, data) > surfaceLevel) cubeindex |= 2;
        if (GetData(x+1,y+1,z, data) > surfaceLevel) cubeindex |= 4;
        if (GetData(x,y+1,z, data) > surfaceLevel) cubeindex |= 8;
        if (GetData(x,y,z+1, data) > surfaceLevel) cubeindex |= 16;
        if (GetData(x+1,y,z+1, data) > surfaceLevel) cubeindex |= 32;
        if (GetData(x+1,y+1,z+1, data) > surfaceLevel) cubeindex |= 64;
        if (GetData(x,y+1,z+1, data) > surfaceLevel) cubeindex |= 128;
        return cubeindex;
    }
}
