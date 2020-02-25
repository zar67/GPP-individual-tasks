using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGCameraController : MonoBehaviour
{
    public Transform target;
    public Transform pivot;
    public LayerMask camera_blocking_objects;
    public float rotation_speed = 3;
    public bool lock_to_axis;
    public float lock_delay = 1.5f;

    bool move_camera = true;
    bool lerping_to_axis = false;

    Vector3[] directions = new Vector3[4] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
    Vector2 base_offset = new Vector2(5.5f, 2.9f);
    Vector3 offset;
    Vector3 nearest_dir;

    float camera_timer = 0;
    float distance;
    float zoom_value = 1;

    void Awake()
    {
        pivot.position = target.position;
        pivot.parent = target;
        ResetCamera();

        distance = Vector3.Distance(target.position, transform.position);
    }

    void LateUpdate()
    {
        if (move_camera)
        {
            if (Input.GetAxis("CameraVertical") > 0.75f)
            {
                zoom_value = Mathf.Clamp(zoom_value * 0.95f, 0.35f, 1.65f);
            }
            else if(Input.GetAxis("CameraVertical") < -0.75f)
            {
                zoom_value = Mathf.Clamp(zoom_value * 1.05f, 0.35f, 1.65f);
            }

            if (Input.GetAxis("CameraHorizontal") == 0 && lock_to_axis)
            {
                if (lerping_to_axis)
                {
                    offset = Vector3.Lerp(offset, nearest_dir, rotation_speed * Time.deltaTime);
                    Debug.Log(zoom_value);
                    zoom_value = Mathf.Lerp(zoom_value, 1, rotation_speed * Time.deltaTime);

                    if (Vector3.Distance(offset * zoom_value, nearest_dir) < 0.5f)
                    {
                        lerping_to_axis = false;
                    }
                }
                else
                {
                    camera_timer += Time.deltaTime;
                }

                if (camera_timer > lock_delay)
                {
                    camera_timer = 0;
                    lerping_to_axis = true;
                    GetNearestDirection();
                }
            }
            else
            {
                Quaternion turn_angle = Quaternion.AngleAxis(Input.GetAxis("CameraHorizontal") * rotation_speed, Vector3.up);
                offset = turn_angle * offset;
                lerping_to_axis = false;
            }

            transform.position = target.position - (offset * zoom_value);

            // Raycasting
            Vector3 dir = transform.position - target.position;
            RaycastHit hit;
            if (Physics.Raycast(target.position, dir, out hit, distance, camera_blocking_objects))
            {
                transform.position = target.position + (dir.normalized * hit.distance * 0.95f);
            }

            transform.LookAt(target.position);
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
        offset = (target.forward * base_offset.x) - (target.up * base_offset.y);
        transform.position = target.position - offset;
        transform.LookAt(target.position);
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
                nearest_dir = (-directions[i] * base_offset.x) - (target.up * base_offset.y);
                break;
            }
        }
    }
}
