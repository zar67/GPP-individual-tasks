using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spline
{
    [SerializeField, HideInInspector]
    public List<Vector3> points;
    [SerializeField, HideInInspector]
    bool is_closed;
    [SerializeField, HideInInspector]
    bool auto_set_control_points;

    public Spline(Vector3 centre)
    {
        points = new List<Vector3>
        {
            centre + Vector3.left,
            centre + (Vector3.left + Vector3.up) * 0.5f,
            centre + (Vector3.right + Vector3.down) * 0.5f,
            centre + Vector3.right
        };
    }

    public Vector3 this[int i]
    {
        get
        {
            return points[i];
        }
    }

    public bool AutoSetControlPoints
    {
        get
        {
            return auto_set_control_points;
        }
        set
        {
            if (auto_set_control_points != value)
            {
                auto_set_control_points = value;
                if (auto_set_control_points)
                {
                    AutoSetAllControlPoints();
                }
            }
        }
    }

    public int NumPoints
    {
        get
        {
            return points.Count;
        }
    }

    public int NumSegments
    {
        get
        {
            return points.Count / 3;
        }
    }

    public void AddSegment(Vector3 anchorPos)
    {
        points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
        points.Add((points[points.Count - 1] + anchorPos) * 0.5f);
        points.Add(anchorPos);

        if (auto_set_control_points)
        {
            AutoSetAllAffectedControlPoints(points.Count - 1);
        }
    }

    public void AddSegmentAtEnd()
    {
        Vector3 direction = points[0] - points[points.Count - 1];
        direction.Normalize();
        direction *= 5;
        AddSegment(points[points.Count - 1] - direction);
    }

    public Vector3[] GetPointsInSegment(int i)
    {
        return new Vector3[] { points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[LoopIndex(i * 3 + 3)] };
    }

    public void MovePoint(int i, Vector3 position)
    {
        Vector3 delta_move = position - points[i];

        if (i % 3 == 0 || !auto_set_control_points)
        {
            points[i] = position;

            if (auto_set_control_points)
            {
                AutoSetAllAffectedControlPoints(i);
            }
            else
            {

                if (i % 3 == 0)
                {
                    if (i + 1 < points.Count || is_closed)
                    {
                        points[LoopIndex(i + 1)] += delta_move;
                    }
                    if (i - 1 >= 0 || is_closed)
                    {
                        points[LoopIndex(i - 1)] += delta_move;
                    }
                }
                else
                {
                    bool next_pos_anchor = (i + 1) % 3 == 0;
                    int corresponding_control_index = (next_pos_anchor) ? i + 2 : i - 2;
                    int anchor_index = (next_pos_anchor) ? i + 1 : i - 1;

                    if (corresponding_control_index >= 0 && corresponding_control_index < points.Count || is_closed)
                    {
                        float distance = (points[LoopIndex(anchor_index)] - points[LoopIndex(corresponding_control_index)]).magnitude;
                        Vector3 direction = (points[LoopIndex(anchor_index)] - position).normalized;
                        points[LoopIndex(corresponding_control_index)] = points[LoopIndex(anchor_index)] + direction * distance;
                    }
                }
            }
        }
    }

    public void ToggleClosed()
    {
        is_closed = !is_closed;

        if (is_closed)
        {
            points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
            points.Add(points[0] * 2 - points[1]);
            if (auto_set_control_points)
            {
                AutoSetAnchorControlPoints(0);
                AutoSetAnchorControlPoints(points.Count - 3);
            }
        }
        else
        {
            points.RemoveRange(points.Count - 2, 2);
            if (auto_set_control_points)
            {
                AutoSetStartAndEndControls();
            }
        }
    }

    void AutoSetAllAffectedControlPoints(int updated_anchor_index)
    {
        for (int i = updated_anchor_index - 3; i <= updated_anchor_index + 3; i += 3)
        {
            if (i >= 0 && i < points.Count || is_closed)
            {
                AutoSetAnchorControlPoints(LoopIndex(i));
            }
        }

        AutoSetStartAndEndControls();
    }

    void AutoSetAllControlPoints()
    {
        for (int i = 0; i < points.Count; i += 3)
        {
            AutoSetAnchorControlPoints(i);
        }

        AutoSetStartAndEndControls();
    }

    void AutoSetAnchorControlPoints(int anchor_index)
    {
        Vector3 anchor_position = points[anchor_index];
        Vector3 direction = Vector3.zero;
        float[] neighbour_distances = new float[2];

        if (anchor_index - 3 >= 0 || is_closed)
        {
            Vector3 offset = points[LoopIndex(anchor_index - 3)] - anchor_position;
            direction += offset.normalized;
            neighbour_distances[0] = offset.magnitude;
        }
        if (anchor_index + 3 >= 0 || is_closed)
        {
            Vector3 offset = points[LoopIndex(anchor_index + 3)] - anchor_position;
            direction -= offset.normalized;
            neighbour_distances[1] = -offset.magnitude;
        }

        direction.Normalize();

        for (int i = 0; i < 2; i++)
        {
            int control_index = anchor_index + i * 2 - 1;
            if (control_index >= 0 && control_index < points.Count || is_closed)
            {
                points[LoopIndex(control_index)] = anchor_position + direction * neighbour_distances[i] * .5f;
            }
        }
    }

    void AutoSetStartAndEndControls()
    {
        if (!is_closed)
        {
            points[1] = (points[0] + points[2]) * .5f;
            points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * .5f;
        }
    }

    int LoopIndex(int i)
    {
        return (i + points.Count) % points.Count;
    }

}