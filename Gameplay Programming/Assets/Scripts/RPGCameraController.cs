using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGCameraController : MonoBehaviour
{
    public Transform player;

    Vector3 offset;

    void Awake()
    {
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        transform.position = player.position + offset;
        
        // If Input
        // Lerp to next 90 degree angle
    }
}
