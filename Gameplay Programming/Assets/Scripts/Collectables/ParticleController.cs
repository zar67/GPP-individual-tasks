using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!particles.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
