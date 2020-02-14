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
            player.in_range_of_switch = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            player_in_range = false;
            player.in_range_of_switch = false;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("LeftAttack") && player_in_range && !clicked && target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Default"))
        {
            GetComponent<DoorCutscene>().StartCutscene();
        }

        if (clicked &&target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Close"))
        {
            Release();
        }
    }

    public void Click()
    {
        clicked = true;
        switch_animator.SetTrigger("clicked");
    }

    void Release()
    {
        switch_animator.SetTrigger("released");
        clicked = false;
        click_timer = 0;
    }
}
