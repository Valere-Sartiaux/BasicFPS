using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//This Script is the "Hub" of all the other map generating scripts
public class MapGenerator : MonoBehaviour //Monobehaviour is needed as it interacts other objects
{
    public int seed; //this allows us to get different results

    
    public int mapWidth; //this determines the width of the map
    public int mapLength; //this determines the length of the map

    [Range(0f, 25f)]
    public int octaves; //This determines the amount of times the values are put through the generator

    [Range(0f, 1f)]
    public float persistance; //this determines the strength of the extra processing
    

    public float lacunarity; //this does some weird shit idk
    
    public float noiseScale; //this determines the density of the map
    public bool autoUpdate; //this determines of the map auto-updates when variables are changed

    public enum DrawMode{NoiseMap, ColourMap, MeshMap}; //this creates a editor specific "mode" that you can change
    public DrawMode drawMode; //this assigns the draw mode to a variable called "drawMode"

    public TerrainType[] regions; //creates a TerrainType object called "regions"

    public Vector2 offset; //this allows us to scroll through the generated map
    public void GenerateMap() //this function doesnt return anything, it executes all the other scripts
    {
        float[,] noiseMap = Noise.GenerateNoiseMap (mapWidth, mapLength, seed, noiseScale, octaves, persistance, lacunarity, offset); //creates a 2d float array called "noiseMap" using the 2dfloat array returned from the "GenerateNoiseMap" function in the "Noise" class

        Color[] colourMap = new Color[mapWidth * mapLength]; //creates a colour map the size of the texture

        for (int y = 0; y < mapLength; y++ )
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y]; 

                for (int i = 0; i < regions.Length; i++) 
                { 
                    if (currentHeight <= regions[i].altitude)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour; //applies the colour of the pixels based on the float value of that pixel
                        break;
                    }
                }
            }
        }


        MapDisplay display = FindObjectOfType<MapDisplay>(); //creates a "MapDisplay" instance called "display" and assigns it the first active loaded object of "MapDisplay" 
        
        if (drawMode == DrawMode.NoiseMap) 
        {
            display.DrawTexture(TextureGenerator.TextureNoiseMap(noiseMap)); //executes the "DrawNoiseMap" function inside the "MapDisplay" class with the parameter of "nosieMap"
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureColourMap(colourMap, mapWidth, mapLength));
        }
        else if (drawMode == DrawMode.MeshMap)
        {
            display.DrawMeshMap(MeshGenerator.GenerateTerrainmesh(noiseMap), TextureGenerator.TextureColourMap(colourMap, mapWidth, mapLength));
        }
    }

    public void OnValidate() //this checks that values aren't too low as to cause math errors
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapLength < 1) 
        {
            mapLength = 1;
        }
        if (lacunarity < 0.1f)
        {
            lacunarity = 0.1f;
        }
        if (octaves < 1)
        {
            octaves = 1;
        }
        if (noiseScale <= 0) //This if statement checks if scale is 0 to avoid dividing by 0
        {
            noiseScale = 0.01f; //this sets the minimum value of scale to 0.00001
        }

    }
}



[System.Serializable] //this allows the struct to be editable in the inspector
public struct TerrainType //structs to store variables
{
    public string name;
    [Range(0f, 1f)]
    public float altitude;
    public Color colour;
}
