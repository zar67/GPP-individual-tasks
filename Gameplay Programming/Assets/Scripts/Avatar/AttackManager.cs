using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    RPGCharacterController player;
    Animator player_animator;

    [HideInInspector]
    public GameObject collided = null;

    private void Awake()
    {
        player = FindObjectOfType<RPGCharacterController>();
        player_animator = player.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //collided = other.gameObject;
        Slime enemy = other.gameObject.GetComponent<Slime>();
        if (enemy != null && !enemy.hit &&
            ((!player_animator.GetBool("armed") && !player_animator.GetCurrentAnimatorStateInfo(2).IsName("Default")) ||
            ((player_animator.GetBool("armed") && !player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(3).IsName("Default")))))
        {
            enemy.hit = true;
            enemy.TakeDamage(player.attack_damage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Slime enemy = other.gameObject.GetComponent<Slime>();
        if (enemy != null)
        {
            enemy.hit = false;
        }
    }
}
