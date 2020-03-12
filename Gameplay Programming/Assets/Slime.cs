using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slime : MonoBehaviour
{
    
    public Size startSize = Size.MEDIUM;
    public float starting_health = 30;
    public float[] sizeScaling = new float[3] { 0.2f, 1, 2 };

    public Size current_size;
    public float health = 0;
    float damage = 10;

    public Slider healthBarUI;
    RPGCameraController player_camera;

    private void Awake()
    {
        player_camera = GameObject.FindObjectOfType<RPGCameraController>();

        health = starting_health;
        current_size = startSize;
        float scale = sizeScaling[(int)startSize];
        transform.localScale = new Vector3(scale, scale, scale);

        healthBarUI = GetComponentInChildren<Slider>();
        healthBarUI.transform.position = transform.position + (transform.up * sizeScaling[(int)startSize]);
        healthBarUI.maxValue = health;
    }

    private void Update()
    {
        healthBarUI.value = health;
        Vector3 direction = healthBarUI.transform.position - player_camera.transform.position;
        healthBarUI.transform.rotation = Quaternion.LookRotation(direction, transform.up);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            other.GetComponent<RPGCharacterController>().DamagePlayer(damage);
        }
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("TAKE DAMAGE");
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
        Slime obj_2 = Instantiate(this);
    
        float scale = sizeScaling[(int)startSize - 1];

        obj_1.startSize = current_size - 1;
        obj_1.health = starting_health;
        obj_1.current_size = obj_1.startSize;
        obj_1.transform.position = RandomSpawnPosition();
        obj_1.transform.localScale = new Vector3(scale, scale, scale);
        obj_1.healthBarUI = obj_1.GetComponentInChildren<Slider>();
        obj_1.healthBarUI.maxValue = obj_1.health;

        obj_2.startSize = current_size - 1;
        obj_2.health = starting_health;
        obj_2.current_size = obj_2.startSize;
        obj_2.transform.position = RandomSpawnPosition();
        obj_2.transform.localScale = new Vector3(scale, scale, scale);
        obj_2.healthBarUI = obj_2.GetComponentInChildren<Slider>();
        obj_2.healthBarUI.maxValue = obj_2.health;

        Destroy(this.gameObject);
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
 