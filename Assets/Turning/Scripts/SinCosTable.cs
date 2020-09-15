using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SinCosTable 
{
    private static float[] sinValues;

    private static float[] cosValues;

    private static bool calculated = false;

    public static float GetSinValue(int idx)
    {
        return sinValues[idx];
    }

    public static float GetCosValue(int idx)
    {
        return cosValues[idx];
    }

    public static void Calculate(int n)
    {
        if (calculated == true) return;

        calculated = true;

        sinValues = new float[n+1];
        cosValues = new float[n+1];

        float anglePerSegment = Mathf.Deg2Rad * 360f / n;
        for (int i = 0; i <= n; i++)
        {
            float sin = Mathf.Sin(anglePerSegment * i);
            sinValues[i] = sin;

            float cos = Mathf.Cos(anglePerSegment * i);
            cosValues[i] = cos;
        }
    }
}
