using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SplineGameplayController : MonoBehaviour
{
   // References
    public SplineCreator player_spline;

    [HideInInspector]
    public RPGCharacterController player_controller;
    [HideInInspector]
    public RPGCameraController camera_controller;

    public int bezier_position_divisions = 10;
    public bool left_dir_positive = true;

    [HideInInspector]
    public List<Vector3> player_spline_positions;

    //[HideInInspector]
    public int current_player_index = 0;
    //[HideInInspector]
    public bool triggered = false;
    bool end_lerp = false;
    Vector3 camera_offset;

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
                if (Input.GetAxis("Horizontal") < 0 && current_player_index < player_spline_positions.Count)
                {
                    if (current_player_index == player_spline_positions.Count - 1)
                    {
                        triggered = false;
                        player_controller.ResetAnimator();
                        end_lerp = true;
                    }
                    else
                    {
                        Vector3 new_pos = Vector3.Lerp(player_controller.transform.position, player_spline_positions[current_player_index + 1], player_controller.move_speed * Time.deltaTime);
                        player_controller.transform.position = new Vector3(new_pos.x, player_controller.transform.position.y, new_pos.z);

                        Vector3 target_dir = player_spline_positions[current_player_index + 1];
                        target_dir.y = player_controller.transform.position.y;
                        player_controller.transform.LookAt(target_dir);

                        if (Vector3.Distance(player_controller.transform.position, player_spline_positions[current_player_index + 1]) < 0.2f)
                        {
                            current_player_index += 1;

                            if (current_player_index == player_spline_positions.Count)
                            {
                                triggered = false;
                                player_controller.ResetAnimator();
                                end_lerp = true;
                            }
                        }

                        camera_offset = camera_controller.target.position + (-camera_controller.target.right * camera_controller.base_offset.x) + (camera_controller.target.up * camera_controller.base_offset.y);
                    }
                }
                else if (Input.GetAxis("Horizontal") > 0)
                {
                    if (current_player_index == 0)
                    {
                        triggered = false;
                        player_controller.ResetAnimator();
                        end_lerp = true;
                    }
                    else
                    {
                        Vector3 new_pos = Vector3.Lerp(player_controller.transform.position, player_spline_positions[current_player_index - 1], player_controller.move_speed * Time.deltaTime);
                        player_controller.transform.position = new Vector3(new_pos.x, player_controller.transform.position.y, new_pos.z);

                        Vector3 target_dir = player_spline_positions[current_player_index - 1];
                        target_dir.y = player_controller.transform.position.y;
                        player_controller.transform.LookAt(target_dir);

                        if (Vector3.Distance(player_controller.transform.position, player_spline_positions[current_player_index - 1]) < 0.2f)
                        {
                            current_player_index -= 1;

                            if (current_player_index == 0)
                            {
                                triggered = false;
                                player_controller.ResetAnimator();
                                end_lerp = true;
                            }
                        }

                        camera_offset = camera_controller.target.position - (-camera_controller.target.right * camera_controller.base_offset.x) + (camera_controller.target.up * camera_controller.base_offset.y);
                    }
                }

                camera_controller.transform.position = Vector3.Lerp(camera_controller.transform.position, camera_offset, camera_controller.rotation_speed * Time.deltaTime);
                camera_controller.transform.LookAt(camera_controller.target);
            }

            if (Input.GetButtonDown("Jump") && player_controller.GetComponent<Animator>().GetInteger("jumping") == 0)
            {
                player_controller.set_jump = true;
            }
        }

        if (end_lerp)
        {
            Vector3 offset = camera_controller.target.position + (-camera_controller.target.forward * camera_controller.base_offset.x) + (camera_controller.target.up * camera_controller.base_offset.y);
            camera_controller.transform.position = Vector3.Lerp(camera_controller.transform.position, offset, camera_controller.rotation_speed * Time.deltaTime);
            camera_controller.transform.LookAt(camera_controller.target.position);

            if (Vector3.Distance(camera_controller.transform.position, offset) < 0.25f)
            {
                end_lerp = false;
                EndSplineGameplay();
            }
        }
    }

    public void StartSplineGameplay()
    {
        triggered = true;
        player_controller.move_speed = RPGCharacterController.base_move_speed * 2;
    }

    public void EndSplineGameplay()
    {
        player_controller.move_speed = RPGCharacterController.base_move_speed;
        player_controller.jump_force *= 2;

        player_controller.accept_input = true;
        camera_controller.transform.position = camera_controller.target.position + (-camera_controller.target.forward * camera_controller.base_offset.x) + (camera_controller.target.up * camera_controller.base_offset.y);
        camera_controller.transform.LookAt(camera_controller.target.position);
        camera_controller.EnableCamera();
    }
}
