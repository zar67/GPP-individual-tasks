using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    // Particles
    public GameObject pickup_particles_prefab;

    // Time Limit
    public float collectible_time_limit = 2f;
    float elapsed_time = 0f;
    bool collected = false;

    // Rotation
    public float rotate_speed = 100f;

    // Component Reference
    [HideInInspector]
    public RPGCharacterController player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<RPGCharacterController>();
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * rotate_speed);
        
        if (collected)
        {
            elapsed_time += Time.deltaTime;

            if (elapsed_time >= collectible_time_limit)
            {
                Disable();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            Instantiate(pickup_particles_prefab, transform.position, transform.rotation).GetComponent<ParticleSystem>();
            collected = true;
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
