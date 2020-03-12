using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : SwitchTarget
{
    // Close On Timer
    public bool close_after_time;
    public float close_delay;
    float close_timer = 0;

    // Close On Walk Through
    public bool close_after_walked_through;

    // Component References
    Animator door_animator;

    private void Awake()
    {
        door_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (door_animator.GetCurrentAnimatorStateInfo(0).IsName("Default"))
        {
            door_animator.ResetTrigger("close");
        }

        if (close_after_time && !close_after_walked_through && door_animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            close_timer += Time.deltaTime;

            if (close_timer >= close_delay)
            {
                Close();
                close_timer = 0;
            }
        }
    }

    public override void Trigger()
    {
        door_animator.SetTrigger("open");
    }

    public void Close()
    {
        door_animator.SetTrigger("close");
    }
}
