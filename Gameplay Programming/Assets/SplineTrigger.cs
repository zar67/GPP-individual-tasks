using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineTrigger : MonoBehaviour
{
    public SplineGameplayController target;
    public bool position_start;

    private void OnTriggerExit(Collider other)
    {
        if (!target.triggered)
        {
            TriggerSpline();
        }
    }

    void TriggerSpline()
    {
        target.camera_controller.DisableCamera();

        target.player_controller.accept_input = false;
        target.player_controller.jump_force /= 2;
        target.player_controller.ResetAnimator();

        target.left_dir_positive = position_start;

        if (position_start)
        {
            target.current_player_index = 0 + target.bezier_position_divisions;
        }
        else
        {
            target.current_player_index = target.player_spline_positions.Count - target.bezier_position_divisions;
        }
        target.player_controller.transform.position = target.player_spline_positions[target.current_player_index];

        if (position_start)
        {
            target.camera_controller.transform.position = target.camera_controller.target.position + (-target.camera_controller.target.right * target.camera_controller.base_offset.x) + (target.camera_controller.target.up * target.camera_controller.base_offset.y);
        }
        else
        {
            target.camera_controller.transform.position = target.camera_controller.target.position - (-target.camera_controller.target.right * target.camera_controller.base_offset.x) + (target.camera_controller.target.up * target.camera_controller.base_offset.y);
        }

        target.camera_controller.transform.LookAt(target.camera_controller.target.position);
        target.StartSplineGameplay();
    }
}
