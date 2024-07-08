using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    public static Texture2D TextureColourMap(Color[] colourMap, int width, int length) //creates the texture
    {
        Texture2D texture = new Texture2D(width, length); 

        texture.filterMode = FilterMode.Point; //sets the filter mode to "point" which is basically OFF, no AA or anything
        texture.wrapMode = TextureWrapMode.Clamp; //clamp makes it not wrap around at the edges like pacman
        texture.SetPixels(colourMap); //this applies the "colourmap" to the texture pixel by pixel 
        texture.Apply(); //this finishes the text
        return texture;
    }

    public static Texture2D TextureNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0); //This takes the width of the noisemap from the 2d array passed through
        int length = noiseMap.GetLength(1); //This takes the length of the noisemap from the 2d array passed throug

        Color[] colourArray = new Color[width * length]; //creates every colour needed for the texture
        for (int y = 0; y < length; y++) //this for loop assigns a colour to every pixel of the colour array depending on it's float value
        {
            for (int x = 0; x < width; x++)
            {
                colourArray[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]); //this assigns the colour based on the float value between 1 and 0, 1 being white and 0 being black
            }
        }

        return TextureColourMap(colourArray, width, length);
    }
}
