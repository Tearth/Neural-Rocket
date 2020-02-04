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
        var particles = ExhaustParticles.main;
        particles.startSpeedMultiplier = _baseSpeed * Rocket.Thrust / 100;
    }
}
