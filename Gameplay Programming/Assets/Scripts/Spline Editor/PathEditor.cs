using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    PathCreator creator;
    Path path;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        if (GUILayout.Button("Create new"))
        {
            Undo.RecordObject(creator, "Create new");
            creator.CreatePath();
            path = creator.path;
        }

        if (GUILayout.Button("Add Anchor"))
        {
            Undo.RecordObject(creator, "Add Anchor");
            path.AddSegmentAtEnd();
        }

        if (GUILayout.Button("Toggle closed"))
        {
            Undo.RecordObject(creator, "Toggle closed");
            path.ToggleClosed();
        }

        bool autoSetControlPoints = GUILayout.Toggle(path.AutoSetControlPoints, "Auto Set Control Points");
        if (autoSetControlPoints != path.AutoSetControlPoints)
        {
            Undo.RecordObject(creator, "Toggle auto set controls");
            path.AutoSetControlPoints = autoSetControlPoints;
        }

        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
        }
    }

    void OnSceneGUI()
    {
        Draw();
    }

    void Draw()
    {
        for (int i = 0; i < path.NumSegments; i++)
        {
            Vector3[] points = path.GetPointsInSegment(i);
            Handles.color = Color.black;
            Handles.DrawLine(points[1], points[0]);
            Handles.DrawLine(points[2], points[3]);
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);
        }

        for (int i = 0; i < path.NumPoints; i++)
        {
            if (i % 3 == 0)
            {
                Handles.color = Color.red;
            }
            else
            {
                Handles.color = Color.white;
            }

            Vector3 new_position = Handles.FreeMoveHandle(path[i], Quaternion.identity, .1f, Vector3.zero, Handles.SphereHandleCap);
            if (path[i] != new_position)
            {
                Undo.RecordObject(creator, "Move point");
                path.MovePoint(i, new_position);
            }
        }
    }

    void OnEnable()
    {
        creator = (PathCreator)target;
        if (creator.path == null)
        {
            creator.CreatePath();
        }
        path = creator.path;
    }
}