using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternShape : ScriptableObject
{
    public static PatternShape Current { get; set; }

    public static Mesh BuildMeshFromShape(PatternShape shape)
    {
        var points = shape.TransformedPoints;
        Vector3[] vertices = new Vector3[points.Length * 2];
        int[] triangles = new int[(points.Length - 1) * 2 * 3];

        int vertIndex = 0;
        int triangleIndex = 0;
        for (int i = 0; i < points.Length; i++)
        {
            vertices[vertIndex] = points[i];
            vertices[vertIndex + 1] = vertices[vertIndex];
            vertices[vertIndex + 1].y = -vertices[vertIndex].y;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = vertIndex - 2;
                triangles[triangleIndex + 1] = vertIndex;
                triangles[triangleIndex + 2] = vertIndex - 1;
                triangles[triangleIndex + 3] = vertIndex - 1;
                triangles[triangleIndex + 4] = vertIndex;
                triangles[triangleIndex + 5] = vertIndex + 1;

                triangleIndex += 6;
            }

            vertIndex += 2;
        }

        var mesh =  new Mesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        return mesh;
    }

    public Vector2 offset = Vector2.zero;

    public Vector2 scale = Vector2.one;

    public Vector2[] points;

    public Vector2[] TransformedPoints
    {
        get
        {
            Vector2[] ps = new Vector2[points.Length];
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i] = points[i] * scale + offset;
            }

            return ps;
        }
    }
}
