using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    MovingPlatformsController controller;
    GameObject player = null;

    public int current_index = 0;
    bool move = false;

    private void Update()
    {
        // TODO: Rotate Under Weight of Player
        if (move)
        {
            Vector3 direction = (controller.spline_positions[current_index + 1] - transform.position).normalized;

            if (controller.rotate_to_spline && direction != Vector3.zero)
            {
                if (controller.floating)
                {
                    Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * controller.platform_move_speed);
                }
                else
                {
                    Vector3 target = direction;
                    target.y = controller.start_y;
                    Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target), Time.deltaTime * controller.platform_move_speed);
                }
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            if (controller.mechanical_movement)
            {
                if (!controller.floating)
                {
                    Vector3 new_pos = Vector3.Lerp(transform.position, controller.spline_positions[current_index + 1], controller.platform_move_speed * Time.deltaTime);
                    transform.position = new Vector3(new_pos.x, controller.start_y, new_pos.z);
                }
            }
            else
            {
                transform.position += direction * Time.deltaTime * controller.platform_move_speed * 0.5f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * controller.platform_move_speed);
            }

            if (!controller.floating)
            {
                Vector3 target = controller.spline_positions[current_index + 1];
                target.y = controller.start_y;

                if (Vector3.Distance(transform.position, target) < 0.5f)
                {
                    NextPosition();
                }
            }

            if (Vector3.Distance(transform.position, controller.spline_positions[current_index + 1]) < 0.5f)
            {
                NextPosition();
            }
        }
    }

    public bool MechanicalMove()
    {
        Vector3 new_pos = Vector3.Lerp(transform.position, controller.spline_positions[current_index + 1], controller.platform_move_speed * 5 * Time.deltaTime);
        transform.position = new_pos;

        return Vector3.Distance(transform.position, controller.spline_positions[current_index + 1]) < 0.5f;
    }

    public void StartPlatform(MovingPlatformsController platform_controller)
    {
        controller = platform_controller;
        transform.rotation = Quaternion.LookRotation((controller.spline_positions[current_index + 1] - transform.position).normalized);
        move = true;
    }

    void NextPosition()
    {
        current_index += 1;

        if (current_index == controller.spline_positions.Count - 1)
        {
            if (player != null)
            {
                player.transform.parent = null;
                player = null;
            }

            controller.RemovePlatform(this);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            player = other.gameObject;
            other.gameObject.transform.parent = this.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            player = null;
            other.gameObject.transform.parent = null;
        }
    }
}
