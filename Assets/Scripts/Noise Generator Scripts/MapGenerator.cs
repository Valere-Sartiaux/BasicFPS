using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using System.Numerics;

//This Script is the "Hub" of all the other map generating scripts
public class MapGenerator : MonoBehaviour //Monobehaviour is needed as it interacts other objects
{

    //------------------------------------------------------------------------------------------
    //---------------------------------VARIABLE INITIALISATION----------------------------------
    //------------------------------------------------------------------------------------------

    public Noise.NormalisationMode normalisation;
    public int seed; //this allows us to get different results

    public const int chunkSize = 241; //actuall size of mesh is 240x240, dont get confused
    [Range(0, 6)]
    public int levelOfDetailEditor; //Level of detail amount

    [Range(0f, 100f)]
    public float mapHeight; //height multiplier
    public AnimationCurve mapHeightCurve; //height animation curve

    [Range(0f, 25f)]
    public int octaves; //This determines the amount of times the values are put through the generator

    [Range(0f, 1f)]
    public float persistance; //this determines the strength of the extra processing


    public float lacunarity; //this does some weird shit idk

    public float noiseScale; //this determines the density of the map

    public bool useFalloff;
    public bool autoUpdate; //this determines of the map auto-updates when variables are changed

    public enum DrawMode { NoiseMap, ColourMap, MeshMap, FalloffMap }; //this creates a editor specific "mode" that you can change
    public DrawMode drawMode; //this assigns the draw mode to a variable called "drawMode"

    public TerrainType[] regions; //creates a TerrainType object called "regions"

    float[,] falloffMap;

    public UnityEngine.Vector2 offset; //this allows us to scroll through the generated map

    void Awake()
    {
        falloffMap = FallOffMapGenerator.GenerateFallOffMap(chunkSize);
    }








    //------------------------------------------------------------------------------------------
    //---------------------------------THREADING STUFF------------------------------------------
    //------------------------------------------------------------------------------------------
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>(); //This is the thread for the MapData Generation
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>(); //This is the thread for the MeshData Generation
    public void RequestMapData(UnityEngine.Vector2 center, Action<MapData> callback) //this request the mapData and starts the generate map thread when the callback is recieved
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(UnityEngine.Vector2 center, Action<MapData> callback) //this generates the mapdata
    {
        MapData mapData = GenerateMapData(center);
        lock (mapDataThreadInfoQueue) //This prevents other thread from accessing this function, making them wait for their turn
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
        

    }

    public void RequestMeshData(MapData mapData, int lOD, Action<MeshData> callback) //this request the meshData and starts the generate map thread when the callback is recieved
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lOD, callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData,int lOD, Action<MeshData> callback) //this generates the mapdata
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, mapHeight, mapHeightCurve, lOD);
        lock (meshDataThreadInfoQueue) //This prevents other thread from accessing this function, making them wait for their turn
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }


    }

    void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0) //this goes through and processes the mapdata function that are in the queue
        { 
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) 
            { 
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0) //this goes through and processes the meshdata function that are in the queue
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }







    //------------------------------------------------------------------------------------------
    //---------------------------------GENERATION AND RENDERING---------------------------------
    //------------------------------------------------------------------------------------------

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData(UnityEngine.Vector2.zero);


        MapDisplay display = FindObjectOfType<MapDisplay>(); //creates a "MapDisplay" instance called "display" and assigns it the first active loaded object of "MapDisplay" 

        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureNoiseMap(mapData.noiseMap)); //executes the "DrawNoiseMap" function inside the "MapDisplay" class with the parameter of "nosieMap"
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureColourMap(mapData.colourMap, chunkSize, chunkSize));
        }
        else if (drawMode == DrawMode.MeshMap)
        {
            display.DrawMeshMap
                (
                MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, mapHeight, mapHeightCurve, levelOfDetailEditor), TextureGenerator.TextureColourMap(mapData.colourMap, chunkSize, chunkSize));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.DrawTexture
                (
                TextureGenerator.TextureNoiseMap
                    (
                    FallOffMapGenerator.GenerateFallOffMap
                        (
                            chunkSize
                        )
                    )
                );
        }
    }

    MapData GenerateMapData(UnityEngine.Vector2 center) //this function doesnt return anything, it executes all the other scripts
    {
        float[,] noiseMap = Noise.GenerateNoiseMap (chunkSize, chunkSize, seed, noiseScale, octaves, persistance, lacunarity, center + offset, normalisation); //creates a 2d float array called "noiseMap" using the 2dfloat array returned from the "GenerateNoiseMap" function in the "Noise" class

        Color[] colourMap = new Color[chunkSize * chunkSize]; //creates a colour map the size of the texture

        for (int y = 0; y < chunkSize; y++ )
        {
            for (int x = 0; x < chunkSize; x++)
            {
                if (useFalloff)
                {
                    noiseMap[x,y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
                float currentHeight = noiseMap[x, y]; 

                for (int i = 0; i < regions.Length; i++) 
                { 
                    if (currentHeight >= regions[i].altitude)
                    {
                        colourMap[y * chunkSize + x] = regions[i].colour; //applies the colour of the pixels based on the float value of that pixel
                        
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colourMap);

    }

    public void OnValidate() //this checks that values aren't too low as to cause math errors
    {

        falloffMap = FallOffMapGenerator.GenerateFallOffMap(chunkSize);

        if (lacunarity < 0.1f)
        {
            lacunarity = 0.1f;
        }
        if (octaves < 1)
        {
            octaves = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }

        if (noiseScale <= 0) //This if statement checks if scale is 0 to avoid dividing by 0
        {
            noiseScale = 0.01f; //this sets the minimum value of scale to 0.00001
        }

    }




}

//------------------------------------------------------------------------------------------
//---------------------------------DATA STORAGE--------------------------------------------
//------------------------------------------------------------------------------------------

[System.Serializable] //this allows the struct to be editable in the inspector
public struct TerrainType //structs to store variables
{
    public string name;
    [Range(0f, 1f)]
    public float altitude;
    public Color colour;
}

public struct MapData //struct to store the mapdata
{
    public readonly float[,] noiseMap;
    public readonly Color[] colourMap;

    public  MapData(float[,] noiseMap, Color[] colourMap)
    {
        this.noiseMap = noiseMap;
        this.colourMap = colourMap;
    }
}


