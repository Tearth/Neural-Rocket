using System;
using UnityEngine;

public class ExhaustEntity : MonoBehaviour
{
    public RocketEntity Rocket;
    public ParticleSystem ExhaustParticles;

    private float _baseSpeed;

    private void Start()
    {
        _baseSpeed = ExhaustParticles.main.startSpeedMultiplier;
    }

    private void Update()
    {
        if (Math.Abs(Rocket.ThrustPercentage) > float.Epsilon)
        {
            if (!ExhaustParticles.isPlaying)
            {
                ExhaustParticles.Play();
            }

            var particles = ExhaustParticles.main;
            particles.startSpeedMultiplier = _baseSpeed * Rocket.ThrustPercentage / 100;
        }
        else
        {
            ExhaustParticles.Stop();
        }
    }
}
