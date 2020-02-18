using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorSwitch : MonoBehaviour
{
    public DoorController target;

    public CanvasGroup interact_UI;
    bool show_ui = false;

    Animator switch_animator;
    RPGCharacterController player;
    bool player_in_range = false;

    bool clicked = false;
    float click_timer = 0;

    Vector3 start_position;

    private void Awake()
    {
        switch_animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<RPGCharacterController>();
        start_position = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            player_in_range = true;
            player.in_range_of_switch = true;
            show_ui = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            player_in_range = false;
            player.in_range_of_switch = false;
            show_ui = false;
        }
    }

    private void Update()
    {
        if (show_ui && interact_UI.alpha != 2)
        {
            float new_alpha = Mathf.Lerp(interact_UI.alpha, 2, 0.1f);

            if (new_alpha > 1.95f)
            {
                new_alpha = 2;
            }

            interact_UI.alpha = new_alpha;
        }
        else if (!show_ui && interact_UI.alpha != 0)
        {
            float new_alpha = Mathf.Lerp(interact_UI.alpha, 0, 0.1f);

            if (new_alpha < 0.05f)
            {
                new_alpha = 0;
            }

            interact_UI.alpha = new_alpha;
        }

        if (Input.GetButtonDown("LeftAttack") && 
            player_in_range && 
            !clicked && 
            target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Default") && 
            GetComponent<DoorCutscene>().state == DoorCutscene.CutsceneState.NONE)
        {
            GetComponent<DoorCutscene>().StartCutscene();
            show_ui = false;
        }

        if (clicked && target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Close"))
        {
            Release();
        }

        if (switch_animator.GetCurrentAnimatorStateInfo(0).IsName("Default") && transform.position != start_position)
        {
            transform.position = start_position;
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