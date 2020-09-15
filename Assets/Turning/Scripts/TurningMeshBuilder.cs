using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class TurningMeshBuilder : MonoBehaviour
{
    protected Mesh mesh;

    protected TurningController controller;

    void Awake()
    {
        controller = GetComponent<TurningController>();
        controller.AddPostShapeChangeAction(UpdateMesh);

        mesh = new Mesh();
        mesh.MarkDynamic();
        GetComponent<MeshFilter>().sharedMesh = mesh;

        SinCosTable.Calculate(controller.CircumferenceSegmentCount);
    }

    struct EdgeVertexProp
    {
        public Vector2 baseNormal;
        public int edgeIndex;
        public int skip;
    }

    //private void LateUpdate()
    //{
    //    Vector3[] vertices = mesh.vertices;
    //    Vector3[] normal = mesh.normals;

    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Debug.DrawLine(vertices[i], vertices[i] + normal[i]);
    //    }
    //}

    void UpdateMesh()
    {
        float width = controller.Width;
        float height = controller.Height;
        int circumferenceSegmentCount = controller.CircumferenceSegmentCount;
        var edge = controller.Edge;
      
        mesh.Clear();
        if (edge.Length < 2) return;

        int edgeLength = edge.Length;
        NativeList<EdgeVertexProp> edgeVertexProps = new NativeList<EdgeVertexProp>(Allocator.Temp);
        edgeVertexProps.Add(new EdgeVertexProp { baseNormal = (edge[1] - edge[0]).CrossWithNormalization(), edgeIndex = 0, skip = 0});

        for (int i = 1; i < edgeLength - 1; i++)
        {
            Vector2 d1 = edge[i] - edge[i - 1];
            Vector2 d2 = edge[i + 1] - edge[i];
            float l1 = d1.NormalizeOutLength();
            float l2 = d2.NormalizeOutLength();

            if (Vector2.Dot(d1, d2) < 0.75f && (l1 + l2) > 0.5f)
            {
                edgeVertexProps.Add(new EdgeVertexProp { baseNormal = d1.Cross(), edgeIndex = i, skip = 0 });
                edgeVertexProps.Add(new EdgeVertexProp { baseNormal = d2.Cross(), edgeIndex = i, skip = 1 });
            }
            else
            {
                edgeVertexProps.Add(new EdgeVertexProp { baseNormal = d1.Cross(), edgeIndex = i, skip = 0 });
            }     
        }

        edgeVertexProps.Add(new EdgeVertexProp { baseNormal = (edge[edgeLength - 1] - edge[edgeLength - 2]).CrossWithNormalization(),
            edgeIndex = edgeLength - 1, skip = 0 });

        Vector2 basePos = transform.localPosition;
        bool beginFill = true;
        bool endFill = true;

        int heightSegmentCount = edge.Length - 1;
        int totalVertexCount = (circumferenceSegmentCount + 1) * edgeVertexProps.Length;
        int triangleIndexCount = heightSegmentCount * circumferenceSegmentCount * 6;

        if (beginFill)
        {
            totalVertexCount += circumferenceSegmentCount + 2;
            triangleIndexCount += circumferenceSegmentCount * 3;
        }

        if (endFill)
        {
            totalVertexCount += circumferenceSegmentCount + 2;
            triangleIndexCount += circumferenceSegmentCount * 3;
        }

        Vector3[] vertices = new Vector3[totalVertexCount];
        Vector2[] texCoords = new Vector2[totalVertexCount];
        Vector2[] uv2s = new Vector2[totalVertexCount];
        Vector3[] normals = new Vector3[totalVertexCount];
        int[] triangles = new int[triangleIndexCount];

        int vertexIndex = 0;
        int triangleIndex = 0;
        for (int i = 0; i < edgeVertexProps.Length; i++)
        {
            EdgeVertexProp evp = edgeVertexProps[i];
            Vector2 p = edge[evp.edgeIndex];

            float radius = p.y - basePos.y;
            float x = p.x - basePos.x;

            float nr = evp.baseNormal.y;

            float ux = (p.x - edge[0].x) / width * 0.8f + 0.1f;

            float uy2 = radius / height;

            bool addTriangle = i > 0 && evp.skip == 0;

            for (int j = 0; j <= circumferenceSegmentCount; j++)
            {
                float y = SinCosTable.GetSinValue(j);
                float z = SinCosTable.GetCosValue(j);

                vertices[vertexIndex] = new Vector3(x, y * radius, z * radius);
                texCoords[vertexIndex] = new Vector2(ux, (float)(j) / circumferenceSegmentCount * 0.8f);
                uv2s[vertexIndex] = new Vector2(0f, uy2);

                Vector3 n2 = new Vector3(evp.baseNormal.x, y * nr, z * nr);

                normals[vertexIndex] = n2.normalized;

                if (addTriangle && j < circumferenceSegmentCount)
                {
                    int a = vertexIndex;
                    int b = a + 1;
                    int c = vertexIndex - circumferenceSegmentCount - 1;
                    int d = c + 1;

                    triangles[triangleIndex + 0] = a;
                    triangles[triangleIndex + 1] = b;
                    triangles[triangleIndex + 2] = c;

                    triangles[triangleIndex + 3] = c;
                    triangles[triangleIndex + 4] = b;
                    triangles[triangleIndex + 5] = d;

                    triangleIndex += 6;
                }

                vertexIndex += 1;
            }
        }

        if (beginFill)
        {
            Vector3 normal = -Vector3.right;

            float radius = 0.09f;
            Vector2 texCoordCenter = new Vector2(0.1f, 0.9f);

            int centerIndex = vertexIndex;
            vertices[centerIndex] = new Vector3(edge[0].x - basePos.x, 0f, 0f);
            texCoords[centerIndex] = texCoordCenter;
            normals[centerIndex] = normal;

            vertexIndex++;

            for (int i = 0; i <= circumferenceSegmentCount; i++)
            {
                vertices[vertexIndex] = vertices[i];
                normals[vertexIndex] = normal;
                texCoords[vertexIndex] = new Vector2(
                    texCoordCenter.x + radius * SinCosTable.GetCosValue(i),
                    texCoordCenter.y + radius * SinCosTable.GetSinValue(i));
                //vertices[vertexIndex].x = vertices[centerIndex].x;

                if (i < circumferenceSegmentCount)
                {
                    triangles[triangleIndex + 0] = centerIndex;
                    triangles[triangleIndex + 1] = vertexIndex;
                    triangles[triangleIndex + 2] = vertexIndex + 1;

                    triangleIndex += 3;
                }

                vertexIndex++;
            }
        }

        if (endFill)
        {
            Vector3 normal = Vector3.right;

            float radius = 0.09f;
            Vector2 texCoordCenter = new Vector2(0.9f, 0.9f);

            int firstIndexOfTheLastRing = (circumferenceSegmentCount + 1) * (edgeVertexProps.Length - 1);

            int centerIndex = vertexIndex;
            vertices[centerIndex] = new Vector3(edge[heightSegmentCount].x - basePos.x, 0f, 0f);
            texCoords[centerIndex] = texCoordCenter;
            normals[centerIndex] = normal;

            vertexIndex++;

            for (int i = 0; i <= circumferenceSegmentCount; i++)
            {
                vertices[vertexIndex] = vertices[firstIndexOfTheLastRing + i];
                normals[vertexIndex] = normal;
                texCoords[vertexIndex] = new Vector2(
                    texCoordCenter.x + radius * SinCosTable.GetCosValue(i),
                    texCoordCenter.y + radius * SinCosTable.GetSinValue(i));
                //vertices[vertexIndex].x = vertices[centerIndex].x;

                if (i < circumferenceSegmentCount)
                {
                    triangles[triangleIndex + 0] = centerIndex;
                    triangles[triangleIndex + 1] = vertexIndex + 1;
                    triangles[triangleIndex + 2] = vertexIndex;

                    triangleIndex += 3;
                }

                vertexIndex++;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = texCoords;
        mesh.uv2 = uv2s;
        mesh.normals = normals;
        mesh.triangles = triangles;

        edgeVertexProps.Dispose();
    }
}
