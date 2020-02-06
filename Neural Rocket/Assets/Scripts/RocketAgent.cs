using System;
using MLAgents;
using UnityEngine;
using Random = UnityEngine.Random;

public class RocketAgent : Agent
{
    public RocketEntity RocketEntity;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private float _initialMass;

    private float _maxHeight;
    
    public override void InitializeAgent()
    {
        _initialPosition = RocketEntity.transform.position;
        _initialRotation = RocketEntity.transform.rotation;
        _initialMass = RocketEntity.RocketRigidbody.mass;
        _maxHeight = RocketEntity.transform.position.y;
    }

    public override void CollectObservations()
    {
        // var velocityNormalized = NormalizeVector(RocketEntity.RocketRigidbody.velocity, 0, RocketEntity.OrbitalSpeed * 2);
        var test = RocketEntity.RocketRigidbody.rotation.eulerAngles;
        var test2 = new Vector3(
            test.x < 180 ? test.x : test.x - 360,
            test.y < 180 ? test.y : test.y - 360,
            test.z < 180 ? test.z : test.z - 360
            );
        var rotationNormalized = NormalizeVector(test2, -180, 180);
        //Debug.Log(rotationNormalized);
        var angleOfAttackNormalized = RocketEntity.AngleOfAttackVector;
        // Debug.Log(angleOfAttackNormalized);
        // var altitudeNormalized = NormalizeValue(RocketEntity.transform.position.y, 0, RocketEntity.TargetAltitude * 2);

        // AddVectorObs(velocityNormalized);
        AddVectorObs(rotationNormalized);
        AddVectorObs(angleOfAttackNormalized);
        // AddVectorObs(altitudeNormalized);

        // Debug.Log($"NVelocity: {velocityNormalized}, NRotation: {rotationNormalized}, NAngleOfAttack: {angleOfAttackNormalized}, NAltitude: {altitudeNormalized}");
    }

    public override void AgentAction(float[] vectorAction)
    {
        RocketEntity.SetGimbal(vectorAction[0], vectorAction[1]);
        //RocketEntity.SetThrust(vectorAction[2]);
        RocketEntity.SetThrust(1);

        _maxHeight = Mathf.Max(_maxHeight, RocketEntity.transform.position.y);

        // Longer flight = better
        //AddReward(0.1f);

        //if (RocketEntity.AccelerationMeter.Acceleration.magnitude < 1)
        //{
        //    AddReward(-1);
        //}

        // Rocket hit the ground
        var heightRes = _maxHeight / 1000f;
        if (RocketEntity.FuelPercentage <= 0)
        {
            SetReward(heightRes);
            Done();
        }

        if (RocketEntity.AngleOfAttack > 20 || RocketEntity.Destroyed)
        {
            SetReward(heightRes / 2 - 1);
            Done();
        }
    }

    public override void AgentReset()
    {
         RocketEntity.transform.position = _initialPosition;
         RocketEntity.transform.rotation = _initialRotation;
         RocketEntity.RocketRigidbody.velocity = Vector3.zero;
         RocketEntity.RocketRigidbody.angularVelocity = Vector3.zero;
         RocketEntity.RocketRigidbody.mass = _initialMass * Random.Range(0.5f, 1);
         RocketEntity.Destroyed = false;

         _maxHeight = RocketEntity.transform.position.y;
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
