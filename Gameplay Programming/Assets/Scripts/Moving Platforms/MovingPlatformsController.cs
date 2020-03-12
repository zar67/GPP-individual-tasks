using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PathCreation;

public class MovingPlatformsController : SwitchTarget
{
    [Header("Spline Variables")]
    public PathCreator spline;
    public EndOfPathInstruction endInstruction;

    [Header("References")]
    public Transform platformsParent;
    public GameObject platformsPrefab;

    [Header("Trigger Variables")]
    public bool triggerOnStart = false;
    public bool stopAfterTime = false;
    public float stopDelay = 0f;

    [Header("Movement Variables")]
    public float distanceBetweenPlatforms = 10;
    public float platformMoveSpeed = 3;

    [Header("Rotation Variables")]
    public bool rotateXWithSpline = false;
    public bool rotateYWithSpline = false;
    public bool rotateZWithSpline = false;

    [Header("Mechanical Movement")]
    public bool mechanicalMovement = false;
    public float mechanicalDelay = 2;
    public float mechanicalDistance = 1;

    

    List<MovingPlatform> platforms = new List<MovingPlatform> { };

    float mechanical_move_timer = 0;
    float end_timer = 0;
    float spawn_timer = 0;
    float time_between_spawns = 10;

    [HideInInspector]
    public bool triggered;

    void Awake()
    {
        triggered = triggerOnStart;
        time_between_spawns = distanceBetweenPlatforms / platformMoveSpeed;

        if (triggered)
        {
            MovingPlatform new_platform = Instantiate(platformsPrefab, spline.path.GetPoint(0), Quaternion.identity, platformsParent).GetComponent<MovingPlatform>();
            new_platform.StartPlatform(this);
            platforms.Add(new_platform);
        }
        
    }

    void Update()
    {
        if (triggered)
        {
            if (stopAfterTime)
            {
                end_timer += Time.deltaTime;

                if (end_timer > stopDelay)
                {
                    triggered = false;
                    end_timer = 0;
                }
            }

            spawn_timer += Time.deltaTime;
            if (!mechanicalMovement && endInstruction != EndOfPathInstruction.Reverse && spawn_timer > time_between_spawns)
            {
                spawn_timer = 0;
                MovingPlatform new_platform = Instantiate(platformsPrefab, spline.path.GetPoint(0), Quaternion.identity, platformsParent).GetComponent<MovingPlatform>();
                new_platform.StartPlatform(this);
                platforms.Add(new_platform);
            }

            if (mechanicalMovement)
            {
                if (mechanical_move_timer > mechanicalDelay)
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
                        MovingPlatform new_platform = Instantiate(platformsPrefab, spline.path.GetPoint(0), Quaternion.identity, platformsParent).GetComponent<MovingPlatform>();
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
        spawn_timer = time_between_spawns;
        triggered = true;

        if (mechanicalMovement)
        {
            mechanical_move_timer = mechanicalDelay;
            MovingPlatform new_platform = Instantiate(platformsPrefab, spline.path.GetPoint(0), Quaternion.identity, platformsParent).GetComponent<MovingPlatform>();
            new_platform.StartPlatform(this);
            platforms.Add(new_platform);
        }

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