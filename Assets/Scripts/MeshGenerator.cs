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

                        // Vector3 vertex = new Vector3(x, y, z) + ((GetCubeCorner(indexA) + GetCubeCorner(indexB)) / 2);
                        Vector3 vertex = new Vector3(x, y, z) + VertexInterp(surfaceLevel, GetCubeCorner(indexA), GetCubeCorner(indexB), GetData(indexA, data), GetData(indexB, data));
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

    private Vector3 VertexInterp(float surfaceLevel, Vector3 p1, Vector3 p2, float val1, float val2) {
        if(val1 < val2)
            return Vector3.Lerp(p1, p2, surfaceLevel);
        else 
            return Vector3.Lerp(p2, p1, surfaceLevel);
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
            val = 1;
        }
        return val;
    }

    private float GetData (int i, float[,,] data) {
        float val = 0;
        try {
            if(i == 0) val = data[0, 0, 0];
            if(i == 1) val = data[1, 0, 0];
            if(i == 2) val = data[1, 1, 0];
            if(i == 3) val = data[0, 1, 0];
            if(i == 4) val = data[0, 0, 1];
            if(i == 5) val = data[1, 0, 1];
            if(i == 6) val = data[1, 1, 1];
            if(i == 7) val = data[0, 1, 1];
        } catch (System.Exception e) {
            val = data[0, 0, 0];
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
