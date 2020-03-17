using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    RPGCharacterController player;
    Animator player_animator;

    [HideInInspector]
    public GameObject collided = null;
    bool hit_something = false;

    private void Awake()
    {
        player = FindObjectOfType<RPGCharacterController>();
        player_animator = player.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player_animator.GetBool("armed"))
        {
            Slime enemy = other.gameObject.GetComponent<Slime>();
            AttackEnemy(enemy);
        }
        else
        {
            if (!hit_something)
            {
                Slime enemy = other.gameObject.GetComponent<Slime>();
                AttackEnemy(enemy);
            }
        }
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
        if (enemy != null)
        {
            if (!enemy.hit && player.playerAttacking())
            {
                enemy.hit = true;
                enemy.TakeDamage(player.attack_damage);
                hit_something = true;
            }

            if (!player.playerAttacking())
            {
                enemy.hit = false;
                hit_something = false;
            } 
        }
    }
}
