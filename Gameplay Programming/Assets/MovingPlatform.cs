using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    MovingPlatformsController controller;
    int current_index = 0;

    bool move = false;

    private void Update()
    {
        if (move)
        {
            Vector3 new_pos = Vector3.Lerp(transform.position, controller.spline_positions[current_index + 1], controller.platform_move_speed * Time.deltaTime);
            transform.position = new Vector3(new_pos.x, transform.position.y, new_pos.z);

            if (Vector3.Distance(transform.position, controller.spline_positions[current_index + 1]) < 0.2f)
            {
                current_index += 1;

                if (current_index == controller.spline_positions.Count - 1)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void StartPlatform(MovingPlatformsController platform_controller)
    {
        controller = platform_controller;
        move = true;
    }
}
