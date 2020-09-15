using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PointNode 
{
    public PointNode(Vector3 p, float d1, float d2, float a)
    {
        position = p;

        float c = Mathf.Cos(Mathf.Deg2Rad * a);
        float s = Mathf.Sin(Mathf.Deg2Rad * a);

        controlA = new Vector3(d1 * c, d1 * s);
        controlB = new Vector3(-d2 * c, -d2 * s);
    }

    public Vector3 position;

    public Vector3 controlA;
    public Vector3 controlB;

    public bool controlConstraintBreak;
    public bool fixedControlA;
    public bool fixedControlB;
}

[Serializable]
public class Path
{
    [SerializeField] private List<PointNode> controlPoints;

    [SerializeField] private bool closed;

    public bool Closed
    {
        set 
        {
            if (value == true)
            {
                if (Count > 2)
                    closed = value;
            }
            else
            {
                closed = value;
            }               
        }
        get 
        {
            return closed;
        }
    }

    public Path(float width, float height)
    {
        PointNode p1 = new PointNode(new Vector3(-width / 2f, 0f, 0f), 1f, 1f, 90f);
        PointNode p2 = new PointNode(new Vector3(+width / 2f, 0f, 0f), 1f, 1f, 90f);

        controlPoints = new List<PointNode>();
        controlPoints.Add(p1);
        controlPoints.Add(p2); 
    }

    public PointNode this[int idx]
    {
        get
        {
            return controlPoints[idx];
        }

        set
        {
            controlPoints[idx] = value;
        }
    }

    public List<Vector3> GetPoints(int segment)
    {
        List<Vector3> ps = new List<Vector3>();

        PointNode cp_prev, cp = controlPoints[0];
        Vector3 pa, pb, pc, pd = Vector3.zero;
        int count = controlPoints.Count;

        if (closed)
        {           
            for (int i = 1; i <= count; i++)
            {
                cp_prev = cp;
                cp = i != count ? controlPoints[i] : controlPoints[0];
                pa = cp_prev.position;
                pb = (cp_prev.fixedControlA) ? cp_prev.position : cp_prev.controlA + cp_prev.position;
                pc = (cp.fixedControlB) ? cp.position : cp.controlB + cp.position;
                pd = cp.position;

                ps.Add(pa);

                for (int j = 1; j < segment; j++)
                {
                    float t = (float)(j) / segment;
                    float o_t = 1 - t;

                    ps.Add(o_t * o_t * o_t * pa + 3 * o_t * o_t * t * pb + 3 * o_t * t * t * pc + t * t * t * pd);
                }
            }
        }
        else
        {
            for (int i = 1; i < controlPoints.Count; i++)
            {
                cp_prev = cp;
                cp = controlPoints[i];
                pa = cp_prev.position;
                pb = (cp_prev.fixedControlA) ? cp_prev.position : cp_prev.controlA + cp_prev.position;
                pc = (cp.fixedControlB) ? cp.position : cp.controlB + cp.position;
                pd = cp.position;

                ps.Add(pa);

                for (int j = 1; j < segment; j++)
                {
                    float t = (float)(j) / segment;
                    float o_t = 1 - t;

                    ps.Add(o_t * o_t * o_t * pa + 3 * o_t * o_t * t * pb + 3 * o_t * t * t * pc + t * t * t * pd);
                }          
            }

            ps.Add(pd);
        }
        

        return ps;
    }

    public int Count { get { return controlPoints.Count; } }

    public void Add(PointNode p)
    {
        controlPoints.Add(p);
    }

    public void Insert(PointNode p, int idx)
    {
        controlPoints.Insert(idx + 1, p);
    }

    public void Remove(int idx)
    {
        controlPoints.RemoveAt(idx);

        if (Count <= 2) 
            Closed = false;
    }
}
