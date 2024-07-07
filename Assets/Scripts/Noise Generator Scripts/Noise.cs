using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//This Script Generates Perlin Noise using the Unity inbuilt perlin noice generator


public static class Noise //This creates a STATIC class called "Noise"

//Static meaning that it cannot be instantiated or derived from
//Static classes are usually there to hold methods, variables ect which don't change
//Good example: System.Math

{
    public static float[,] GenerateNoiseMap(int mapWidth,int mapLength, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) //This generates a static function which returns a 2D float Array called "GenerateNoiseMap", with the parameters of int "mapWidth", int "mapLength", float "amplitude"
    {
        float[,] noiseMap = new float[mapWidth,mapLength]; //This instance a new 2d float array called "noiseMap" with the size defined by the variables "mapWidth" and "map Length"

        System.Random rand = new System.Random(seed); //This creating an instances of the random class using the int variable "seed" as a parameter
        Vector2[] octaveOffsets = new Vector2[octaves]; //This creates a new 2d vector array the size of the amount of octaves

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rand.Next(-100000, 100000) + offset.x; //This creates a random offset vector to get a more random result from the perlinnoise
            float offsetY = rand.Next(-100000, 100000) + offset.y; //offset.x/y is there to scroll through the perlin noise
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0) //This if statement checks if scale is 0 to avoid dividing by 0
        {
            scale = 0.01f; //this sets the minimum value of scale to 0.00001
        }

        float maxNoiseHeight = float.MinValue; //float.MinValue is the smallest possible value a float can have
        float minNoiseHeight = float.MaxValue;//float.MaxValue is the largest possible value a float can have

        float halfWidth = mapWidth / 2f;
        float halfLength = mapLength / 2f;



        for (int x = 0; x < mapWidth; x++) //These for loops go through every cell of the 2d float array and assigns them a float value
        {
            for (int y = 0; y < mapLength ; y++)
            {

                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                for (int i = 0; i < octaves; i++) //This goes through every octaves
                {


                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x; //This creates different sample locations for the perlin noise generator
                    float sampleY = (y-halfLength) / scale * frequency + octaveOffsets[i].y; //The higher the frequency, the further each point gets from one another, the octave

                    float noiseValue = Mathf.PerlinNoise(sampleY, sampleX) * 2 - 1; //this creates a noise value based on the coordinates given, multiply by two and minus one to give use some negative numbers

                    noiseHeight += noiseValue * amplitude;

                    amplitude *= persistance;

                    frequency *= lacunarity;

                    
                }

                if (noiseHeight > maxNoiseHeight) //keeps track of the highest and lowest values to that it can be interperlated later
                {
                    maxNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight; //assigns the value to the noise map location
            }
        }

        for (int x = 0; x < mapWidth; x++) //These for loops go through every cell of the 2d float array and assigns them a float value
        {
            for (int y = 0; y < mapLength ; y++)
            {
                noiseMap[x,y] = Mathf.InverseLerp(maxNoiseHeight, minNoiseHeight, noiseMap[x,y]); //This interperlates the given numbers into a number between 0-1, if it isnt then the noise map values will just output white or black
            }
        }



        return noiseMap;

    }

}
