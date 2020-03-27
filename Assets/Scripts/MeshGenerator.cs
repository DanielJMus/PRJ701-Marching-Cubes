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
    Vector3Int worldSize;

    int triCount = 0;

    public Mesh Generate (Voxel[,,] data, float surfaceLevel, Vector3Int size)
    {
        Mesh mesh = new Mesh();
        // mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        worldSize = size;
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

                        Vector3 vertex = new Vector3(x, y, z) + Smooth( surfaceLevel, 
                                                                        GetCubeCorner(indexA),
                                                                        GetCubeCorner(indexB), 
                                                                        GetData(new Vector3Int(x, y, z), indexA, data), 
                                                                        GetData(new Vector3Int(x, y, z), indexB, data));
                        // Check if vertex is already being used by neighboring thingo
                        int triangle = SharesVertex(x, y, z, data, vertex);
                        if(triangle < 0)
                        {
                            triangle = triCount;
                            triCount++;
                            vertices.Add(vertex);
                        }

                        triangles.Add(triangle);
                        data[x, y, z].AddVertex(new Vertex(vertex, triangle));
                    }
                    data[x, y, z].HasBeenGenerated = true;
                }
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.RecalculateNormals(180);
        return mesh;
    }

    Vector3Int[] neighbors = new Vector3Int[] 
    {   
        new Vector3Int(-1, 0, 0), 
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, 0),
        new Vector3Int(0, 0, -1),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
    };

    private int SharesVertex (int x, int y, int z, Voxel[,,] data, Vector3 vertex)
    {
        foreach(Vector3Int v in neighbors)
        {
            Voxel voxel = GetNeighborVoxel(x + v.x, y + v.y, z + v.z, data);
            if(voxel != null) 
            {
                if(voxel.HasVertex(vertex) > -1)
                {
                    return voxel.HasVertex(vertex);
                }
            }
        }
        return -1;
    }

    private Voxel GetNeighborVoxel (int x, int y, int z, Voxel[,,] data)
    {
        if(x < 0 || y < 0 || z < 0 || x >= worldSize.x || y >= worldSize.y || z >= worldSize.z) return null;
        else return data[x, y, z];
    }

    private Vector3 Smooth(float surfaceLevel, Vector3 position1, Vector3 position2, float point1, float point2)
    {
        surfaceLevel = (surfaceLevel - point1) / (point2 - point1);
        return position1 + surfaceLevel * (position2-position1);
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

    private float GetData (Vector3Int p, int i, Voxel[,,] data) {
        float val = 0;
        try {
            if(i == 0) val = data[p.x, p.y, p.z].Value;
            if(i == 1) val = data[p.x + 1, p.y, p.z].Value;
            if(i == 2) val = data[p.x + 1, p.y + 1, p.z].Value;
            if(i == 3) val = data[p.x, p.y + 1, p.z].Value;
            if(i == 4) val = data[p.x, p.y, p.z + 1].Value;
            if(i == 5) val = data[p.x + 1, p.y, p.z + 1].Value;
            if(i == 6) val = data[p.x + 1, p.y + 1, p.z + 1].Value;
            if(i == 7) val = data[p.x, p.y + 1, p.z + 1].Value;
        } catch (System.Exception e) {
            val = 1;
        }
        return val;
    }

    private int GetCubeIndex (int x, int y, int z, Voxel[,,] data, float surfaceLevel)
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
