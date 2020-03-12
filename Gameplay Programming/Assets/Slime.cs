using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    public Size startSize = Size.MEDIUM;
    public float starting_health = 20;
    public float[] sizeScaling = new float[3] { 0.2f, 1, 2 };

    public Size current_size;
    public float health = 0;
    float damage = 10;

    private void Awake()
    {
        health = starting_health * (int)startSize;
        current_size = startSize;
        float scale = sizeScaling[(int)startSize];

        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag.Equals("Player"))
        {
            collision.collider.GetComponent<RPGCharacterController>().DamagePlayer(damage);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Destroy(gameObject);
        }

        float health_threshold = (starting_health * (int)startSize) / 4 * (int)current_size;
        if (health <= health_threshold && current_size != Size.SMALL)
        {
            Split();
        }
    }

    void Split()
    {
        Slime obj_1 = Instantiate(this);
        Slime obj_2 = Instantiate(this);
    
        float scale = sizeScaling[(int)startSize - 1];

        obj_1.startSize = current_size - 1;
        obj_1.health = starting_health * (int)obj_1.startSize;
        obj_1.current_size = obj_1.startSize;
        obj_1.transform.position = RandomSpawnPosition();
        obj_1.transform.localScale = new Vector3(scale, scale, scale);

        obj_2.startSize = current_size - 1;
        obj_2.health = starting_health * (int)obj_2.startSize;
        obj_2.current_size = obj_2.startSize;
        obj_2.transform.position = RandomSpawnPosition();
        obj_2.transform.localScale = new Vector3(scale, scale, scale);

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
        SMALL = 1,
        MEDIUM = 2,
        LARGE = 3
    };
}
 