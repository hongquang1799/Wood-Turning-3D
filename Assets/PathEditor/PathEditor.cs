using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(PathProvider))]
public class PathEditor : Editor
{
    private PathProvider pathProvider;

    private Path path;

    private bool showControlPoints = true;

    private string fileDirectory = "Assets/Resources/Patterns/";

    private string fileName = "Pattern_1";

    private void OnEnable()
    {
        pathProvider = target as PathProvider;
        if (pathProvider.path == null) 
            pathProvider.path = new Path(2f, 1f);

        path = pathProvider.path;
    }

    private void OnSceneGUI()
    {
        Vector3 basePos = pathProvider.transform.position;

        Event guiEvent = Event.current;

        if (guiEvent.shift)
        {
            Vector3 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin - basePos;
            mousePos.z = 0f; 

            float closestDistance = float.MaxValue;
            int closestIndex = -1;

            for (int i = 0; i < path.Count; i++)
            {
                float distance = (mousePos - path[i].position).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            if (closestIndex != -1)
            {
                PointNode p = new PointNode(mousePos, 1f, 1f, 90f);
                if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
                {
                    path.Insert(p, closestIndex);
                    //Debug.LogFormat("insert point {0}", closestIndex);
                }
                else if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1)
                {
                    path.Remove(closestIndex);
                    //Debug.LogFormat("remove point {0}", closestIndex);
                }
                else if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.B)
                {
                    path[closestIndex].controlConstraintBreak = !path[closestIndex].controlConstraintBreak;
                }
                else if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.A)
                {
                    path[closestIndex].fixedControlA = !path[closestIndex].fixedControlA;
                }
                else if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.S)
                {
                    path[closestIndex].fixedControlB = !path[closestIndex].fixedControlB;
                }
            }    
        }

        if (showControlPoints)
        {
            Vector3 pos, controlA_world, controlA_local, controlB_world, controlB_local;
            for (int i = 0; i < path.Count; i++)
            {
                PointNode p = path[i];
                pos = p.position + basePos;
                Handles.color = Color.red;

                pos = Handles.FreeMoveHandle(pos, Quaternion.identity, 0.2f, Vector3.zero, Handles.CylinderHandleCap);
                p.position = pos - basePos;

                if (p.controlConstraintBreak)
                {
                    Handles.color = Color.blue;
                    controlA_world = Handles.FreeMoveHandle(pos + p.controlA, Quaternion.identity, 0.1f, Vector3.zero, Handles.CylinderHandleCap); ;
                    controlA_local = controlA_world - pos;

                    controlB_world = Handles.FreeMoveHandle(pos + p.controlB, Quaternion.identity, 0.1f, Vector3.zero, Handles.CylinderHandleCap); ;
                    controlB_local = controlB_world - pos;

                    p.controlB = controlB_local;
                    p.controlA = controlA_local;

                    if (p.fixedControlA)
                        Handles.color = Color.black;
                    else
                        Handles.color = Color.blue;
                    Handles.DrawLine(p.controlA + pos, pos);

                    if (p.fixedControlB)
                        Handles.color = Color.black;
                    else
                        Handles.color = Color.blue;
                    Handles.DrawLine(p.controlB + pos, pos);
                }
                else
                {
                    float d2 = p.controlB.magnitude;

                    Handles.color = Color.blue;
                    controlA_world = Handles.FreeMoveHandle(pos + p.controlA, Quaternion.identity, 0.1f, Vector3.zero, Handles.CylinderHandleCap); ;
                    controlA_local = controlA_world - pos;

                    controlB_local = -controlA_local.normalized * d2;

                    float d1 = controlA_local.magnitude;

                    controlB_world = Handles.FreeMoveHandle(pos + controlB_local, Quaternion.identity, 0.1f, Vector3.zero, Handles.CylinderHandleCap); ;
                    controlB_local = controlB_world - pos;
                    controlA_local = -controlB_local.normalized * d1;
                    p.controlB = controlB_local;
                    p.controlA = controlA_local;

                    if (p.fixedControlA) 
                        Handles.color = Color.black;
                    else
                        Handles.color = Color.blue;
                    Handles.DrawLine(p.controlA + pos, pos);

                    if (p.fixedControlB)
                        Handles.color = Color.black;
                    else
                        Handles.color = Color.blue;
                    Handles.DrawLine(p.controlB + pos, pos);
                }
            }
        }
        
        Handles.color = Color.green;
        var ps = path.GetPoints(pathProvider.segment);

        if (path.Closed)
        {
            for (int i = 1; i < ps.Count; i++)
            {
                Handles.DrawLine(ps[i - 1] + basePos, ps[i] + basePos);
            }
            Handles.DrawLine(ps[ps.Count - 1] + basePos, ps[0] + basePos);
        }
        else
        {
            for (int i = 1; i < ps.Count; i++)
            {
                Handles.DrawLine(ps[i - 1] + basePos, ps[i] + basePos);
            }
        }
        
        ps.Clear();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        path.Closed = GUILayout.Toggle(path.Closed, "Closed");

        GUILayout.Space(100);

        showControlPoints = GUILayout.Toggle(showControlPoints, "Show Control Points");      

        if (GUILayout.Button("Reset"))
        {
            pathProvider.path = new Path(2f, 1f);
            path = pathProvider.path;
        }

        GUILayout.Space(100);

        fileDirectory = GUILayout.TextField(fileDirectory);
        fileName = GUILayout.TextField(fileName);

        if (GUILayout.Button("Save"))
        {
            var ps = path.GetPoints(pathProvider.segment);
            var points = new Vector2[ps.Count];

            for(int i = 0; i < ps.Count; i++)
            {
                points[i] = ps[i];
            }

            PatternShape shapeData = CreateInstance<PatternShape>();
            shapeData.points = points;
            AssetDatabase.CreateAsset(shapeData, fileDirectory + fileName + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
}
#endif