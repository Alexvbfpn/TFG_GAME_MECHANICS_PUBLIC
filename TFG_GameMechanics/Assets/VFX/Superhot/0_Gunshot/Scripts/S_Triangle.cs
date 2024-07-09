using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Triangle : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;
    private LineRenderer lineRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        int numParticlesAlive = particleSystem.GetParticles(particles);

        if (numParticlesAlive == 3)
        {
            // Actualizar el material del LineRenderer con el color de las partículas
            lineRenderer.material.color = particles[0].GetCurrentColor(particleSystem);
            
            // Actualiza las posiciones del LineRenderer para formar el triángulo
            lineRenderer.positionCount = 3;
            lineRenderer.SetPosition(0, particles[0].position);
            lineRenderer.SetPosition(1, particles[1].position);
            lineRenderer.SetPosition(2, particles[2].position);
        }
        else
        {
            // Desactiva el LineRenderer si no hay 3 partículas
            lineRenderer.positionCount = 0;
        }
    }
}
