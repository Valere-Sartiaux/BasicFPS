using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; //IMPORTANT this allows us to use the unity editor functions

//This Script auto updates the main map generator class

[CustomEditor (typeof (MapGenerator))] //this is needed to map the script show up in the Unity editor
public class MapGeneratorEditor : Editor //EDITOR is needed to interact with the unity editor
{
    public override void OnInspectorGUI() //I have no idea what override does
    {
        MapGenerator mapGen = (MapGenerator)target; //this instances the MapGenerator as "mapGen" and does some more UI trolling idk, i think it adds the custom button in the mapGenerator section 

        if (DrawDefaultInspector()) //if "something happens"
        {
            if (mapGen.autoUpdate) //checks if the autoupdate is on
            {
                mapGen.GenerateMap(); //executes the map generation again
            }
        }
        if (GUILayout.Button("Execute Generator")) //if this button is pressed do stuff
        {
            mapGen.GenerateMap(); //the thing it's doing
        }
    }
}
