using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SplineGameplayController : MonoBehaviour
{
    public List<Vector3> player_spline_positions;
    public List<Vector3> camera_spline_positions;
    int current_player_index = 0;
    int current_camera_index = 0;
    bool triggered = false;

    // References
    public PathCreator player_spline;
    public PathCreator camera_spline;

    RPGCharacterController player_controller;
    RPGCameraController camera_controller;

    private void Awake()
    {
        player_controller = GameObject.FindObjectOfType<RPGCharacterController>();
        camera_controller = GameObject.FindObjectOfType<RPGCameraController>();

        // Get Player Spline Points
        for (int i = 0; i < player_spline.path.NumSegments; i++)
        {
            Vector3[] points = player_spline.path.GetPointsInSegment(i);
            Vector3[] new_positions = Handles.MakeBezierPoints(points[0], points[3], points[1], points[2], (int)(points[0] - points[3]).magnitude / 2);

            for (int pos = 0; pos < new_positions.Length; i++)
            {
                player_spline_positions.Add(new_positions[pos]);
            }
        }

        // Get Camera Spline Points
        for (int i = 0; i < camera_spline.path.NumSegments; i++)
        {
            Vector3[] points = camera_spline.path.GetPointsInSegment(i);
            Vector3[] new_positions = Handles.MakeBezierPoints(points[0], points[3], points[1], points[2], (int)(points[0] - points[3]).magnitude / 2);

            for (int pos = 0; pos < new_positions.Length; i++)
            {
                camera_spline_positions.Add(new_positions[pos]);
            }
        }
    }

    private void Update()
    {
        if (triggered)
        {

        }
    }

    public void StartSplineGameplay()
    {
        triggered = true;
        player_controller.accept_input = false;
        player_controller.ResetAnimator();
        camera_controller.DisableCamera();
        
        // Set Player and Camera to Start Positions
    }

    public void EndSplineGameplay()
    {
        triggered = false;
        player_controller.accept_input = true;
        camera_controller.EnableCamera();
    }
}
