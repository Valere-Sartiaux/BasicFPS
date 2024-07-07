using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This Script handles the displaying of the generated nosiemap
public class MapDisplay : MonoBehaviour //Monobehaviour is needed as it interacts other objects
{
    public Renderer textureRender; //creates a "Renderer" representation called "textureRender"

    public void DrawNoiseMap(float[,] noiseMap) //This function draws the noiseMap generated in another script and require a 2d float array to be passed through
    {
        int width = noiseMap.GetLength(0); //This takes the width of the noisemap from the 2d array passed through
        int length = noiseMap.GetLength(1); //This takes the length of the noisemap from the 2d array passed throug

        Texture2D texture = new Texture2D(width, length); //creates a new 2d texture with the specified dimentions

        Color[] colourArray = new Color[width * length]; //creates every colour needed for the texture
        for (int x = 0; x < width; x++) //this for loop assigns a colour to every pixel of the colour array depending on it's float value
        {
            for (int y = 0; y < length; y++)
            {
                colourArray[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]); //this assigns the colour based on the float value between 1 and 0, 1 being white and 0 being black
            }
        }
        texture.SetPixels(colourArray); //sets every pixel to it's appropriate colour
        texture.Apply(); //finishes the texture

        textureRender.sharedMaterial.mainTexture = texture; //set's the main texture of the material to be the texture we just created
        textureRender.transform.localScale = new Vector3(width, 1, length); //scales the texture depending on the size of the map
    }
}
