using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGCameraController : MonoBehaviour
{
    public Transform target;

    Vector3 offset;

    void Awake()
    {
        offset = target.position - transform.position;
    }

    void LateUpdate()
    {
        transform.position = target.position - offset;
        transform.LookAt(target.position);
    }
}
