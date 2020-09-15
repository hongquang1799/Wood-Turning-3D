using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningMeshProjectionBuilder : MonoBehaviour
{
    public TurningController controller;//

    protected Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        mesh.MarkDynamic();
        GetComponent<MeshFilter>().sharedMesh = mesh;

        controller.AddPostShapeChangeAction(UpdateMesh);
    }

    void UpdateMesh()
    {
        var edge = controller.Edge;

        mesh.Clear();
        if (edge.Length < 2) return;

        Vector2 basePos = controller.transform.localPosition;

        int totalVertexCount = edge.Length * 2;
        int triangleIndexCount = (edge.Length - 1) * 2 * 3;

        Vector3[] vertices = new Vector3[totalVertexCount];
        int[] triangles = new int[triangleIndexCount];

        int vertexIndex = 0;
        int triangleIndex = 0;
        for (int i = 0; i < edge.Length; i++)
        {
            float y = edge[i].y - basePos.y;
            float x = edge[i].x - basePos.x;

            vertices[vertexIndex] = new Vector3(x, y, 0f);
            vertices[vertexIndex + 1] = new Vector3(x, -y, 0f);

            if (i > 0)
            {
                triangles[triangleIndex + 0] = vertexIndex - 2;
                triangles[triangleIndex + 1] = vertexIndex;
                triangles[triangleIndex + 2] = vertexIndex - 1;
                triangles[triangleIndex + 3] = vertexIndex - 1;
                triangles[triangleIndex + 4] = vertexIndex;
                triangles[triangleIndex + 5] = vertexIndex + 1;

                triangleIndex += 6;
            }

            vertexIndex += 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}
