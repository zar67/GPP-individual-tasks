using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject collided = null;

    private void OnTriggerEnter(Collider other)
    {
        collided = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        collided = null;
    }
}
