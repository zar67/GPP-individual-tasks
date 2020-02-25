﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGCameraController : MonoBehaviour
{
    public Transform target;
    public Transform pivot;
    public float rotation_speed = 3;

    bool move_camera = true;
    Vector3 offset;
    Vector3[] directions;
    Vector3 nearest_dir;
    float camera_timer = 0;
    bool lerping = false;

    void Awake()
    {
        directions = new Vector3[4] {Vector3.forward, Vector3.right, Vector3.back, Vector3.left};

        pivot.position = target.position;
        pivot.parent = target;
        ResetCamera();
    }

    void LateUpdate()
    {
        if (move_camera)
        {
            if (Input.GetAxis("Camera") == 0)
            {
                if (lerping)
                {
                    //offset = (-directions[nearest_dir] * 5.5f) - (target.up * 5.3f);
                    offset = Vector3.Lerp(offset, nearest_dir, rotation_speed * Time.deltaTime);

                    if (Vector3.Distance(offset, nearest_dir) < 0.5f)
                    {
                        lerping = false;
                    }
                }
                else
                {
                    camera_timer += Time.deltaTime;
                }

                if (camera_timer > 2)
                {
                    camera_timer = 0;
                    lerping = true;
                    GetNearestDirection();
                }
            }
            else
            {
                Quaternion turn_angle = Quaternion.AngleAxis(Input.GetAxis("Camera") * rotation_speed, Vector3.up);
                offset = turn_angle * offset;
            }

            transform.position = target.position - offset;
            transform.LookAt(target.position + new Vector3(0, 2.4f, 0));
        }
    }

    public void DisableCamera()
    {
        move_camera = false;
    }

    public void EnableCamera()
    {
        move_camera = true;
    }
    public void ResetCamera()
    {
        offset = (target.forward * 5.5f) - (target.up * 5.3f);
        transform.position = target.position - offset;
        transform.LookAt(target.position + new Vector3(0, 2.4f, 0));
        move_camera = true;
    }

    void GetNearestDirection()
    {
        Vector3 difference = transform.position - target.position;
        difference.y = 0;

        float[] angles = new float[4];
        angles[0] = Vector3.Angle(difference, Vector3.forward);
        angles[1] = Vector3.Angle(difference, Vector3.right);
        angles[2] = Vector3.Angle(difference, Vector3.back);
        angles[3] = Vector3.Angle(difference, Vector3.left);

        float nearest = Mathf.Min(Mathf.Min(Mathf.Min(angles[0], angles[1]), angles[2]), angles[3]);

        nearest_dir = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            if (angles[i] == nearest)
            {
                nearest_dir = (-directions[i] * 5.5f) - (target.up * 5.3f);
                break;
            }
        }
    }
}
