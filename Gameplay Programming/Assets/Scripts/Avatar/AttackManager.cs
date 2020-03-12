using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    RPGCharacterController player;

    [HideInInspector]
    public GameObject collided = null;

    private void Awake()
    {
        player = FindObjectOfType<RPGCharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        collided = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        collided = null;
    }
}
