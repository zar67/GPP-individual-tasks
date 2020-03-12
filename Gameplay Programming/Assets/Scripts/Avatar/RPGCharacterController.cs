using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapons
{
    NONE,
    TWO_HANDED_SWORD
}

public class RPGCharacterController : MonoBehaviour
{
    // Camera
    public Transform player_camera;

    public LayerMask ground;

    [HideInInspector]
    public bool accept_input = true;

    // Movement Variables
    public bool grounded = false;
    public const int base_move_speed = 7;
    public const int strafe_move_speed = 5;

    public float move_speed = 7;
    public float rotate_speed = 125;

    // Jump Variables
    [HideInInspector]
    public bool can_double_jump = false;
    public float jump_force = 10;
    public float double_jump_force = 8;
    [HideInInspector]
    public bool set_jump = false;
    [HideInInspector]
    public bool set_double_jump = false;

    [HideInInspector]
    public bool has_double_jumped = false;

    // Arming Variables
    bool armed_pressed = false;
    public GameObject weapon_sheathed;
    public GameObject weapon_armed;
    public Weapons current_weapon = Weapons.NONE;
    float armed_timer = 0;
    float armed_delay = 5;

    // Collectables
    public SpeedBoost speed_boost;
    public DoubleJump double_jump;

    // Attack 
    [HideInInspector]
    public bool hit;

    // Switches
    public bool in_range_of_switch = false;

    // Component References
    Animator player_animator;
    Rigidbody player_rb;
    CapsuleCollider player_collider;

    private void Awake()
    {
        // Set Component References
        player_animator = GetComponent<Animator>();
        player_rb = GetComponent<Rigidbody>();
        player_collider = GetComponent<CapsuleCollider>();

        // Set To Unarmed
        current_weapon = Weapons.TWO_HANDED_SWORD;
        Sheath();
    }

    void Update()
    {
        if (accept_input)
        {
            UpdateAnimator();

            // Arm
            if (player_animator.GetBool("armed"))
            {
                armed_timer += Time.deltaTime;

                if (armed_timer >= armed_delay)
                {
                    StartCoroutine(Sheath());
                    armed_timer = 0;
                }
            }

            // Set Jump Bools
            if (Input.GetButtonDown("Jump"))
            {
                // Jump
                if (grounded)
                {
                    set_jump = true;
                }
                // Double Jump
                else if (can_double_jump && !grounded && (player_animator.GetInteger("jumping") != 0) && !has_double_jumped)
                {
                    set_double_jump = true;
                }
            }

            // Strafe
            if (Input.GetAxis("Strafe") != 0)
            {
                if (speed_boost != null)
                {
                    move_speed = strafe_move_speed * 2;
                }
                else
                {
                    move_speed = strafe_move_speed;
                }

                player_animator.SetBool("strafe", true);
            }
            else
            {
                if (speed_boost != null)
                {
                    move_speed = base_move_speed * 2;
                }
                else
                {
                    move_speed = base_move_speed;
                }

                player_animator.SetBool("strafe", false);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                Death();
            }
        }
    }

    void FixedUpdate()
    {
        if (accept_input)
        {
            // Move
            Vector3 velocity = (player_camera.transform.forward * Input.GetAxis("Vertical")) + (player_camera.transform.right * Input.GetAxis("Horizontal"));
            velocity = velocity.normalized * move_speed;

            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                transform.rotation = Quaternion.Euler(0f, player_camera.rotation.eulerAngles.y, 0f);
                Quaternion new_rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0, velocity.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, new_rotation, rotate_speed * Time.deltaTime);
            }

            if (grounded && player_animator.GetInteger("jumping") == 0)
            {
                velocity.y = 0;
            }
            else
            {
                velocity.y = player_rb.velocity.y;
            }

            player_rb.velocity = velocity;
        }

        if (player_rb.velocity.y <= 0)
        {
            // Land
            if (grounded)
            {
                player_animator.SetInteger("jumping", 0);
                has_double_jumped = false;
            }
            // Fall
            else if (!player_animator.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
            {
                if (player_animator.GetInteger("jumping") == 0)
                {
                    player_animator.Play("Fall", 0);
                }

                player_animator.SetInteger("jumping", 2);
            }
        }

        if (set_jump)
        {
            set_jump = false;
            player_animator.SetInteger("jumping", 1);
            player_rb.velocity = Vector3.up * jump_force;
        }

        if (set_double_jump)
        {
            set_double_jump = false;
            has_double_jumped = true;
            player_animator.Play("Double Jump", 0);
            player_rb.velocity = Vector3.up * double_jump_force;
        }
    }

    public void UpdateAnimator()
    {
        // Move
        if (accept_input)
        {
            player_animator.SetFloat("vertical_input", Input.GetAxis("Vertical"));
        }
        player_animator.SetFloat("horizontal_input", -Input.GetAxis("Horizontal"));
        
        if ((accept_input && Input.GetAxis("Vertical") != 0) || Input.GetAxis("Horizontal") != 0)
        {
            player_animator.SetBool("move", true);
        }
        else
        {
            player_animator.SetBool("move", false);
        }

        // Attack
        if (Input.GetButtonDown("LeftAttack") && 
            !in_range_of_switch && 
            !player_animator.GetCurrentAnimatorStateInfo(0).IsName("Double Jump") && 
            player_animator.GetCurrentAnimatorStateInfo(2).IsName("Default") &&
            player_animator.GetCurrentAnimatorStateInfo(3).IsName("Default"))
        {
            armed_timer = 0;
            player_animator.SetInteger("attack", -1);
        }
        else if (Input.GetButtonDown("RightAttack") && 
            !player_animator.GetCurrentAnimatorStateInfo(0).IsName("Double Jump") && 
            player_animator.GetCurrentAnimatorStateInfo(2).IsName("Default") &&
            player_animator.GetCurrentAnimatorStateInfo(3).IsName("Default"))
        {
            armed_timer = 0;
            player_animator.SetInteger("attack", 1);
        }
        else
        {
            player_animator.SetInteger("attack", 0);
        }

        // Kick
        if (Input.GetButtonDown("Kick") && 
            !player_animator.GetCurrentAnimatorStateInfo(0).IsName("Double Jump") &&
            player_animator.GetCurrentAnimatorStateInfo(4).IsName("Default") &&
            player_animator.GetCurrentAnimatorStateInfo(5).IsName("Default"))
        {
            player_animator.SetInteger("kick", Random.Range(1, 3));
        }
        else
        {
            player_animator.SetInteger("kick", 0);
        }

        // Wield
        if (Input.GetAxis("Wield") > 0 && !armed_pressed && current_weapon != Weapons.NONE)
        {
            if (player_animator.GetBool("armed"))
            {
                StartCoroutine(Sheath());
            }
            else
            {
                StartCoroutine(Wield());
            }

            armed_pressed = true;
            armed_timer = 0;
        }
        if (Input.GetAxis("Wield") == 0)
        {
            armed_pressed = false;
        }

        if (current_weapon == Weapons.NONE)
        {
            weapon_armed.SetActive(false);
            weapon_sheathed.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Walkable"))
        {
            grounded = true;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
        {
            grounded = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Walkable"))
        {
            grounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        grounded = false;
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(new Vector3(player_collider.bounds.center.x, player_collider.bounds.min.y + (player_collider.radius * 0.9f), player_collider.bounds.center.z), 
                                   player_collider.radius * 0.95f, 
                                   ground);
    }

    public IEnumerator Sheath()
    {
        player_animator.SetLayerWeight(1, 0);
        player_animator.SetLayerWeight(3, 0);
        player_animator.SetLayerWeight(5, 0);
        player_animator.Play("Sheath");

        yield return new WaitForSeconds(0.5f);

        weapon_armed.SetActive(false);
        weapon_sheathed.SetActive(true);
        player_animator.SetBool("armed", false);
    }

    IEnumerator Wield()
    { 
        player_animator.SetLayerWeight(1, 1);
        player_animator.SetLayerWeight(3, 1);
        player_animator.SetLayerWeight(5, 1);
        player_animator.Play("Draw Sword");

        yield return new WaitForSeconds(0.25f);

        weapon_armed.SetActive(true);
        weapon_sheathed.SetActive(false);
        player_animator.SetBool("armed", true);
    }

    public void ResetAnimator()
    {
        player_animator.SetBool("move", false);
        player_animator.SetFloat("vertical_input", 0);
        player_animator.SetFloat("horizontal_input", 0);
        player_animator.SetInteger("jumping", 0);
        player_animator.SetInteger("attack", 0);
        player_animator.SetInteger("kick", 0);
        player_animator.SetBool("strafe", false);

        player_rb.velocity = new Vector3(0, player_rb.velocity.y, 0);
    }

    public void Death()
    {
        player_animator.SetTrigger("death");
        accept_input = false;
        ResetAnimator();
    }

    public void DamagePlayer()
    {

    }

    void Hit()
    {
        hit = true;
    }

    void NotHit()
    {
        hit = false;
    }
}
