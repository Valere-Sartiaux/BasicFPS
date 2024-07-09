using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FallOffMapGenerator
{
    public static float[,] GenerateFallOffMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2f - 1f;
                float y = j / (float)size * 2f - 1f;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = EvaluatationCurve(value);

            }
        }

        return map;
    }

    static float EvaluatationCurve(float value)
    {
        float a = 3f;
        float b = 2.2f;

        return Mathf.Pow(value,a)/(Mathf.Pow(value,a) + Mathf.Pow(b-b * value,a));
    }
}
