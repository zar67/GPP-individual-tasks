using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovingPlatformsController : MonoBehaviour
{
    public PathCreator platform_spline;

    public Transform platforms_parent;
    public GameObject platform_prefab;

    public bool mechanical_movement = true;
    public bool floating = false;
    public float start_y = 0;
    public bool rotate_to_spline = false;
    public float distance_between_platforms = 10;
    public float platform_move_speed = 5;
    public int bezier_position_divisions = 10;

    [HideInInspector]
    public List<Vector3> spline_positions = new List<Vector3> { };
    List<MovingPlatform> platforms = new List<MovingPlatform> { };

    float timer = 0;
    float time_between_spawns = 10;
    bool triggered = true;

    // Start is called before the first frame update
    void Awake()
    {
        time_between_spawns = distance_between_platforms / platform_move_speed;

        // Get Spline Points
        for (int i = 0; i < platform_spline.path.NumSegments; i++)
        {
            Vector3[] points = platform_spline.path.GetPointsInSegment(i);
            Vector3[] new_positions = Handles.MakeBezierPoints(points[0], points[3], points[1], points[2], bezier_position_divisions);
            spline_positions.AddRange(new_positions);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered)
        {
            timer += Time.deltaTime;

            if (timer > time_between_spawns)
            {
                timer = 0;
                MovingPlatform new_platform = Instantiate(platform_prefab, spline_positions[0], Quaternion.identity, platforms_parent).GetComponent<MovingPlatform>();
                new_platform.StartPlatform(this);
                platforms.Add(new_platform);
            }
        }
    }

    public void StartPlatforms()
    {
        triggered = true;
    }

    public void StopPlatforms()
    {
        triggered = false;
    }
}
