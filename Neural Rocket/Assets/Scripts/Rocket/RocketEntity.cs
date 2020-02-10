using System;
using UnityEngine;

public class RocketEntity : MonoBehaviour
{
    [Header("General")]
    public Rigidbody RocketRigidbody;
    public AccelerationMeter AccelerationMeter;
    public Transform CenterOfThrust;
    public Transform Payload;
    public WorldParams WorldParams;
    public RocketParams RocketParams;
    public bool Destroyed;

    [Header("Control")]
    public float ThrustPercentage;

    public float FuelPercentage => (RocketRigidbody.mass - RocketParams.DryMass) / (_initialMass - RocketParams.DryMass) * 100;
    public float AngleOfAttack => Vector3.SignedAngle(transform.up, RocketRigidbody.velocity.normalized, Vector3.forward);
    
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
        UpdateMass();
        UpdateDrag();
        ApplyThrustForce();
        ApplyPayloadDragForce();
        ApplyAntiGravityForce();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Ground")
        {
            Destroyed = true;
        }
    }

    private void UpdateMass()
    {
        if (FuelPercentage > 0)
        {
            RocketRigidbody.mass -= RocketParams.FuelUsage * ThrustPercentage / 100;
        }
    }

    private void UpdateDrag()
    {
        var fixedAltitude = Mathf.Clamp(transform.position.y, 0, WorldParams.ZeroDragAltitude);
        var multiplier = Math.Abs(fixedAltitude) < float.Epsilon ? 1 : 1 - fixedAltitude / WorldParams.ZeroDragAltitude;
        RocketRigidbody.drag = _initialDrag * multiplier;
        RocketRigidbody.angularDrag = _initialAngularDrag * multiplier;
    }

    private void ApplyThrustForce()
    {
        var force = RocketParams.MaxThrustForce * ThrustPercentage / 100;
        var forceRelativeToRotation = CenterOfThrust.up * force;

        RocketRigidbody.AddForceAtPosition(forceRelativeToRotation, CenterOfThrust.position);
    }

    private void ApplyPayloadDragForce()
    {
        var attackDir = transform.up - RocketRigidbody.velocity.normalized;
        var powSpeed = Mathf.Pow(RocketRigidbody.velocity.magnitude, 2);
        var dragForce = RocketRigidbody.drag * RocketParams.PayloadDrag * attackDir * powSpeed;
        RocketRigidbody.AddForceAtPosition(dragForce, Payload.position);
    }

    private void ApplyAntiGravityForce()
    {
        var velocityWithoutVerticalPart = new Vector3(
            RocketRigidbody.velocity.x,
            0,
            RocketRigidbody.velocity.z
        );

        var fixedVelocity = Mathf.Clamp(velocityWithoutVerticalPart.magnitude, 0, WorldParams.OrbitalSpeed);
        var multiplier = fixedVelocity / WorldParams.OrbitalSpeed;
        var antiGravityForce = -WorldParams.Gravity * multiplier;

        RocketRigidbody.AddForce(antiGravityForce, ForceMode.Acceleration);
    }

    public void SetGimbal(float z)
    {
        var rotationZ = RocketParams.MaxGimbal * z;

        CenterOfThrust.localEulerAngles = new Vector3(
            CenterOfThrust.localEulerAngles.x,
            CenterOfThrust.localEulerAngles.y,
            rotationZ);
    }

    public void SetThrust(float thrust)
    {
        if (FuelPercentage > 0)
        {
            var percentage = (thrust + 1) * 50;
            if (percentage >= RocketParams.MinThrustPercentage)
            {
                ThrustPercentage = percentage;
            }
            else
            {
                ThrustPercentage = 0;
            }
        }
        else
        {
            ThrustPercentage = 0;
        }
    }
}
