using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class MovingPlatform : MonoBehaviour
{
    MovingPlatformsController controller;
    GameObject player = null;

    public int current_index = 0;
    bool move = false;

    EndOfPathInstruction end = EndOfPathInstruction.Stop;
    float distance_travelled = 0;

    // Mechanical Move
    bool moving = false;
    float start_dist = 0;

    private void Update()
    {
        // TODO: Rotate Under Weight of Player
        if (move)
        {
            if (!controller.mechanical_movement)
            {
                distance_travelled += controller.platform_move_speed * Time.deltaTime;
                transform.position = controller.spline.path.GetPointAtDistance(distance_travelled, end);
            }

            if (controller.rotate_to_spline)
            {
                transform.rotation = controller.spline.path.GetRotationAtDistance(distance_travelled, end);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            if (distance_travelled >= controller.spline.path.length)
            {
                End();
            }
        }
    }

    public bool MechanicalMove()
    {
        if (moving)
        {
            distance_travelled += controller.platform_move_speed * Time.deltaTime;
            transform.position = controller.spline.path.GetPointAtDistance(distance_travelled, end);

            if (distance_travelled >= start_dist + controller.mechanical_movement_distance)
            {
                moving = false;
                return true;
            }

            return false;
        }
        else
        {
            start_dist = distance_travelled;
            moving = true;
            return false;
        }
    }

    public void StartPlatform(MovingPlatformsController platform_controller)
    {
        controller = platform_controller;
        transform.rotation = Quaternion.LookRotation((controller.spline.path.GetPoint(current_index + 1) - transform.position).normalized);
        move = true;
    }

    void End()
    {
        if (player != null)
        {
            player.transform.parent = null;
            player = null;
        }

        controller.RemovePlatform(this);
        Destroy(gameObject);
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
