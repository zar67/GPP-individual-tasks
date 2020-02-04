using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapons
{
    NONE,
    TWO_HANDED_SWORD
}

// TODO: Rolling
// TODO: Kicks
// TODO: Death
// TODO: Hit
// TODO: Stunned

public class RPGCharacterController : MonoBehaviour
{    
    public LayerMask ground;

    // Movement Variables
    public const int base_move_speed = 7;
    public float move_speed = 7;
    public float rotate_speed = 150;

    // Jump Variables
    public bool can_double_jump = false;
    public float jump_force = 10;
    public float double_jump_force = 8;

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

        if (IsGrounded())
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y = player_rb.velocity.y;
        }

        player_rb.velocity = velocity;

        if (Input.GetButtonDown("Jump"))
        {
            // Jump
            if (IsGrounded())
            {
                player_animator.SetInteger("jumping", 1);
                player_rb.velocity = Vector3.up * jump_force;
                PlayDoubleJumpParticles();
            }
            // Double Jump
            else if (can_double_jump && !IsGrounded() && player_animator.GetInteger("jumping") == 2 && !double_jump)
            {
                double_jump = true;
                player_animator.Play("Double Jump", 0);
                player_rb.velocity = Vector3.up * double_jump_force;
                PlayDoubleJumpParticles();
            }
        }

        if (player_rb.velocity.y <= 0)
        {
            // Fall
            if (!IsGrounded())
            {
                player_animator.SetInteger("jumping", 2);
            }
            // Land
            else
            {
                player_animator.SetInteger("jumping", 0);
                double_jump = false;
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
        return Physics.CheckSphere(new Vector3(player_collider.bounds.center.x, player_collider.bounds.min.y, player_collider.bounds.center.z), 
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
