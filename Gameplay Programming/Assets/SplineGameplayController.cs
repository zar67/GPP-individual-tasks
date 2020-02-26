using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SplineGameplayController : MonoBehaviour
{
   // References
    public PathCreator player_spline;
    public PathCreator camera_spline;

    RPGCharacterController player_controller;
    RPGCameraController camera_controller;

    public int bezier_position_divisions = 10;
    public bool left_dir_positive = true;

    [HideInInspector]
    public List<Vector3> player_spline_positions;
    [HideInInspector]
    public List<Vector3> camera_spline_positions;

    int current_player_index = 0;
    int current_camera_index = 0;
    [HideInInspector]
    public bool triggered = false;

    private void Awake()
    {
        player_controller = GameObject.FindObjectOfType<RPGCharacterController>();
        camera_controller = GameObject.FindObjectOfType<RPGCameraController>();

        // Get Player Spline Points
        for (int i = 0; i < player_spline.path.NumSegments; i++)
        {
            Vector3[] points = player_spline.path.GetPointsInSegment(i);
            Vector3[] new_positions = Handles.MakeBezierPoints(points[0], points[3], points[1], points[2], bezier_position_divisions);
            player_spline_positions.AddRange(new_positions);
        }

        // Get Camera Spline Points
        for (int i = 0; i < camera_spline.path.NumSegments; i++)
        {
            Vector3[] points = camera_spline.path.GetPointsInSegment(i);
            Vector3[] new_positions = Handles.MakeBezierPoints(points[0], points[3], points[1], points[2], bezier_position_divisions);
            camera_spline_positions.AddRange(new_positions);
        }
    }

    private void Update()
    {
        if (triggered)
        {
            player_controller.UpdateAnimator();

            if (Input.GetAxis("Horizontal") != 0)
            {
                bool dir_positive = (Input.GetAxis("Horizontal") < 0 && left_dir_positive) || (Input.GetAxis("Horizontal") > 0 && !left_dir_positive);

                if (dir_positive)
                {
                    // Lerp towards current + 1
                }
                else
                {
                    //Lerp towards current - 1
                }
            }

            if (Input.GetButtonDown("Jump"))
            {
                player_controller.set_jump = true;
            }
        }
    }

    public void StartSplineGameplay()
    {
        triggered = true;
        player_controller.accept_input = false;
        player_controller.jump_force /= 2;
        player_controller.ResetAnimator();
        camera_controller.DisableCamera();

        player_controller.transform.position = player_spline_positions[0];
        camera_controller.transform.position = camera_spline_positions[0];
        camera_controller.transform.LookAt(camera_controller.target);
    }

    public void EndSplineGameplay()
    {
        triggered = false;
        player_controller.accept_input = true;
        player_controller.jump_force *= 2;
        camera_controller.EnableCamera();
    }
}
