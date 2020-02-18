using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGCameraController : MonoBehaviour
{
    public Transform target;
    public Transform pivot;
    public float rotation_speed = 3;

    bool move_camera = true;
    Vector3 offset = new Vector3(0, -5.5f, 5.3f);

    void Awake()
    {
        pivot.position = target.position;
        pivot.parent = target;
        ResetCamera();
    }

    void LateUpdate()
    {
        if (move_camera)
        {
            Quaternion turn_angle = Quaternion.AngleAxis(Input.GetAxis("Camera") * rotation_speed, Vector3.up);
            offset = turn_angle * offset;
            transform.position = target.position - offset;
            transform.LookAt(target.position + new Vector3(0, 2.4f, 0));
        }
    }

    public void DisableCamera()
    {
        move_camera = false;
    }

    public void ResetCamera()
    {
        offset = (target.forward * 5.5f) - (target.up * 5.3f);
        transform.LookAt(target.position + new Vector3(0, 2.4f, 0));
        move_camera = true;
    }
}
