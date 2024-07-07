using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//This Script Generates Perlin Noise using the Unity inbuilt perlin noice generator


public static class Noise //This creates a STATIC class called "Noise"

//Static meaning that it cannot be instantiated or derived from
//Static classes are usually there to hold methods, variables ect which don't change
//Good example: System.Math

{
    public static float[,] GenerateNoiseMap(int mapWidth,int mapLength, float amplitude) //This generates a static function which returns a 2D float Array called "GenerateNoiseMap", with the parameters of int "mapWidth", int "mapLength", float "amplitude"
    {
        float[,] noiseMap = new float[mapWidth,mapLength]; //This instance a new 2d float array called "noiseMap" with the size defined by the variables "mapWidth" and "map Length"

        if (amplitude <= 0) //This if statement checks if amplitude is 0 to avoid dividing by 0
        {
            amplitude = 0.00001f; //this sets the minimum value of amplitude to 0.00001
        }

        for (int x = 0; x < mapLength; x++) //These for loops go through every cell of the 2d float array and assigns them a float value
        {
            for (int y = 0; y < mapWidth; y++)
            {
                float sampleX = x / amplitude; //creates new float called "sampleX/Y" and divides it by the amplitude to determine the density of the map
                float sampleY = y / amplitude; //This works because it is a flaot array??? I think

                float noiseValue = Mathf.PerlinNoise(sampleX, sampleY); //this creates a noise value based on the coordinates given
                noiseMap[x,y] = noiseValue; //assigns the value to the noise map location


            }
        }

        return noiseMap;

    }

}
