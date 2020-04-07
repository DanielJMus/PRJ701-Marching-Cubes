using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubes
{

    // CONSTRUCTOR

    public MarchingCubes () {}


    // PRIVATE FIELDS

    private float SurfaceLevel = 0.5f;
    private List<Vector3> Vertices = new List<Vector3>();
    private List<int> Indices = new List<int>();
    private int[] cornerAIndices = new int[12] {0, 1, 2, 3, 4, 5, 6, 7, 0, 1, 2, 3};
    private int[] cornerBIndices = new int[12] {1, 2, 3, 0, 5, 6, 7, 4, 4, 5, 6, 7};


    // PUBLIC METHODS

    // Returns a mesh generated using the marching cubes algorithm.
    public Mesh GenerateMesh (ChunkData Chunk)
    {
        Mesh mesh = new Mesh();
        int TriangleCount = 0;
        for (int x = 0; x < TerrainData.ChunkSize.x; x++)
            for (int y = 0; y < TerrainData.ChunkSize.y; y++)
                for (int z = 0; z < TerrainData.ChunkSize.z; z++)
                {
                    int CubeIndex = GetCubeIndex(x, y, z, Chunk);
                    int[] Triangles = Utils.TriangleTable[CubeIndex];

                    foreach(int edge in Triangles)
                    {
                        if(edge == -1) continue;

                        int indexA = cornerAIndices[edge];
                        int indexB = cornerBIndices[edge];

                        Vector3 vertex = new Vector3(x, y, z) + Smooth( SurfaceLevel, 
                                                                        GetCubeCorner(indexA),
                                                                        GetCubeCorner(indexB), 
                                                                        GetVoxelValue(new Vector3Int(x, y, z), indexA, Chunk), 
                                                                        GetVoxelValue(new Vector3Int(x, y, z), indexB, Chunk));
                        // Check if vertex is already being used by neighboring thingo
                        int triangle = SharesVertex(x, y, z, Chunk, vertex);
                        if(triangle < 0)
                        {
                            triangle = TriangleCount;
                            TriangleCount++;
                            Vertices.Add(vertex);
                            Chunk.Data[x, y, z].AddVertex(new Vertex(vertex, triangle));
                        }

                        Indices.Add(triangle);
                    }
                }
        mesh.SetVertices(Vertices);
        mesh.SetTriangles(Indices.ToArray(), 0);
        mesh.RecalculateNormals();
        Dispose();
        return mesh;
    }

    // If the vertex is already used by a neighbor, reused the index rather than creating a new one.
    private int SharesVertex (int x, int y, int z, ChunkData Chunk, Vector3 vertex)
    {
        foreach(Vector3Int neighbor in Utils.Neighbors)
        {
            Voxel voxel = TerrainData.GetVoxel(Chunk.GetGlobalVoxel(new Vector3Int(
                x + neighbor.x,
                y + neighbor.y,
                z + neighbor.z
            )));
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

    // Smooths the position of the selected voxel to create a smoother surface
    private Vector3 Smooth(float surfaceLevel, Vector3 position1, Vector3 position2, float point1, float point2)
    {
        surfaceLevel = (surfaceLevel - point1) / (point2 - point1);
        return position1 + surfaceLevel * (position2-position1);
    }

    // Returns corner of cube depending on index value
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

    // Returns the value of the selected neighboring voxel
    private float GetVoxelValue (Vector3Int p, int i, ChunkData Chunk)
    {
        Voxel targetVoxel = null;

        if (i == 0) targetVoxel = TerrainData.GetVoxel(Chunk.GetGlobalVoxel(p + new Vector3Int(0, 0, 0)));
        if (i == 1) targetVoxel = TerrainData.GetVoxel(Chunk.GetGlobalVoxel(p + new Vector3Int(1, 0, 0)));
        if (i == 2) targetVoxel = TerrainData.GetVoxel(Chunk.GetGlobalVoxel(p + new Vector3Int(1, 1, 0)));
        if (i == 3) targetVoxel = TerrainData.GetVoxel(Chunk.GetGlobalVoxel(p + new Vector3Int(0, 1, 0)));
        if (i == 4) targetVoxel = TerrainData.GetVoxel(Chunk.GetGlobalVoxel(p + new Vector3Int(0, 0, 1)));
        if (i == 5) targetVoxel = TerrainData.GetVoxel(Chunk.GetGlobalVoxel(p + new Vector3Int(1, 0, 1)));
        if (i == 6) targetVoxel = TerrainData.GetVoxel(Chunk.GetGlobalVoxel(p + new Vector3Int(1, 1, 1)));
        if (i == 7) targetVoxel = TerrainData.GetVoxel(Chunk.GetGlobalVoxel(p + new Vector3Int(0, 1, 1)));

        if (targetVoxel != null)
        {
            return targetVoxel.Value;
        } 
        else {
            return 1;
        }
    }

    private int GetCubeIndex (int x, int y, int z, ChunkData Chunk)
    {
        int index = 0;
        if (GetVoxelValue(new Vector3Int(x, y, z), 0, Chunk) > SurfaceLevel) index |= 1;
        if (GetVoxelValue(new Vector3Int(x, y, z), 1, Chunk) > SurfaceLevel) index |= 2;
        if (GetVoxelValue(new Vector3Int(x, y, z), 2, Chunk) > SurfaceLevel) index |= 4;
        if (GetVoxelValue(new Vector3Int(x, y, z), 3, Chunk) > SurfaceLevel) index |= 8;
        if (GetVoxelValue(new Vector3Int(x, y, z), 4, Chunk) > SurfaceLevel) index |= 16;
        if (GetVoxelValue(new Vector3Int(x, y, z), 5, Chunk) > SurfaceLevel) index |= 32;
        if (GetVoxelValue(new Vector3Int(x, y, z), 6, Chunk) > SurfaceLevel) index |= 64;
        if (GetVoxelValue(new Vector3Int(x, y, z), 7, Chunk) > SurfaceLevel) index |= 128;
        return index;
    }

    // Clears all data relating to the current generated mesh
    private void Dispose ()
    {
        Vertices.Clear();
        Indices.Clear();
    }
}
