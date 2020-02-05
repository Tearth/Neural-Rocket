using UnityEngine;

public class RocketEntity : MonoBehaviour
{
    [Header("General")]
    public Rigidbody RocketRigidbody;
    public AccelerationMeter AccelerationMeter;
    public Transform CenterOfThrust;

    [Header("Control")]
    public float ThrustPercentage;
    public float MaxThrustForce;
    public float MaxGimbal;

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
