using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slime : MonoBehaviour
{
    public Size startSize = Size.MEDIUM;
    public float base_health = 10;
    public float[] sizeScaling = new float[3] { 0.2f, 1, 2 };

    public bool hit = false;
    public Size current_size;
    public float health = 0;
    float attack_damage = 10;
    public float move_speed = 5;
    public float jump_force = 7;
    public float move_delay = 1.5f;

    public Transform center_position;
    public Vector2 area_size;
    public Vector3 movement_position;
    bool player_in_range = false;
    public float move_timer = 0;
    bool jumping = false;

    float attack_delay = 1;
    float attack_timer = 0;

    public Slider healthBarUI;
    Rigidbody rigid_body;
    RPGCameraController player_camera;
    RPGCharacterController player;

    bool move_backwards = false;

    private void Awake()
    {
        rigid_body = GetComponent<Rigidbody>();
        player = FindObjectOfType<RPGCharacterController>();
        player_camera = FindObjectOfType<RPGCameraController>();

        base_health = player.attack_damage;

        health = base_health * ((int)startSize + 1);
        current_size = startSize;
        float scale = sizeScaling[(int)startSize];
        transform.localScale = new Vector3(scale, scale, scale);

        float x_pos = Random.Range(center_position.position.x - area_size.x, center_position.position.x + area_size.x);
        float z_pos = Random.Range(center_position.position.z - area_size.y, center_position.position.z + area_size.y);
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
                float x_pos = Random.Range(center_position.position.x - area_size.x, center_position.position.x + area_size.x);
                float z_pos = Random.Range(center_position.position.z - area_size.y, center_position.position.z + area_size.y);
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
            move_backwards = true;

            if (!jumping)
            {
                move_timer = 0;
                rigid_body.velocity = Vector3.up * jump_force;
                jumping = true;
            }
            
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Walkable"))
        {
            jumping = false;
            move_backwards = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            attack_timer += Time.deltaTime;

            if (attack_timer > attack_delay)
            {
                attack_timer = 0;
                other.GetComponent<RPGCharacterController>().DamagePlayer(attack_damage);
                move_backwards = true;

                if (!jumping)
                {
                    move_timer = 0;
                    rigid_body.velocity = Vector3.up * jump_force;
                    jumping = true;
                }
                
            }
        }
    }

    void Move(Vector3 direction)
    {
        Vector3 rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, transform.up), move_speed * Time.deltaTime).eulerAngles;
        transform.rotation = Quaternion.Euler(0, rotation.y, 0);

        if (move_backwards)
        {
            rigid_body.velocity -= direction.normalized * move_speed * Time.deltaTime;
        }
        else
        {
            if (jumping)
            {
                rigid_body.velocity += direction.normalized * move_speed * Time.deltaTime;
            }
            else
            {
                move_timer += Time.deltaTime;

                if (move_timer > move_delay)
                {
                    move_timer = 0;
                    rigid_body.velocity = Vector3.up * jump_force;
                    jumping = true;
                }
            }
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
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
        slime.health = base_health * ((int)slime.startSize + 1);
        slime.current_size = slime.startSize;
        slime.transform.position = RandomSpawnPosition();
        slime.transform.localScale = new Vector3(scale, scale, scale);
        slime.healthBarUI = slime.GetComponentInChildren<Slider>();
        slime.healthBarUI.maxValue = slime.health;
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
 