using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(ChiselShape))]
public class ChiselShapeEditor : Editor
{
    private ChiselShape shape;

    private void OnEnable()
    {
        shape = target as ChiselShape;
    }

    private void OnSceneGUI()
    {
        if (shape.points.Length < 2) return;

        Vector3 basePos = shape.transform.position;
        var ps = shape.points;
        for (int i = 0; i < ps.Length; i++)
        {
            Vector3 p = ps[i] - shape.offset;
            p = Handles.FreeMoveHandle(basePos + p, Quaternion.identity, 0.01f, Vector3.zero, Handles.CylinderHandleCap);
            ps[i] = p - basePos + shape.offset.ToVector3();
        }
        
        Handles.color = Color.green;

        Vector3 p1 = ps[ps.Length - 1] - shape.offset;
        Vector3 p2 = ps[0] - shape.offset;
        Handles.DrawLine(p1 + basePos, p2 + basePos);
        for (int i = 1; i < ps.Length; i++)
        {
            p1 = ps[i - 1] - shape.offset;
            p2 = ps[i] - shape.offset;
            Handles.DrawLine(p1 + basePos, p2 + basePos);
        }        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();               
    }
}
#endif