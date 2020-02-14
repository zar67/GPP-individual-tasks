using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGCameraController : MonoBehaviour
{
    public Transform target;
    public Transform pivot;
    public float rotation_speed = 3;

    Vector3 offset = new Vector3(0, -5.5f, 5.3f);

    void Awake()
    {
        pivot.position = target.position;
        pivot.parent = target;
    }

    void LateUpdate()
    {
        Quaternion turn_angle = Quaternion.AngleAxis(Input.GetAxis("Camera") * rotation_speed, Vector3.up);
        offset = turn_angle * offset;

        transform.position = target.position - offset;
        transform.LookAt(target.position + new Vector3(0, 2.4f, 0));
    }
}
