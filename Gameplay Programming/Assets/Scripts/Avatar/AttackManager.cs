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
        Slime enemy = other.gameObject.GetComponent<Slime>();
        AttackEnemy(enemy);
    }

    private void OnTriggerStay(Collider other)
    {
        Slime enemy = other.gameObject.GetComponent<Slime>();
        AttackEnemy(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        Slime enemy = other.gameObject.GetComponent<Slime>();
        if (enemy != null && !player.playerAttacking())
        {
            enemy.hit = false;
        }
    }

    void AttackEnemy(Slime enemy)
    {
        Debug.Log(player.hit_landed);
        if (enemy != null && !enemy.hit && player.playerAttacking())
        {
            enemy.hit = true;
            enemy.TakeDamage(player.attack_damage);
        }

        if (!player.playerAttacking())
        {
            enemy.hit = false;
        }
    }
}
