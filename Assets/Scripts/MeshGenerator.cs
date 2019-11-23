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
                        
                        int indexA = cornerAIndices[edge];
                        int indexB = cornerBIndices[edge];

                        Vector3 vertex = new Vector3(x, y, z) + Smooth(surfaceLevel, GetCubeCorner(indexA), GetCubeCorner(indexB), GetData(new Vector3Int(x, y, z), indexA, data), GetData(new Vector3Int(x, y, z), indexB, data));
                        vertices.Add(vertex); 
                        triangles.Add(triCount);
                        triCount++;
                    }
                }
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.RecalculateNormals();
        return mesh;
    }

    private Vector3 Smooth(float surfaceLevel, Vector3 p1, Vector3 p2, float val1, float val2) {
        surfaceLevel = (surfaceLevel - val1) / (val2 - val1);
        return p1 + surfaceLevel * (p2-p1);
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

    private float GetData (Vector3Int p, int i, float[,,] data) {
        float val = 0;
        try {
            if(i == 0) val = data[p.x, p.y, p.z];
            if(i == 1) val = data[p.x + 1, p.y, p.z];
            if(i == 2) val = data[p.x + 1, p.y + 1, p.z];
            if(i == 3) val = data[p.x, p.y + 1, p.z];
            if(i == 4) val = data[p.x, p.y, p.z + 1];
            if(i == 5) val = data[p.x + 1, p.y, p.z + 1];
            if(i == 6) val = data[p.x + 1, p.y + 1, p.z + 1];
            if(i == 7) val = data[p.x, p.y + 1, p.z + 1];
        } catch (System.Exception e) {
            val = 1;
        }
        return val;
    }

    private int GetCubeIndex (int x, int y, int z, float[,,] data, float surfaceLevel)
    {
        int cubeindex = 0;
        if (GetData(new Vector3Int(x, y, z), 0, data) > surfaceLevel) cubeindex |= 1;
        if (GetData(new Vector3Int(x, y, z), 1, data) > surfaceLevel) cubeindex |= 2;
        if (GetData(new Vector3Int(x, y, z), 2, data) > surfaceLevel) cubeindex |= 4;
        if (GetData(new Vector3Int(x, y, z), 3, data) > surfaceLevel) cubeindex |= 8;
        if (GetData(new Vector3Int(x, y, z), 4, data) > surfaceLevel) cubeindex |= 16;
        if (GetData(new Vector3Int(x, y, z), 5, data) > surfaceLevel) cubeindex |= 32;
        if (GetData(new Vector3Int(x, y, z), 6, data) > surfaceLevel) cubeindex |= 64;
        if (GetData(new Vector3Int(x, y, z), 7, data) > surfaceLevel) cubeindex |= 128;
        return cubeindex;
    }
}
