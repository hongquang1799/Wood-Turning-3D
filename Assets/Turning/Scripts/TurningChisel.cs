using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vector2i = ClipperLib.IntPoint;
using Polygon = System.Collections.Generic.List<ClipperLib.IntPoint>;

public class TurningChisel : MonoBehaviour
{
    public ParticleSystem particleSystem;

    protected TurningController subject;

    protected TouchDragChisel chiselDrag;
    public TurningController Subject { set { subject = value; } get { return subject; } }

    protected Vector2[] basePolygon;

    protected List<Vector2i> clipPolygon = new List<Vector2i>();

    protected List<Vector2> clipPoints = new List<Vector2>();
    public List<Vector2i> Polygon => clipPolygon;

    private Vector2 currentPosition;

    private Vector2 previousPosition;

    private BoundingBox bound;

    protected bool contacting = false;

    void Start()
    {
        particleSystem.Stop();

        previousPosition = transform.position;

        Vector2 offset = Vector2.zero;
        Vector2[] points = null;
        if (GetComponent<ChiselShape>())
        {
            var chiselShape = GetComponent<ChiselShape>();
            points = chiselShape.points;
            offset = chiselShape.offset;
        }

        float clampOffset = float.MinValue;
        basePolygon = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            basePolygon[i] = points[i] + offset;
            if (clampOffset < basePolygon[i].y) clampOffset = basePolygon[i].y;
        }

        chiselDrag = GetComponent<TouchDragChisel>();
        chiselDrag.clampPositionY = subject.transform.position.y - clampOffset + 0.001f;

        ContactEnd();

        StartCoroutine(PreventContactingWhenPositionNotChanged(0.5f));
    }

    private void Update()
    {
        if (subject)
        {
            if (!transform.hasChanged)
            {
                return;
            }               

            transform.hasChanged = false;

            currentPosition = transform.localPosition;
            BuildClipPolygon(previousPosition, currentPosition);

            if (subject.OverlapChisel(clipPoints, bound))
            {
                if (!contacting)
                {
                    contacting = true;
                    ContactBegin();
                }

                subject.Clip(this);          
            }
            else
            {
                if (contacting)
                {
                    contacting = false;
                    ContactEnd();
                }
            }
          
            previousPosition = currentPosition;
        }
    }

    public void ChangePosition(Vector3 position)
    {
        transform.position = position;

        if (previousPosition != currentPosition)
        {
            transform.hasChanged = true;
        }

        previousPosition = currentPosition = position;     
    }

    void ContactBegin()
    {
        chiselDrag.OnChiselContactBegin();
        particleSystem.Play();
    }

    void ContactEnd()
    {
        chiselDrag.OnChiselContactEnd();
        particleSystem.Stop();
    }

    void BuildClipPolygon(Vector2 begin, Vector2 end)
    {
        Vector2 delta = end - begin;
        int a = 0, b = 0;
        float max_eval = float.MinValue;
        float min_eval = float.MaxValue;
        Vector2 p; 
        for (int i = 0; i < basePolygon.Length; i++)
        {
            p = basePolygon[i];
            float eval = -delta.y * p.x + delta.x * p.y;

            if (eval < min_eval)
            {
                min_eval = eval;
                a = i;
            }

            if (eval > max_eval)
            {
                max_eval = eval;
                b = i;
            }
        }

        clipPolygon.Clear();
        clipPoints.Clear();

        bound.pointA.x = float.MaxValue;
        bound.pointA.y = float.MaxValue;
        bound.pointB.x = float.MinValue;
        bound.pointB.y = float.MinValue;

        int k = a;
        clipPolygon.Add((basePolygon[k] + begin).ToVector2i());
        while (k != b)
        {
            p = basePolygon[k] + end;
            clipPoints.Add(p);
            clipPolygon.Add(p.ToVector2i());

            k = (k + 1) % basePolygon.Length;

            bound.pointA.x = Mathf.Min(bound.pointA.x, p.x);
            bound.pointA.y = Mathf.Min(bound.pointA.y, p.y);
            bound.pointB.x = Mathf.Max(bound.pointB.x, p.x);
            bound.pointB.y = Mathf.Max(bound.pointB.y, p.y);
        }

        k = b;
        clipPolygon.Add((basePolygon[b] + end).ToVector2i());        
        while (k != a)
        {
            p = basePolygon[k] + begin;
            clipPoints.Add(p);
            clipPolygon.Add(p.ToVector2i());

            k = (k + 1) % basePolygon.Length;

            bound.pointA.x = Mathf.Min(bound.pointA.x, p.x);
            bound.pointA.y = Mathf.Min(bound.pointA.y, p.y);
            bound.pointB.x = Mathf.Max(bound.pointB.x, p.x);
            bound.pointB.y = Mathf.Max(bound.pointB.y, p.y);
        }
    }

    IEnumerator PreventContactingWhenPositionNotChanged(float maxDuration)
    {
        float time = maxDuration;

        while (true)
        {
            if (transform.hasChanged)
            {
                time = maxDuration;
            }

            time -= Time.deltaTime;

            if (time < 0f)
            {
                if (contacting)
                {
                    contacting = false;
                    ContactEnd();
                }           
            }

            yield return null;
        }
    }
}
