using System;
using MLAgents;
using UnityEngine;

public class RocketAgent : Agent
{
    public RocketEntity RocketEntity;

    public override void CollectObservations()
    {
        var velocityNormalized = NormalizeVector(RocketEntity.RocketRigidbody.velocity, 0, RocketEntity.OrbitalSpeed * 2);
        var rotationNormalized = NormalizeVector(RocketEntity.RocketRigidbody.rotation.eulerAngles, -180, 180);
        var angleOfAttackNormalized = NormalizeValue(RocketEntity.AngleOfAttack, 0, 180);
        var altitudeNormalized = NormalizeValue(RocketEntity.transform.position.y, 0, RocketEntity.TargetAltitude * 2);

        AddVectorObs(velocityNormalized);
        AddVectorObs(rotationNormalized);
        AddVectorObs(angleOfAttackNormalized);
        AddVectorObs(altitudeNormalized);

        Debug.Log($"NVelocity: {velocityNormalized}, NRotation: {rotationNormalized}, NAngleOfAttack: {angleOfAttackNormalized}, NAltitude: {altitudeNormalized}");
    }

    public override void AgentAction(float[] vectorAction)
    {
        RocketEntity.SetGimbal(vectorAction[0], vectorAction[1]);
        RocketEntity.SetThrust(vectorAction[2]);
    }

    public override float[] Heuristic()
    {
        var gimbalX = 0.0f;
        var gimbalY = 0.0f;
        var thrust = -1.0f;

        if (Input.GetKey(KeyCode.A))
        {
            gimbalX = -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            gimbalX = 1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            gimbalY = 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            gimbalY = -1;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            thrust = 1;
        }

        return new []
        {
            gimbalX,
            gimbalY,
            thrust
        };
    }

    private float NormalizeValue(float value, float minValue, float maxValue)
    {
        var delta = maxValue - minValue;
        return (value - minValue) / delta;
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
