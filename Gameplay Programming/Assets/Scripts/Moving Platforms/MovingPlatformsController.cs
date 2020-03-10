using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PathCreation;

public class MovingPlatformsController : SwitchTarget
{
    public PathCreator spline;

    public Transform platforms_parent;
    public GameObject platform_prefab;

    public bool trigger_on_start = false;
    public bool end_after_time = false;
    public float end_delay = 0f;
    public bool mechanical_movement = true;
    public float mechanical_movement_delay;
    public float mechanical_movement_distance = 1;
    public bool rotate_to_spline = false;
    public float distance_between_platforms = 10;
    public float platform_move_speed = 5;

    List<MovingPlatform> platforms = new List<MovingPlatform> { };

    float mechanical_move_timer = 0;
    float end_timer = 0;
    float spawn_timer = 0;
    float time_between_spawns = 10;
    //[HideInInspector]
    public bool triggered;

    void Awake()
    {
        triggered = trigger_on_start;
        time_between_spawns = distance_between_platforms / platform_move_speed;
        spawn_timer = time_between_spawns;

        if (mechanical_movement)
        {
            mechanical_move_timer = mechanical_movement_delay;
            MovingPlatform new_platform = Instantiate(platform_prefab, spline.path.GetPoint(0), Quaternion.identity, platforms_parent).GetComponent<MovingPlatform>();
            new_platform.StartPlatform(this);
            platforms.Add(new_platform);
        }
    }

    void Update()
    {
        if (triggered)
        {
            if (end_after_time)
            {
                end_timer += Time.deltaTime;

                if (end_timer > end_delay)
                {
                    triggered = false;
                    end_timer = 0;
                }
            }

            spawn_timer += Time.deltaTime;
            if (!mechanical_movement && spawn_timer > time_between_spawns)
            {
                spawn_timer = 0;
                MovingPlatform new_platform = Instantiate(platform_prefab, spline.path.GetPoint(0), Quaternion.identity, platforms_parent).GetComponent<MovingPlatform>();
                new_platform.StartPlatform(this);
                platforms.Add(new_platform);
            }

            if (mechanical_movement)
            {
                if (mechanical_move_timer > mechanical_movement_delay)
                {
                    bool finished = false;
                    foreach (MovingPlatform platform in platforms)
                    {
                        if (platform.MechanicalMove())
                        {
                            mechanical_move_timer = 0;
                            finished = true;
                        }
                    }

                    if (finished)
                    {
                        MovingPlatform new_platform = Instantiate(platform_prefab, spline.path.GetPoint(0), Quaternion.identity, platforms_parent).GetComponent<MovingPlatform>();
                        new_platform.StartPlatform(this);
                        platforms.Add(new_platform);
                    }
                }
                else
                {
                    mechanical_move_timer += Time.deltaTime;
                }
            }
        }
    }

    public void StartPlatforms()
    {
        triggered = true;
    }

    public void StopPlatforms()
    {
        triggered = false;
    }

    public void RemovePlatform(MovingPlatform platform)
    {
        platforms.Remove(platform);
    }
}
