using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    MovingPlatformsController controller;
    public int current_index = 0;

    bool move = false;

    private void Update()
    {
        if (move)
        {
            if (controller.mechanical_movement)
            {
                Vector3 new_pos = Vector3.Lerp(transform.position, controller.spline_positions[current_index + 1], controller.platform_move_speed * Time.deltaTime);
                transform.position = new_pos;//new Vector3(new_pos.x, transform.position.y, new_pos.z);

                if (controller.rotate_to_spline)
                {
                    transform.rotation = Quaternion.LookRotation((controller.spline_positions[current_index + 1] - controller.spline_positions[current_index]).normalized);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }

                if (Vector3.Distance(transform.position, controller.spline_positions[current_index + 1]) < 0.2f)
                {
                    current_index += 1;

                    if (current_index == controller.spline_positions.Count - 1)
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else
            {
                Vector3 direction = (controller.spline_positions[current_index + 1] - controller.spline_positions[current_index]).normalized;
                transform.position += direction * Time.deltaTime * controller.platform_move_speed * 0.5f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * controller.platform_move_speed);

                if (Vector3.Distance(transform.position, controller.spline_positions[current_index + 1]) < 0.5f)
                {
                    current_index += 1;

                    if (current_index == controller.spline_positions.Count - 1)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    public void StartPlatform(MovingPlatformsController platform_controller)
    {
        controller = platform_controller;
        transform.rotation = Quaternion.LookRotation((controller.spline_positions[current_index + 1] - controller.spline_positions[current_index]).normalized);
        move = true;
    }
}
