using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    public DoorController target;

    Animator switch_animator;
    RPGCharacterController player;
    bool player_in_range = false;

    bool clicked = false;
    float click_delay = 0;
    float click_timer = 0;

    private void Awake()
    {
        switch_animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<RPGCharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            player_in_range = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            player_in_range = false;
        }
    }

    private void Update()
    {
        if (player.hit && player_in_range && !clicked && target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Default"))
        {
            clicked = true;

            switch_animator.SetTrigger("clicked");
            target.Open();

            if (target.close_after_time)
            {
                click_delay = target.close_delay;
            }
        }

        if (clicked)
        {
            if (target.close_after_time && !target.close_after_walked_through)
            {
                click_timer += Time.deltaTime;

                if (click_timer >= click_delay)
                {
                    Release();
                }
            }
        }
    }

    void Release()
    {
        switch_animator.SetTrigger("released");
        clicked = false;
        click_timer = 0;
    }
}
