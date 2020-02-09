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

    // Respawning
    bool active = true;
    float respawn_delay = 5f;
    float respawn_timer = 0;

    // Rotation
    public float rotate_speed = 100f;

    // Component Reference
    [HideInInspector]
    public RPGCharacterController player;
    MeshRenderer renderer;
    SphereCollider collider;
    ParticleSystem default_particles;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<RPGCharacterController>();
        renderer = GetComponent<MeshRenderer>();
        collider = GetComponent<SphereCollider>();
        default_particles = gameObject.GetComponentInChildren<ParticleSystem>();
    }

    public virtual void Update()
    {
        if (collected)
        {
            elapsed_time += Time.deltaTime;

            if (elapsed_time >= collectible_time_limit)
            {
                Disable();
                Destroy(player_particles);
            }
        }
        
        if (active)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * rotate_speed);
        }
        else
        {
            respawn_timer += Time.deltaTime;

            if (respawn_timer >= respawn_delay)
            {
                Respawn();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            Collect();
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

    void Collect()
    {
        Pickup();

        elapsed_time = 0;

        collected = true;
        active = false;

        renderer.enabled = false;
        collider.enabled = false;

        if (default_particles)
        {
            ParticleSystem.EmissionModule emmision = default_particles.emission;
            emmision.enabled = false;
        }

        if (pickup_particles_prefab != null)
        {
            Instantiate(pickup_particles_prefab, transform.position, transform.rotation);
        }
        if (player_particles_prefab != null)
        {
            player_particles = Instantiate(player_particles_prefab, player.gameObject.transform.position, Quaternion.identity, player.gameObject.transform);
        }
    }

    void Respawn()
    {
        respawn_timer = 0;
        active = true;

        renderer.enabled = true;
        collider.enabled = true;

        if (default_particles)
        {
            ParticleSystem.EmissionModule emmision = default_particles.emission;
            emmision.enabled = true;
        }
    }
}
