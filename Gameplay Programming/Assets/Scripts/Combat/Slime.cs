using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slime : MonoBehaviour
{
    [Header("Health")]
    public float baseHealth = 10;
    float health = 0;

    [Header("Death")]
    public GameObject deathParticles;

    [Header("Attack")]
    public float baseAttackDamage = 5;
    public float attackDelay = 1;
    float attack_damage = 5;
    float attack_timer = 0;

    [Header("Movement Variables")]
    public float moveSpeed = 5;
    public float jumpForce = 7;
    public float moveDelay = 1.5f;

    Vector3 movement_position;
    bool player_in_range = false;
    float move_timer = 0;
    bool jumping = false;

    [Header("Sizing")]
    public Size startSize = Size.MEDIUM;
    public float[] sizeScaling = new float[3] { 0.2f, 1, 2 };
    Size current_size;
    
    [Header("Patrol Area")]
    public Transform areaCenter;
    public Vector2 areaSize;

    [Header("UI")]
    public Slider healthBarUI;

    [HideInInspector]
    public bool hit = false;
    Rigidbody rigid_body;
    Animator animator;
    RPGCameraController player_camera;
    RPGCharacterController player;

    private void Awake()
    {
        rigid_body = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        player = FindObjectOfType<RPGCharacterController>();
        player_camera = FindObjectOfType<RPGCameraController>();

        baseHealth = player.attack_damage;

        health = baseHealth * ((int)startSize + 1);
        attack_damage = baseAttackDamage * ((int)startSize + 1);
        current_size = startSize;
        float scale = sizeScaling[(int)startSize];
        transform.localScale = new Vector3(scale, scale, scale);

        float x_pos = Random.Range(areaCenter.position.x - areaSize.x, areaCenter.position.x + areaSize.x);
        float z_pos = Random.Range(areaCenter.position.z - areaSize.y, areaCenter.position.z + areaSize.y);
        movement_position = new Vector3(x_pos, 0, z_pos);

        healthBarUI = GetComponentInChildren<Slider>();
        healthBarUI.transform.position = transform.position + (transform.up * sizeScaling[(int)startSize]);
        healthBarUI.maxValue = health;
    }

    private void Update()
    {
        healthBarUI.value = health;
        Vector3 ui_rotation = healthBarUI.transform.position - player_camera.transform.position;
        healthBarUI.transform.rotation = Quaternion.LookRotation(ui_rotation, transform.up);

        if (player_in_range)
        {
            Move(player.transform.position - transform.position);
        }
        else
        {
            Move(movement_position - transform.position);

            if (Vector3.Distance(transform.position, movement_position) < 1)
            {
                move_timer = 0;
                float x_pos = Random.Range(areaCenter.position.x - areaSize.x, areaCenter.position.x + areaSize.x);
                float z_pos = Random.Range(areaCenter.position.z - areaSize.y, areaCenter.position.z + areaSize.y);
                movement_position = new Vector3(x_pos, 0, z_pos);
            }
        }

        if (Vector3.Distance(transform.position, player.transform.position) < 10)
        {
            player_in_range = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            other.GetComponent<RPGCharacterController>().DamagePlayer(attack_damage);

            if (!jumping)
            {
                move_timer = 0;
                rigid_body.velocity = Vector3.up * jumpForce;
                jumping = true;

            }
            
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Walkable"))
        {
            if (jumping)
            {
                animator.SetTrigger("land");
            }
            jumping = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            attack_timer += Time.deltaTime;

            if (attack_timer > attackDelay)
            {
                attack_timer = 0;
                other.GetComponent<RPGCharacterController>().DamagePlayer(attack_damage);
                jumping = false;
                animator.SetTrigger("fall");
            }
        }
    }

    void Move(Vector3 direction)
    {
        Vector3 rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, transform.up), moveSpeed * Time.deltaTime).eulerAngles;
        transform.rotation = Quaternion.Euler(0, rotation.y, 0);

        if (jumping)
        {
            rigid_body.velocity += direction.normalized * moveSpeed * Time.deltaTime;
        }
        else
        {
            move_timer += Time.deltaTime;

            if (move_timer > moveDelay)
            {
                move_timer = 0;
                rigid_body.velocity = Vector3.up * jumpForce;
                jumping = true;
                animator.SetTrigger("jump");
            }
        }
        
    }

    public void TakeDamage(float amount)
    {
        animator.SetTrigger("hit");
        health -= amount;

        if (health <= 0)
        {
            Instantiate(deathParticles, transform.position, Quaternion.identity);
            if (current_size == Size.SMALL)
            {
                Destroy(gameObject);
            }
            else
            {
                Split();
            }
        }
    }

    void Split()
    {
        Slime obj_1 = Instantiate(this);
        SetupNewSlime(obj_1);

        Slime obj_2 = Instantiate(this);
        SetupNewSlime(obj_2);

        Destroy(this.gameObject);
    }

    void SetupNewSlime(Slime slime)
    {
        float scale = sizeScaling[(int)startSize - 1];

        slime.startSize = current_size - 1;
        slime.health = baseHealth * ((int)slime.startSize + 1);
        slime.current_size = slime.startSize;
        slime.transform.position = RandomSpawnPosition();
        slime.transform.localScale = new Vector3(scale, scale, scale);
        slime.healthBarUI = slime.GetComponentInChildren<Slider>();
        slime.healthBarUI.maxValue = slime.health;
        slime.attack_damage = baseAttackDamage * ((int)slime.startSize + 1);
    }

    Vector3 RandomSpawnPosition()
    {
        Vector3 new_pos;
        new_pos.x = Random.Range(transform.position.x - 0.5f, transform.position.x + 0.5f);
        new_pos.y = Random.Range(transform.position.y - 0.5f, transform.position.y + 0.5f);
        new_pos.z = Random.Range(transform.position.z - 0.5f, transform.position.z + 0.5f);

        return new_pos;
    }

    public enum Size
    {
        SMALL = 0,
        MEDIUM = 1,
        LARGE = 2
    };
}
 