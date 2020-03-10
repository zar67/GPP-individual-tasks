using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public SwitchTarget target;
    public CanvasGroup interact_UI;

    protected Animator switch_animator;
    protected RPGCharacterController player;
    protected bool player_in_range = false;

    protected bool clicked = false;
    protected Vector3 start_position;

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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            player_in_range = false;
            player.in_range_of_switch = false;
        }
    }

    private void Update()
    {
        if (!clicked &&
            player_in_range &&
            interact_UI.alpha != 2)
        {
            float new_alpha = Mathf.Lerp(interact_UI.alpha, 2, 0.1f);

            if (new_alpha > 1.95f)
            {
                new_alpha = 2;
            }

            interact_UI.alpha = new_alpha;
        }
        else if ((clicked || !player_in_range) && interact_UI.alpha != 0)
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
            !Triggered() &&
            GetComponent<Cutscene>().state == Cutscene.CutsceneState.NONE)
        {
            GetComponent<Cutscene>().StartCutscene();
        }

        if (ReleaseButton())
        {
            Release();
        }

        if (switch_animator.GetCurrentAnimatorStateInfo(0).IsName("Default") && transform.position != start_position)
        {
            transform.position = start_position;
        }
    }

    public virtual bool Triggered()
    {
        return clicked;
    }

    public virtual bool ReleaseButton()
    {
        return Triggered();
    }

    public void Click()
    {
        clicked = true;
        switch_animator.SetTrigger("clicked");
    }

    public void Release()
    {
        switch_animator.SetTrigger("released");
        clicked = false;
    }
}

public class SwitchTarget : MonoBehaviour
{
    public virtual void Trigger()
    {
        Debug.Log("Triggered");
    }
}