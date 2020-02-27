using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SplineGameplayController : MonoBehaviour
{
   // References
    public PathCreator player_spline;

    RPGCharacterController player_controller;
    RPGCameraController camera_controller;

    public int bezier_position_divisions = 10;
    public bool left_dir_positive = true;

    [HideInInspector]
    public List<Vector3> player_spline_positions;

    int current_player_index = 0;
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
                    player_controller.transform.position = Vector3.Lerp(player_controller.transform.position, player_spline_positions[current_player_index + 1], player_controller.move_speed * Time.deltaTime);
                    Vector3 target_dir = player_spline_positions[current_player_index + 1];
                    target_dir.y = player_controller.transform.position.y;
                    player_controller.transform.LookAt(target_dir);

                    if (Vector3.Distance(player_controller.transform.position, player_spline_positions[current_player_index + 1]) < 0.2f)
                    {
                        current_player_index += 1;
                    }
                }
                else
                {
                    player_controller.transform.position = Vector3.Lerp(player_controller.transform.position, player_spline_positions[current_player_index - 1], player_controller.move_speed * Time.deltaTime);
                    Vector3 target_dir = player_spline_positions[current_player_index - 1];
                    target_dir.y = player_controller.transform.position.y;
                    player_controller.transform.LookAt(target_dir);

                    if (Vector3.Distance(player_controller.transform.position, player_spline_positions[current_player_index - 1]) < 0.2f)
                    {
                        current_player_index -= 1;
                    }
                }

                camera_controller.transform.position = camera_controller.target.position + (-camera_controller.target.right * camera_controller.base_offset.x) + (camera_controller.target.up * camera_controller.base_offset.y);
                camera_controller.transform.LookAt(camera_controller.target);
            }

            if (Input.GetButtonDown("Jump"))
            {
                player_controller.set_jump = true;
            }
        }
    }

    public void StartSplineGameplay(bool start_at_start)
    {
        camera_controller.DisableCamera();
        
        player_controller.accept_input = false;
        player_controller.jump_force /= 2;
        player_controller.ResetAnimator();

        if (start_at_start)
        {
            current_player_index = 0;
            player_controller.transform.position = player_spline_positions[0];
        }
        else
        {
            current_player_index = player_spline_positions.Count - 1;
            player_controller.transform.position = player_spline_positions[player_spline_positions.Count - 1];
        }

        triggered = true;
    }

    public void EndSplineGameplay()
    {
        triggered = false;
        player_controller.accept_input = true;
        player_controller.jump_force *= 2;
        camera_controller.EnableCamera();
    }
}
