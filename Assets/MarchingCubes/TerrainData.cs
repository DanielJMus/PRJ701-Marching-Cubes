using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainData : MonoBehaviour
{
        // PUBLIC FIELDS

        // Size of each chunk in the world
        public static Vector3Int ChunkSize = new Vector3Int(32, 32, 32);

        public static int RenderDistance = 16;

        // Currently loaded chunks (have data)
        public static Dictionary<Vector3Int, ChunkData> LoadedChunks = new Dictionary<Vector3Int, ChunkData>();

        // Main chunk preset
        [SerializeField] private GameObject ChunkPrefab = null;

        private Transform MainCam = null;


        // PUBLIC METHODS

        void Start ()
        {
            MainCam = Camera.main.transform;
            // CreateChunkMesh(new Vector3Int(32, 0, 0));
        }

        private Vector3Int PreviousCamChunk = new Vector3Int(-10000, 0, 0);

        void Update ()
        {
            Vector3Int CamChunk = GetChunkByCoordinate(MainCam.position.x, MainCam.position.y, MainCam.position.z);
            if(CamChunk != PreviousCamChunk)
            {
                PreviousCamChunk = CamChunk;
                CreateChunkMesh(CamChunk);
                StartCoroutine(LoadNeighborChunks(CamChunk));
            }
        }

        IEnumerator LoadNeighborChunks (Vector3Int CamChunk)
        {
            foreach(Vector3Int neighbor in Utils.ChunkNeighbors)
            {
                CreateChunkMesh(CamChunk + neighbor);
                yield return null;
            }
        }

        void CreateChunkData (Vector3Int Position)
        {
            DataGenerator dataGenerator = new DataGenerator();
            GameObject chunkGO = Instantiate(ChunkPrefab, Position, Quaternion.identity);
            chunkGO.name = Position.ToString();
            ChunkData Chunk = chunkGO.GetComponent<ChunkData>();
            Chunk.Position = Position;
            LoadedChunks.Add(Position, Chunk);
            dataGenerator.Generate(Chunk);
        }

        void CreateChunkMesh (Vector3Int Position)
        {
            foreach (Vector3Int neighbor in Utils.ChunkNeighbors)
            {
                if (!LoadedChunks.ContainsKey(Position + neighbor))
                {
                    CreateChunkData(Position + neighbor);
                }
            }
            ChunkData Chunk = LoadedChunks[Position];
            if(!Chunk.IsDirty) return;
            MarchingCubes meshGen = new MarchingCubes();
            Chunk.MeshData = meshGen.GenerateMesh(Chunk);
            Chunk.IsDirty = false;
        }

        // Returns the voxel at the specified global position (Returns null if voxel has not been generated.)
        public static Voxel GetVoxel (Vector3Int v)
        {
            Vector3Int ChunkCoordinate = GetChunkByCoordinate(v.x, v.y, v.z);
            ChunkData chunk;
            if (LoadedChunks.TryGetValue(ChunkCoordinate, out chunk))
            {
                return chunk.GetLocalVoxel(GlobalToChunk(v.x, v.y, v.z));
            }
            else
            {
                return null;
            }
        }

        // Returns the coordinates of the chunk the voxel resides in
        public static Vector3Int GetChunkByCoordinate(float x, float y, float z)
        {
            return new Vector3Int(
                (int)Mathf.Floor(x / ChunkSize.x) * ChunkSize.x,
                (int)Mathf.Floor(y / ChunkSize.y) * ChunkSize.y,
                (int)Mathf.Floor(z / ChunkSize.z) * ChunkSize.z
            );
        }

        // Converts global coordinate into local chunk coordinate
        public static Vector3Int GlobalToChunk (int x, int y, int z)
        {
            int xPosition = (x % ChunkSize.x);
            int yPosition = (y % ChunkSize.y);
            int zPosition = (z % ChunkSize.z);

            if (xPosition < 0) xPosition = ChunkSize.x - Mathf.Abs(xPosition);
            if (yPosition < 0) yPosition = ChunkSize.y - Mathf.Abs(yPosition);
            if (zPosition < 0) zPosition = ChunkSize.z - Mathf.Abs(zPosition);

            return new Vector3Int(xPosition, yPosition, zPosition);
        }
}
