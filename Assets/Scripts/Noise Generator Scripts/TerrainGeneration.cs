using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public const float maxViewDistance = 500f; //max viewdistance
    public Transform playerCam; //stores the players transform

    public static Vector2 playerPosition; //stores the players x&y

    int chunkSize;
    int chunkVisibleInViewDistance;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>(); //This stores terrain chunks which have been generated, it records it's vector2 position and the chunk
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>(); //This stores terrain chunks which was redered last update


    private void Start()
    {
        chunkSize = MapGenerator.chunkSize - 1; //sets the chunk size to it's correct size
        chunkVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize); //calculates the amount of chunks visible by the player
    }

    private void Update()
    {
        playerPosition = new Vector2(playerCam.position.x, playerCam.position.z); //This checks player position every frame
        UpdateVisibleChunks();
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
                Vector2 viewedChunkCoordinate = new Vector2(currentChunkX + xOffset, currentChunkY + yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoordinate))  //this checks if the chunk has already been generated
                {
                    terrainChunkDictionary[viewedChunkCoordinate].UpdateTerrainChunk(); //sets the chunk that has already been generated as visible
                    if (terrainChunkDictionary[viewedChunkCoordinate].IsVisible ()) //if it has been set to visible
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoordinate]); //add to the list
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoordinate, new TerrainChunk(viewedChunkCoordinate, chunkSize, transform)); //creates a new terrain chunk
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        public TerrainChunk(Vector2 coord, int size, Transform parent) 
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one*size);
            Vector3 position3D = new Vector3(position.x, 0, position.y);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = position3D;
            meshObject.transform.localScale = Vector3.one * size/10f;
            meshObject.transform.parent = parent;
            SetVisible(false);

                
        }


        public void UpdateTerrainChunk()
        {
            float playerDistToEdge = Mathf.Sqrt(bounds.SqrDistance(playerPosition));
            bool visible = playerDistToEdge <= maxViewDistance;
            SetVisible(visible);
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

}
