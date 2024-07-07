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

    public Vector2 offset; //this allows us to scroll through the generated map
    public void GenerateMap() //this function doesnt return anything, it executes all the other scripts
    {
        float[,] noiseMap = Noise.GenerateNoiseMap (mapWidth, mapLength, seed, noiseScale, octaves, persistance, lacunarity, offset); //creates a 2d float array called "noiseMap" using the 2dfloat array returned from the "GenerateNoiseMap" function in the "Noise" class

        MapDisplay display = FindObjectOfType<MapDisplay>(); //creates a "MapDisplay" instance called "display" and assigns it the first active loaded object of "MapDisplay" 
        display.DrawNoiseMap (noiseMap); //executes the "DrawNoiseMap" function inside the "MapDisplay" class with the parameter of "nosieMap"
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
