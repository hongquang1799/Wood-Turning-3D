using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AccuracyEvaluator
{
    public static float CalculateAccuracy()
    {
        float res = 0f;
        if (PatternShape.Current != null)
        {
            Vector2[] src = PlaySystem.Instance.targetGo.GetComponent<TurningController>().Points;
            Vector2[] dst = PatternShape.Current.TransformedPoints;
            float thresold = 10f;

            if (src != null && dst != null)
            {
                res = CalculateAccuracy(src, dst, thresold);
            }
        }      

        return res;
    }

    private static float CalculateAccuracy(Vector2[] src, Vector2[] dst, float thresold)
    {
        Vector2 pA, pB, p;
        int a = 0, b = 0;

        float accuracyAccumulation = 0f;
        int count = 0;
        pA = dst[0];
        pB = dst[1];

        while (a < src.Length)
        {
            p = src[a];

            if (p.x > dst[0].x && p.x < dst[dst.Length-1].x)
            {
                while ((pA.x > p.x || pB.x < p.x)/* && b < dst.Length - 1*/) 
                {
                    pA = dst[b];
                    pB = dst[b + 1];

                    b++;
                }
                
                float t = (p.x - pA.x) / (pB.x - pA.x);
                float d = (p.y - pA.y) - (pB.y - pA.y) * t;
                float acc = 1f - Mathf.Abs(d) / thresold;

                accuracyAccumulation += acc;
            }

            a++;
            count++;
        }

        a = 0;
        b = 0;
        pA = src[0];
        pB = src[1];
        while (a < dst.Length)
        {
            p = dst[a];

            if (p.x > src[0].x && p.x < src[src.Length - 1].x)
            {
                while ((pA.x > p.x || pB.x < p.x)/* && b < dst.Length - 1*/)
                {
                    pA = src[b];
                    pB = src[b + 1];

                    b++;
                }

                float t = (p.x - pA.x) / (pB.x - pA.x);
                float d = (p.y - pA.y) - (pB.y - pA.y) * t;
                float acc = Mathf.Abs(d) / thresold;

                accuracyAccumulation += acc;
            }

            a++;
            count++;
        }

        return accuracyAccumulation / count;
    }
}
