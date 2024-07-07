using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Script is the "Hub" of all the other map generating scripts
public class MapGenerator : MonoBehaviour //Monobehaviour is needed as it interacts other objects
{
    public int mapWidth; //this determines the width of the map
    public int mapLength; //this determines the length of the map

    public float noiseAmplitude; //this determines the density of the map

    public bool autoUpdate; //this determines of the map auto-updates when variables are changed


    public void GenerateMap() //this function doesnt return anything, it executes all the other scripts
    {
        float[,] noiseMap = Noise.GenerateNoiseMap (mapWidth, mapLength, noiseAmplitude); //creates a 2d float array called "noiseMap" using the 2dfloat array returned from the "GenerateNoiseMap" function in the "Noise" class

        MapDisplay display = FindObjectOfType<MapDisplay>(); //creates a "MapDisplay" instance called "display" and assigns it the first active loaded object of "MapDisplay" 
        display.DrawNoiseMap (noiseMap); //executes the "DrawNoiseMap" function inside the "MapDisplay" class with the parameter of "nosieMap"
    }
}
