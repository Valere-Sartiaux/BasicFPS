using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    const float scale = 5f;
    const float playerUpdateThreshold = 25f;
    const float sqrPlayerUpdateThreshold = playerUpdateThreshold * playerUpdateThreshold;


    public LODInfo[] detailAmounts;
    public static float maxViewDistance = 500f; //max viewdistance
    public Transform playerCam; //stores the players transform


    public Material mapMaterial;
    public static UnityEngine.Vector2 playerPosition; //stores the players x&y
    UnityEngine.Vector2 playerPositionOld;
    
    static MapGenerator mapGenerator;


    int chunkSize;
    int chunkVisibleInViewDistance;

    Dictionary<UnityEngine.Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<UnityEngine.Vector2, TerrainChunk>(); //This stores terrain chunks which have been generated, it records it's vector2 position and the chunk
    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>(); //This stores terrain chunks which was redered last update


    //------------------------------------------------------------------------------------------
    //---------------------------------RUNTIME------------------------------------------
    //------------------------------------------------------------------------------------------


    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();

        maxViewDistance = detailAmounts[detailAmounts.Length-1].visibleDistThreshold;
        chunkSize = MapGenerator.chunkSize - 1; //sets the chunk size to it's correct size
        chunkVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize); //calculates the amount of chunks visible by the player
        UpdateVisibleChunks();
    }

    private void Update()
    {
        playerPosition = new UnityEngine.Vector2(playerCam.position.x, playerCam.position.z) / scale; //This checks player position every frame
        
        if ((playerPositionOld - playerPosition).sqrMagnitude > sqrPlayerUpdateThreshold)
        {
            playerPositionOld = playerPosition;
            UpdateVisibleChunks();
        }
    }
    void UpdateVisibleChunks() //this function loads and unloads chunks within the players view
    {
        for(int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)  //This goes through and unloads previous chunks
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false); //sets the last chunks to false
        }
        terrainChunksVisibleLastUpdate.Clear(); //this clears the list
        






        int currentChunkX = Mathf.RoundToInt(playerPosition.x / chunkSize); //calculates the chunk position based on it's coordinates compared to other coords 
        int currentChunkY = Mathf.RoundToInt(playerPosition.y / chunkSize); //calculates the chunk position based on it's coordinates compared to other coords 

        for (int yOffset = -chunkVisibleInViewDistance; yOffset <= chunkVisibleInViewDistance; yOffset++) //goes through all chunks potentially visible by the player
        {
            for (int xOffset = -chunkVisibleInViewDistance; xOffset <= chunkVisibleInViewDistance; xOffset++)
            {
                UnityEngine.Vector2 viewedChunkCoordinate = new UnityEngine.Vector2(currentChunkX + xOffset, currentChunkY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoordinate))  //this checks if the chunk has already been generated
                {
                    terrainChunkDictionary[viewedChunkCoordinate].UpdateTerrainChunk(); //sets the chunk that has already been generated as visible

                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoordinate, new TerrainChunk(viewedChunkCoordinate, chunkSize, detailAmounts, transform, mapMaterial)); //creates a new terrain chunk
                }
            }
        }
    }



    //------------------------------------------------------------------------------------------
    //---------------------------------TERRAIN GENERATION---------------------------------------
    //------------------------------------------------------------------------------------------

    public class TerrainChunk
    {


        GameObject meshObject;
        UnityEngine.Vector2 position;
        Bounds bounds;

        
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        LODInfo[] detailAmounts;
        LODMesh[] lODMeshes;
        MapData mapData;
        bool mapDataRecieved;
        int previousLODIndex = -1;

        public TerrainChunk(UnityEngine.Vector2 coord, int size, LODInfo[] detailAmounts, Transform parent, Material material) 
        {
            this.detailAmounts = detailAmounts;


            position = coord * size;
            bounds = new Bounds(position, UnityEngine.Vector2.one*size);
            Vector3 position3D = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("TerrainChunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;


            meshObject.transform.position = position3D * scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = UnityEngine.Vector3.one * scale;
            SetVisible(false);

            lODMeshes = new LODMesh[detailAmounts.Length];

            for (int i = 0; i < detailAmounts.Length; i++)
            {
                lODMeshes[i] = new LODMesh(detailAmounts[i].lOD, UpdateTerrainChunk);
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived); 
        }


        void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataRecieved = true;

            Texture2D texture = TextureGenerator.TextureColourMap(mapData.colourMap, MapGenerator.chunkSize, MapGenerator.chunkSize);
            meshRenderer.material.mainTexture = texture;
            UpdateTerrainChunk();
        }




        public void UpdateTerrainChunk()
        {
            if (mapDataRecieved)
            {
                float playerDistToEdge = Mathf.Sqrt(bounds.SqrDistance(playerPosition));
                bool visible = playerDistToEdge <= maxViewDistance;

                if (visible)
                {
                    int lODIndex = 0;

                    for (int i = 0; i < detailAmounts.Length - 1; i++)
                    {
                        if (playerDistToEdge > detailAmounts[i].visibleDistThreshold)
                        {
                            lODIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (lODIndex != previousLODIndex)
                    {
                        LODMesh lODMesh = lODMeshes[lODIndex];
                        if (lODMesh.hasMesh)
                        {
                            previousLODIndex = lODIndex;
                            meshFilter.mesh = lODMesh.mesh;
                        }
                        else if (!lODMesh.hasRequestedMesh)
                        {
                            lODMesh.RequestMesh(mapData);
                        }
                    }

                    terrainChunksVisibleLastUpdate.Add(this); //add to the list
                }


                SetVisible(visible);
            }            
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }






    //------------------------------------------------------------------------------------------
    //---------------------------------DATA STUFF------------------------------------------
    //------------------------------------------------------------------------------------------


    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lOD;
        System.Action updateCallBack;
        public LODMesh(int lOD, System.Action updateCallBack)
        {
            this.lOD = lOD;
            this.updateCallBack = updateCallBack;
        }

        void OnMeshDataRecieved(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;

            updateCallBack();
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData,lOD, OnMeshDataRecieved);
        }

        

    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lOD;
        public float visibleDistThreshold;
    }

}
