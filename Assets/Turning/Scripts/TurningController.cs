using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.EventSystems;

using Vector2i = ClipperLib.IntPoint;
using Polygon = System.Collections.Generic.List<ClipperLib.IntPoint>;

public class TurningController : MonoBehaviour
{
    [SerializeField] protected float width = 1f;
    [SerializeField] protected float height = 1f;
    [SerializeField] protected int circumferenceSegmentCount = 50;
    

    protected NativeList<Vector2> edge;

    protected NativeList<BoundingBox> bounds;

    protected NativeList<float> lengthBuffer;

    protected Polygon polygon = new Polygon();

    protected Action postChangeShapeAction;

    public float Width { get { return width; } }
    public float Height { get { return height; } }
    public int CircumferenceSegmentCount { get { return circumferenceSegmentCount; } }
    public NativeList<Vector2> Edge { get { return edge; } }
    public NativeList<float> LengthBuffer { get { return lengthBuffer; } }
    public Vector2[] Points
    {
        get
        {
            Vector2[] points = null;

            if (polygon != null)
            {
                points = new Vector2[polygon.Count - 2];

                for (int i = 1; i < polygon.Count - 1; i++)
                {
                    Vector2 p = polygon[i].ToVector2(); p.y = -p.y;
                    points[i-1] = p;
                }
            }

            return points;
        }
    }

    public void AddPostShapeChangeAction(Action action)
    {
        postChangeShapeAction += action;
    }

    public void PostShapeChange()
    {
        postChangeShapeAction?.Invoke();
    }

    private void Awake()
    {
        edge = new NativeList<Vector2>(Allocator.Persistent);
        lengthBuffer = new NativeList<float>(Allocator.Persistent);
        bounds = new NativeList<BoundingBox>(Allocator.Persistent);
    }

    private void OnDestroy()
    {
        if (edge.IsCreated)
            edge.Dispose();

        if (bounds.IsCreated)
            bounds.Dispose();

        if (lengthBuffer.IsCreated)
            lengthBuffer.Dispose();
    }

    void Start()
    {
        if (polygon == null || polygon.Count < 3)
            ResetState();
        else
        {
            ExtractEdge();

            PostShapeChange();
        }
    }

    public void LateUpdate()
    {
        Vector3 basePos = transform.position;
        for (int i = 1; i < polygon.Count; i++)
        {
            Debug.DrawLine(basePos + polygon[i].ToVector3(), basePos + polygon[i-1].ToVector3());
        }
        Debug.DrawLine(basePos + polygon[0].ToVector3(), basePos + polygon[polygon.Count-1].ToVector3());
    }

    public void ResetState()
    {
        Vector2 basePos = transform.localPosition;

        polygon.Clear();
        polygon.Add(new Vector2(-width / 2f + basePos.x, 0f + basePos.y).ToVector2i());
        polygon.Add(new Vector2(-width / 2f + basePos.x, -height + basePos.y).ToVector2i());
        polygon.Add(new Vector2(+width / 2f + basePos.x, -height + basePos.y).ToVector2i());
        polygon.Add(new Vector2(+width / 2f + basePos.x, 0f + basePos.y).ToVector2i());

        ExtractEdge();

        PostShapeChange();
    }

    public bool OverlapChisel(List<Vector2> points, BoundingBox bound)
    {
        for (int i = 0; i < bounds.Length; i++)
        {
            if (bounds[i].Overlap(bound))
            {
                return true;
                //for (int j = 1; j < points.Count; j++)
                //{

                //}
            }
        }



        return false;
    }

    public void Clip(TurningChisel chisel)
    {
        if (polygon == null) return;

        List<Polygon> solution = new List<Polygon>();

        ClipperLib.Clipper clipper = new ClipperLib.Clipper();
        clipper.AddPolygon(polygon, ClipperLib.PolyType.ptSubject);
        clipper.AddPolygon(chisel.Polygon, ClipperLib.PolyType.ptClip);
        clipper.Execute(ClipperLib.ClipType.ctDifference, solution, ClipperLib.PolyFillType.pftNonZero, ClipperLib.PolyFillType.pftNonZero);

        polygon = null;
        if (solution.Count > 0)
        {
            Vector2i basePosition = transform.localPosition.ToVector2i();

            polygon = null;
            for (int i = 0; i < solution.Count; i++)
            {
                var poly = solution[i];
                float xA = (float)(poly[1].X - basePosition.X);
                float xB = (float)(poly[poly.Count - 2].X - basePosition.Y);

                if (xA * xB < 0f)
                {
                    polygon = poly;
                }
                else
                {
                    StartCoroutine(CreateDetachedObject(poly));
                }
            }
        }

        if (polygon != null)
        {
            ExtractEdge();
            PostShapeChange();
        }
        else
        {
            polygon = null;
            ExtractEdge();
            PostShapeChange();
        }
    }

    IEnumerator CreateDetachedObject(Polygon polygon)
    {
        GameObject go = new GameObject("DetachedSubject");

        var go_transform = go.transform;
        var this_transform = transform;

        go_transform.localPosition = this_transform.localPosition;
        go_transform.localRotation = this_transform.localRotation;

        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
        var turningController = go.AddComponent<TurningController>();
        turningController.width = width;
        turningController.height = height;
        turningController.circumferenceSegmentCount = circumferenceSegmentCount;
        turningController.polygon = polygon;

        var turningMeshBuilder = go.AddComponent<TurningMeshBuilder>();

        var rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.AddRelativeTorque(new Vector3(1000f, 0f, 0f));
        rb.AddForce(new Vector3(Mathf.Sign(polygon[0].X) * 100f, 0f, 0f), ForceMode.Force);

        yield return null;

        float time = 0f;

        while (time < 7.5f)
        {
            time += Time.deltaTime;

            rb.AddForce(new Vector3(0f, 0f, 40f), ForceMode.Force);

            yield return null;
        }

        Destroy(go);
    }

    void ExtractEdge()
    {
        edge.Clear();
        lengthBuffer.Clear();
        bounds.Clear();

        if (polygon == null) return;

        float len, len_prev = 0f;
        Vector2 p, p_prev = polygon[1].ToVector2();
        p_prev.y = -p_prev.y;

        for (int i = 1; i < polygon.Count - 1; i++)
        {
            p = polygon[i].ToVector2(); p.y = -p.y;
            edge.Add(p);

            len = len_prev + (p - p_prev).magnitude;
            lengthBuffer.Add(len);

            if (i > 1)
            {
                bounds.Add(new BoundingBox(new Vector2(p_prev.x, -p_prev.y), new Vector2(p.x, -p.y)));
            }

            len_prev = len;
            p_prev = p;
        }
    }
}
