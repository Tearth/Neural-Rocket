using UnityEngine;

public class RocketEntity : MonoBehaviour
{
    [Header("General")]
    public Rigidbody RocketRigidbody;
    public AccelerationMeter AccelerationMeter;
    public Transform CenterOfThrust;
    public float DryMass;

    [Header("Control")]
    public float ThrustPercentage;
    public float MaxThrustForce;
    public float MaxGimbal;

    public float FuelPercentage => (RocketRigidbody.mass - DryMass) / (_initialMass - DryMass) * 100;
    public float AngleOfAttack => Vector3.Angle(transform.up, RocketRigidbody.velocity.normalized);

    private float _initialMass;

    private void Start()
    {
        _initialMass = RocketRigidbody.mass;
    }

    private void FixedUpdate()
    {
        ApplyThrustForce();
    }

    private void ApplyThrustForce()
    {
        var force = MaxThrustForce * ThrustPercentage / 100;
        var forceRelativeToRotation = CenterOfThrust.up * force;

        RocketRigidbody.AddForceAtPosition(forceRelativeToRotation, CenterOfThrust.position);
    }
}
