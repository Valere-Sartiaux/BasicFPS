using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    public static MeshData GenerateTerrainmesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int lOD) //This function deals with generating the triangles in the mesh
    {
        int width = heightMap.GetLength(0);
        int length = heightMap.GetLength(1);
        float topLeftX = (width-1) / -2f;
        float topLeftZ = (length-1) / 2f;

        int lODIncrement = (lOD == 0)?1:lOD*2;
        int verticiesPerLine = (width-1)/lODIncrement +1;

        MeshData meshData = new MeshData (verticiesPerLine, verticiesPerLine);
        int vertexIndex = 0;

        for (int y = 0; y < length; y += lODIncrement)
        {
            for (int x = 0; x < width; x += lODIncrement) 
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX+x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ-y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)length);
                if (x < width-1 && y < length-1) 
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticiesPerLine + 1, vertexIndex + verticiesPerLine);
                    meshData.AddTriangle(vertexIndex + verticiesPerLine + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData //this class deals with the data stored inside the mesh and generating the meshes
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;


    int triangleIndex;
    public MeshData(int meshWidth, int meshLength) //this creates all the vertices and uvs of the entire mesh
    {
        vertices = new Vector3[meshWidth * meshLength];
        uvs = new Vector2[meshWidth * meshLength];

        triangles = new int[(meshWidth-1) * (meshLength-1) * 6];

    }

    public void AddTriangle(int a, int b, int c) //this keeps track of the triangles inside of the mesh
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex+1] = b;
        triangles[triangleIndex+2] = c;

        triangleIndex += 3;
    }

    public Mesh CreateMesh() // this creates the final mesh
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        return mesh;
    }
}
