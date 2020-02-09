using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    // Particles
    public GameObject pickup_particles_prefab;
    public GameObject player_particles_prefab;
    GameObject player_particles;

    // Time Limit
    public float collectible_time_limit = 2f;
    [HideInInspector]
    public float elapsed_time = 0f;
    [HideInInspector]
    public bool collected = false;

    // Rotation
    public float rotate_speed = 100f;

    // Component Reference
    [HideInInspector]
    public RPGCharacterController player;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<RPGCharacterController>();
    }

    public virtual void Update()
    {
        if (collected)
        {
            elapsed_time += Time.deltaTime;

            if (elapsed_time >= collectible_time_limit)
            {
                Disable();
                player.active_collectables.Remove(gameObject);
                Destroy(player_particles);
                Destroy(gameObject);
            }
        }
        else
        {
            transform.Rotate(Vector3.up * Time.deltaTime * rotate_speed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if (pickup_particles_prefab != null)
            {
                Instantiate(pickup_particles_prefab, transform.position, transform.rotation);
            }
            if (player_particles_prefab != null)
            {
                player_particles = Instantiate(player_particles_prefab, player.gameObject.transform.position, Quaternion.identity, player.gameObject.transform);
            }

            collected = true;
            other.GetComponent<RPGCharacterController>().active_collectables.Add(gameObject);
            Pickup();

            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<SphereCollider>().enabled = false;

            ParticleSystem particles = gameObject.GetComponentInChildren<ParticleSystem>();
            if (particles)
            {
                ParticleSystem.EmissionModule emmision = particles.emission;
                emmision.enabled = false;
            }
        }
    }

    public virtual void Pickup()
    {
        Debug.Log("Picked Up " + gameObject.name);
    }

    public virtual void Disable()
    {
        Debug.Log("Disabled " + gameObject.name);
    }
}
