using MLAgents;
using UnityEngine;

public class RocketAgent : Agent
{
    public RocketEntity RocketEntity;
    public float TargetHeight;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private float _initialMass;

    private float _maxHeight;
    
    public override void InitializeAgent()
    {
        TargetHeight = Random.Range(RocketEntity.WorldParams.ZeroDragAltitude, RocketEntity.RocketParams.MaxOperationalAltitude);
        _initialPosition = RocketEntity.transform.position;
        _initialRotation = RocketEntity.transform.rotation;
        _initialMass = RocketEntity.RocketRigidbody.mass;
        _maxHeight = RocketEntity.transform.position.y;
    }

    public override void CollectObservations()
    {
        var altitudeNormalized = NormalizeValue(RocketEntity.transform.position.y, 0, RocketEntity.RocketParams.MaxOperationalAltitude);
        AddVectorObs(altitudeNormalized);

        var rotation = RocketEntity.RocketRigidbody.rotation.eulerAngles;
        var normalizedRotation = new Vector3(
            rotation.x < 180 ? rotation.x : rotation.x - 360,
            rotation.y < 180 ? rotation.y : rotation.y - 360,
            rotation.z < 180 ? rotation.z : rotation.z - 360
        );
        var xRotationNormalized = NormalizeValue(normalizedRotation.z, -180, 180);
        AddVectorObs(xRotationNormalized);

        var angleOfAttackNormalized = NormalizeValue(RocketEntity.AngleOfAttack, -90, 90);
        AddVectorObs(angleOfAttackNormalized);

        // Debug.Log($"Altitude: {altitudeNormalized}, XRotation: {xRotationNormalized}, AOANormalized: {angleOfAttackNormalized}");
    }

    public override void AgentAction(float[] vectorAction)
    {
        RocketEntity.SetGimbal(vectorAction[0], 0);
        RocketEntity.SetThrust(vectorAction[1]);

        _maxHeight = Mathf.Max(_maxHeight, RocketEntity.transform.position.y);

        var heightReward = Mathf.Clamp((_maxHeight - 5000) / 5000f, -1, 1);

        // Rocket is on orbit
        // if (RocketEntity.transform.position.y >= RocketEntity.TargetAltitude)
        {
            if (RocketEntity.FuelPercentage <= 0)
            {
                var speedReward = RocketEntity.RocketRigidbody.velocity.x / RocketEntity.WorldParams.OrbitalSpeed;
                SetReward(heightReward + speedReward);
                Done();
            }
        }

        // Rocket is destroyed
        if (Mathf.Abs(RocketEntity.AngleOfAttack) >= 90 || RocketEntity.RocketRigidbody.velocity.y < -5 || RocketEntity.Destroyed)
        {
            SetReward(heightReward);
            Done();
        }
    }

    public override void AgentReset()
    {
        TargetHeight = Random.Range(RocketEntity.WorldParams.ZeroDragAltitude, RocketEntity.RocketParams.MaxOperationalAltitude);
        RocketEntity.transform.position = _initialPosition;
        RocketEntity.transform.rotation = _initialRotation;
        RocketEntity.RocketRigidbody.velocity = Vector3.zero;
        RocketEntity.RocketRigidbody.angularVelocity = Vector3.zero;
        RocketEntity.RocketRigidbody.mass = _initialMass;
        RocketEntity.Destroyed = false;

        _maxHeight = RocketEntity.transform.position.y;
    }

    public override float[] Heuristic()
    {
        var gimbalX = 0.0f;
        var thrust = -1.0f;

        if (Input.GetKey(KeyCode.A))
        {
            gimbalX = -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            gimbalX = 1;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            thrust = 1;
        }

        return new []
        {
            gimbalX,
            thrust
        };
    }

    private float NormalizeValue(float value, float minValue, float maxValue)
    {
        var delta = maxValue - minValue;
        return (value - minValue) / delta * 2 - 1;
    }

    private Vector3 NormalizeVector(Vector3 vector, float minValue, float maxValue)
    {
        var delta = maxValue - minValue;
        return new Vector3(
            (vector.x - minValue) / delta,
            (vector.y - minValue) / delta,
            (vector.z - minValue) / delta
        );
    }
}
