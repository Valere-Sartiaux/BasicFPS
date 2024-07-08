using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This Script handles the displaying of the generated nosiemap
public class MapDisplay : MonoBehaviour //Monobehaviour is needed as it interacts other objects
{
    public Renderer textureRender; //creates a "Renderer" representation called "textureRender"

    public void DrawTexture(Texture2D texture) 
    {
        textureRender.sharedMaterial.mainTexture = texture; //set's the main texture of the material to be the texture we just created
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height); //scales the texture depending on the size of the map
    }
}
