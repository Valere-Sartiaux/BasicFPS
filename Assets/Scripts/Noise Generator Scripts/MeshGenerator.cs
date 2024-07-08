using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    public static MeshData GenerateTerrainmesh(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int length = heightMap.GetLength(1);
        float topLeftX = (width-1) / -2f;
        float topLeftZ = (length-1) / 2f;

        MeshData meshData = new MeshData (width, length);
        int vertexIndex = 0;

        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < width; x++) 
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX+x, heightMap[x, y], topLeftZ-y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)length);
                if (x < width-1 && y < length-1) 
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;


    int triangleIndex;
    public MeshData(int meshWidth, int meshLength)
    {
        vertices = new Vector3[meshWidth * meshLength];
        uvs = new Vector2[meshWidth * meshLength];

        triangles = new int[(meshWidth-1) * (meshLength-1) * 6];

    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex+1] = b;
        triangles[triangleIndex+2] = c;

        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        return mesh;
    }
}
