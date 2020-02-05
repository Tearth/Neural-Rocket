using System;
using UnityEngine;

public class RocketEntity : MonoBehaviour
{
    [Header("General")]
    public Rigidbody RocketRigidbody;
    public AccelerationMeter AccelerationMeter;
    public Transform CenterOfThrust;
    public Transform Payload;
    public float DryMass;
    public float ZeroDragHeight;

    [Header("Control")]
    public float ThrustPercentage;
    public float MaxThrustForce;
    public float MaxGimbal;
    public float PayloadDrag;
    public float MaxFuelUsage;

    public float FuelPercentage => (RocketRigidbody.mass - DryMass) / (_initialMass - DryMass) * 100;
    public float AngleOfAttack => Vector3.Angle(transform.up, RocketRigidbody.velocity.normalized);

    private float _initialMass;
    private float _initialDrag;
    private float _initialAngularDrag;

    private void Start()
    {
        _initialMass = RocketRigidbody.mass;
        _initialDrag = RocketRigidbody.drag;
        _initialAngularDrag = RocketRigidbody.angularDrag;
    }

    private void FixedUpdate()
    {
        if (FuelPercentage <= 0)
        {
            ThrustPercentage = 0;
        }

        UpdateMass();
        UpdateDrag();
        ApplyThrustForce();
        ApplyPayloadDragForce();
    }

    private void UpdateMass()
    {
        if (FuelPercentage > 0)
        {
            RocketRigidbody.mass -= MaxFuelUsage * ThrustPercentage / 100;
        }
    }

    private void UpdateDrag()
    {
        var fixedHeight = Mathf.Clamp(transform.position.y, 0, ZeroDragHeight);
        var multiplier = Math.Abs(fixedHeight) < float.Epsilon ? 1 : 1 - fixedHeight / ZeroDragHeight;
        RocketRigidbody.drag = _initialDrag * multiplier;
        RocketRigidbody.angularDrag = _initialAngularDrag * multiplier;
    }

    private void ApplyThrustForce()
    {
        var force = MaxThrustForce * ThrustPercentage / 100;
        var forceRelativeToRotation = CenterOfThrust.up * force;

        RocketRigidbody.AddForceAtPosition(forceRelativeToRotation, CenterOfThrust.position);
    }

    private void ApplyPayloadDragForce()
    {
        var attackDir = transform.up - RocketRigidbody.velocity.normalized;
        var powSpeed = Mathf.Pow(RocketRigidbody.velocity.magnitude, 2);
        var dragForce = RocketRigidbody.drag * PayloadDrag * attackDir * powSpeed;
        RocketRigidbody.AddForceAtPosition(dragForce, Payload.position);
    }
}
