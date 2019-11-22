using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{

    [SerializeField, Range(0,1)] private float surfaceLevel = 0.5f;
    private DataManager dataManager;
    [SerializeField] Vector3Int WorldSize;
    private float previousSurfaceLevel = 0.0f;

    void Start()
    {
        dataManager = new DataManager(WorldSize.x, WorldSize.y, WorldSize.z);
        dataManager.Generate();
        MeshGenerator meshgen = new MeshGenerator();
        GetComponent<MeshFilter>().mesh = meshgen.Generate(dataManager.GetData(), surfaceLevel);
    }

    void Update () {
        if(surfaceLevel != previousSurfaceLevel) {
            previousSurfaceLevel = surfaceLevel;
            MeshGenerator meshgen = new MeshGenerator();
            GetComponent<MeshFilter>().mesh = meshgen.Generate(dataManager.GetData(), surfaceLevel);
        }
    }

    void OnDrawGizmosSelected () {
        for(int x = 0; x < WorldSize.x; x++)
            for(int y = 0; y < WorldSize.y; y++)
                for(int z = 0; z < WorldSize.z; z++) {
                    if(dataManager.GetData()[x, y, z] < surfaceLevel) {
                        Gizmos.DrawCube(new Vector3(x, y, z), Vector3.one / 4);
                    }
                }
    }
}
