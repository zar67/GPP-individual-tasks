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
    public LayerMask ground;

    // Movement Variables
    public const int base_move_speed = 7;
    public const int strafe_move_speed = 5;
    public float move_speed = 7;
    public float rotate_speed = 125;

    // Jump Variables
    public bool can_double_jump = false;
    public float jump_force = 10;
    public float double_jump_force = 8;
    bool set_jump = false;
    bool set_double_jump = false;

    [HideInInspector]
    public bool double_jump = false;

    // Arming Variables
    public GameObject weapon_sheathed;
    public GameObject weapon_armed;
    public Weapons current_weapon = Weapons.NONE;
    float armed_timer = 0;
    float armed_delay = 5;

    // Collectables
    public List<GameObject> active_collectables;

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
        UpdateAnimator();

        // Rotate
        transform.Rotate(-Vector3.up * -Input.GetAxis("Horizontal") * rotate_speed * Time.deltaTime);

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
            if (IsGrounded())
            {
                set_jump = true;
            }
            // Double Jump
            else if (can_double_jump && !IsGrounded() && (player_animator.GetInteger("jumping") != 0) && !double_jump)
            {
                set_double_jump = true;
            }
        }

        // Strafe
        if (Input.GetAxis("Strafe") != 0)
        {
            move_speed = strafe_move_speed;
            player_animator.SetBool("strafe", true);
        }
        else
        {
            move_speed = base_move_speed;
            player_animator.SetBool("strafe", false);
        }
    }

    void FixedUpdate()
    {
        // Move
        Vector3 velocity = Vector3.zero;

        float movement = Input.GetAxis("Vertical");
        if (movement > 0.1)
        {
            velocity = transform.forward * move_speed;
        }
        else if (movement < -0.1)
        {
            velocity = -transform.forward * move_speed;
        }

        if (IsGrounded() && player_animator.GetInteger("jumping") == 0)
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y = player_rb.velocity.y;
        }

        player_rb.velocity = velocity;

        if (set_jump)
        {
            set_jump = false;
            player_animator.SetInteger("jumping", 1);
            Vector3 position = player_rb.gameObject.transform.position;
            position.y += 0.5f;
            player_rb.gameObject.transform.position = position;
            player_rb.velocity = Vector3.up * jump_force;
            PlayDoubleJumpParticles();
        }

        if (set_double_jump)
        {
            set_double_jump = false;
            double_jump = true;
            player_animator.Play("Double Jump", 0);
            player_rb.velocity = Vector3.up * double_jump_force;
            PlayDoubleJumpParticles();
        }

        if (player_rb.velocity.y <= 0)
        {
            // Land
            if (IsGrounded())
            {
                player_animator.SetInteger("jumping", 0);
                double_jump = false;
            }
            // Fall
            else
            {
                player_animator.SetInteger("jumping", 2);
            }
        }
    }

    void UpdateAnimator()
    {
        // Move
        player_animator.SetFloat("vertical_input", Input.GetAxis("Vertical"));

        // Rotate
        player_animator.SetFloat("turning_input", -Input.GetAxis("Horizontal"));

        // Attack
        if (Input.GetButtonDown("Attack") && !player_animator.GetCurrentAnimatorStateInfo(0).IsName("Double Jump"))
        {
            armed_timer = 0;
            player_animator.SetInteger("attack", Random.Range(1, 7));
        }
        else
        {
            player_animator.SetInteger("attack", 0);
        }

        // Kick
        if (Input.GetButtonDown("Kick") && !player_animator.GetCurrentAnimatorStateInfo(0).IsName("Double Jump"))
        {
            player_animator.SetInteger("kick", Random.Range(1, 3));
        }
        else
        {
            player_animator.SetInteger("kick", 0);
        }

        // Wield
        if (Input.GetButtonDown("Wield") && current_weapon != Weapons.NONE)
        {
            if (player_animator.GetBool("armed"))
            {
                StartCoroutine(Sheath());
            }
            else
            {
                StartCoroutine(Wield());
            }

            armed_timer = 0;
        }

        if (current_weapon == Weapons.NONE)
        {
            weapon_armed.SetActive(false);
            weapon_sheathed.SetActive(false);
        }
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(new Vector3(player_collider.bounds.center.x, player_collider.bounds.min.y + (player_collider.radius * 0.95f), player_collider.bounds.center.z), 
                                   player_collider.radius * 0.95f, 
                                   ground);
    }

    IEnumerator Sheath()
    {
        player_animator.SetBool("armed", false);
        player_animator.SetLayerWeight(1, 0);
        player_animator.SetLayerWeight(3, 0);
        player_animator.Play("Sheath");

        yield return new WaitForSeconds(0.5f);

        weapon_armed.SetActive(false);
        weapon_sheathed.SetActive(true);
    }

    IEnumerator Wield()
    { 
        player_animator.SetBool("armed", true);
        player_animator.SetLayerWeight(1, 1);
        player_animator.SetLayerWeight(3, 1);
        player_animator.Play("Draw Sword");

        yield return new WaitForSeconds(0.25f);

        weapon_armed.SetActive(true);
        weapon_sheathed.SetActive(false);
    }

    public void PlayDoubleJumpParticles()
    {
        foreach (GameObject collectable in active_collectables)
        {
            if (collectable.GetComponent<DoubleJump>() != null)
            {
                collectable.GetComponent<DoubleJump>().PlayParticles();
                break;
            }
        }
    }
}
