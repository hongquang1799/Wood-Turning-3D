using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vector2i = ClipperLib.IntPoint;
using int64 = System.Int64;

public static class MathExtension 
{
    public static float CliperLibRatio = 4096f;

    public static Vector2i ToVector2i(this Vector2 p)
    {
        return new Vector2i((int64)(p.x * CliperLibRatio), (int64)(p.y * CliperLibRatio));
    }

    public static Vector2i ToVector2i(this Vector3 p)
    {
        return new Vector2i((int64)(p.x * CliperLibRatio), (int64)(p.y * CliperLibRatio));
    }

    public static Vector2 ToVector2(this Vector2i p)
    {
        return new Vector2(p.X / CliperLibRatio, p.Y / CliperLibRatio);
    }

    public static Vector3 ToVector3(this Vector2i p)
    {
        return new Vector3(p.X / CliperLibRatio, p.Y / CliperLibRatio, 0f);
    }

    public static Vector2 ToVector2(this Vector3 p)
    {
        return new Vector2(p.x, p.y);
    }

    public static Vector3 ToVector3(this Vector2 p)
    {
        return new Vector2(p.x, p.y);
    }

    public static bool CheckSegmetsIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        return false;
    }

    public static Vector2 Cross(this Vector2 v)
    {
        return new Vector2(-v.y, v.x);
    }

    public static Vector2 CrossWithNormalization(this Vector2 v)
    {
        v.Normalize();

        return new Vector2(-v.y, v.x);
    }

    public static float NormalizeOutLength(this Vector2 v)
    {
        float length = Mathf.Sqrt(v.x * v.x + v.y * v.y);
        if (length > 0f)
        {
            v.x /= length;
            v.y /= length;
        }
        else
        {
            v.x = v.y = 0f;
        }

        return length;
    }

    public static Vector2 LimitLength(this Vector2 v, float limit)
    {
        float length = v.magnitude;
        if (length > limit)
        {
            float m = limit / length;
            v.x *= m;
            v.y *= m;
        }

        return v;
    }
}
