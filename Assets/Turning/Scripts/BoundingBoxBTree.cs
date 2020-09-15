using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoundingBox
{
    public BoundingBox(Vector2 p1, Vector2 p2)
    {
        if (p1.x < p2.x)
        {
            pointA.x = p1.x;
            pointB.x = p2.x;
        }
        else
        {
            pointA.x = p2.x;
            pointB.x = p1.x;
        }

        if (p1.y < p2.y)
        {
            pointA.y = p1.y;
            pointB.y = p2.y;
        }
        else
        {
            pointA.y = p2.y;
            pointB.y = p1.y;
        }
    }

    public bool Overlap(BoundingBox other)
    {
        return !(other.pointA.x > pointB.x || other.pointB.x < pointA.x || other.pointB.y < pointA.y || other.pointA.y > pointB.y);
    }

    public Vector2 pointA;
    public Vector2 pointB;
}

public class Shape
{
    

    public List<Vector2> points;
    public BoundingBox bound;
}

public class BoundingBoxBTree 
{
    
}
