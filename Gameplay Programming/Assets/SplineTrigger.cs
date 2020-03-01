using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineTrigger : MonoBehaviour
{
    public SplineGameplayController target;
    public bool position_start;

    bool begin_lerp = false;
    Vector3 offset;

    private void OnTriggerEnter(Collider other)
    {
        if (!target.triggered)
        {
            target.camera_controller.DisableCamera();

            target.player_controller.accept_input = false;
            target.player_controller.jump_force /= 2;
            target.player_controller.ResetAnimator();

            target.left_dir_positive = position_start;

            if (position_start)
            {
                target.current_player_index = 0;
                target.player_controller.transform.position = target.player_spline_positions[0];
            }
            else
            {
                target.current_player_index = target.player_spline_positions.Count - 1;
                target.player_controller.transform.position = target.player_spline_positions[target.player_spline_positions.Count - 1];
            }

            if (position_start)
            {
                offset = target.camera_controller.target.position + (-target.camera_controller.target.right * target.camera_controller.base_offset.x) + (target.camera_controller.target.up * target.camera_controller.base_offset.y);
            }
            else
            {
                offset = target.camera_controller.target.position - (-target.camera_controller.target.right * target.camera_controller.base_offset.x) + (target.camera_controller.target.up * target.camera_controller.base_offset.y);
            }
            begin_lerp = true;
        }
    }

    private void Update()
    {
        if (begin_lerp)
        {
            target.camera_controller.transform.position = Vector3.Lerp(target.camera_controller.transform.position, offset, target.camera_controller.rotation_speed * Time.deltaTime);
            target.camera_controller.transform.LookAt(target.camera_controller.target.position);

            if (Vector3.Distance(target.camera_controller.transform.position, offset) < 0.25f)
            {
                begin_lerp = false;
                target.StartSplineGameplay();
            }
        }
    }
}
