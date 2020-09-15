using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningShapeCreator : MonoBehaviour
{
    public Vector2 offset;

    public int segment;

    public float width;

    public float height;

    void Update()
    {
        Vector2[] points = new Vector2[segment + 3];
        for (int i = 0; i <= segment; i++)
        {
            float angle = Mathf.Deg2Rad * (180 / segment * i);

            float x = width / 2f * Mathf.Cos(angle) + offset.x;
            float y = width / 2f * Mathf.Sin(angle) + height + offset.y;
            points[i] = new Vector2(x, y);
        }

        points[segment + 1] = new Vector2(-width / 2, 0f);
        points[segment + 2] = new Vector2(width / 2, 0f);

        GetComponent<PolygonCollider2D>().points = points;
    }
}
