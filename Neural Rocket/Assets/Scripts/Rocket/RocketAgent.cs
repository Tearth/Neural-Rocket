﻿using System;
using MLAgents;
using UnityEngine;
using Random = UnityEngine.Random;

public class RocketAgent : Agent
{
    public RocketEntity RocketEntity;
    public float TargetAltitude;
    public float TargetAltitudeTolerance;
    public float TargetHorizontalSpeedTolerance;
    public float TargetVerticalSpeedTolerance;
    public float AngleOfAttackTolerance;
    public float MaxAngleOfAttack;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private float _initialMass;

    public override void InitializeAgent()
    {
        TargetAltitude = Random.Range(RocketEntity.WorldParams.ZeroDragAltitude + TargetAltitudeTolerance, RocketEntity.RocketParams.MaxOperationalAltitude - TargetAltitudeTolerance);
        _initialPosition = RocketEntity.transform.position;
        _initialRotation = RocketEntity.transform.rotation;
        _initialMass = RocketEntity.RocketRigidbody.mass;
    }

    public override void CollectObservations()
    {
        /*
         * I0 - altitude - from 0 to MaxOperationalAltitude
         * I1 - target altitude - from 0 to MaxOperationalAltitude
         * I2 - x speed - from -OrbitalSpeed * 2 to OrbitalSpeed * 2
         * I3 - y speed - from -OrbitalSpeed to OrbitalSpeed
         * I4 - z rotation - from -180 to 180 degrees
         * I5 - angle of attack - from -90 to 90 degrees
         * I6 - z rotation speed - from -PI/2 to PI/2
         */

        // I0
        var altitudeNormalized = NormalizeValue(RocketEntity.transform.position.y, 0, RocketEntity.RocketParams.MaxOperationalAltitude);
        AddVectorObs(altitudeNormalized);

        // I1
        var targetAltitudeNormalized = NormalizeValue(TargetAltitude, 0, RocketEntity.RocketParams.MaxOperationalAltitude);
        AddVectorObs(targetAltitudeNormalized);

        // I2
        var xSpeedNormalized = NormalizeValue(RocketEntity.RocketRigidbody.velocity.x, -RocketEntity.WorldParams.OrbitalSpeed * 2, RocketEntity.WorldParams.OrbitalSpeed * 2);
        AddVectorObs(xSpeedNormalized);

        // I3
        var ySpeedNormalized = NormalizeValue(RocketEntity.RocketRigidbody.velocity.y, -RocketEntity.WorldParams.OrbitalSpeed, RocketEntity.WorldParams.OrbitalSpeed);
        AddVectorObs(ySpeedNormalized);

        // I4
        var rotation = RocketEntity.RocketRigidbody.rotation.eulerAngles;
        var normalizedRotation = new Vector3(
            rotation.x < 180 ? rotation.x : rotation.x - 360,
            rotation.y < 180 ? rotation.y : rotation.y - 360,
            rotation.z < 180 ? rotation.z : rotation.z - 360
        );
        var zRotationNormalized = NormalizeValue(normalizedRotation.z, -180, 180);
        AddVectorObs(zRotationNormalized);

        // I5
        var zRotationSpeedNormalized = NormalizeValue(RocketEntity.RocketRigidbody.angularVelocity.z, (float)-Math.PI / 2, (float)Math.PI / 2);
        AddVectorObs(zRotationSpeedNormalized);

        // I6
        var angleOfAttackNormalized = NormalizeValue(RocketEntity.AngleOfAttack, -90, 90);
        AddVectorObs(angleOfAttackNormalized);

        /*Debug.Log($"Alt: {altitudeNormalized:0.000}, " +
                  $"TAlt: {targetAltitudeNormalized:0.000}, " +
                  $"XSpeed: {xSpeedNormalized:0.000}, " +
                  $"YSpeed: {ySpeedNormalized:0.000}, " +
                  $"ZRot: {zRotationNormalized:0.000}, " +
                  $"ZRotSpeed: {zRotationSpeedNormalized:0.000}, " +
                  $"AOA: {angleOfAttackNormalized:0.000}");*/
    }

    public override void AgentAction(float[] vectorAction)
    {
        var fixedAngleOfAttack = RocketEntity.RocketRigidbody.velocity.magnitude > 5 ? RocketEntity.AngleOfAttack : 0;

        var gimbalResponse = RocketEntity.RocketParams.MaxGimbal * vectorAction[0];
        var thrustResponse = (vectorAction[1] + 1) * 50;

        var rotation = RocketEntity.RocketRigidbody.rotation.eulerAngles;
        var fixedRotationZ = rotation.z < 180 ? rotation.z : rotation.z - 360;

        RocketEntity.SetGimbal(vectorAction[0]);
        RocketEntity.SetThrust(vectorAction[1]);

        // If rocket is below half target altitude
        if (RocketEntity.transform.position.y < TargetAltitude - TargetAltitudeTolerance)
        {
            var angleRange = 20f;
            var altitudeAngleRatio = Mathf.Sqrt(Mathf.Clamp(RocketEntity.transform.position.y / TargetAltitude, 0, 1 - angleRange / 90));
            var angleFrom = -90 * altitudeAngleRatio;
            var angleTo = angleFrom - angleRange;

            // Reward agent if rocket's rotation is within desired angle
            if (fixedRotationZ < angleFrom && fixedRotationZ > angleTo)
            {
                if (thrustResponse >= 50)
                {
                    AddReward(0.3f);
                }
            }

            // Punish agent if he doesn't try to fix its rotation using gimbal
            if (fixedRotationZ > angleFrom && gimbalResponse <= 0 || fixedRotationZ < angleTo && gimbalResponse >= 0)
            {
                AddReward(-0.2f);
            }
        }
        // If rocket is within desired altitude range
        else if (RocketEntity.transform.position.y >= TargetAltitude - TargetAltitudeTolerance &&
                 RocketEntity.transform.position.y <= TargetAltitude + TargetAltitudeTolerance)
        {
            // If rocket's speed is lower than desired
            if (RocketEntity.RocketRigidbody.velocity.x < RocketEntity.WorldParams.OrbitalSpeed - TargetHorizontalSpeedTolerance)
            {
                var angleRange = 10f;
                var angleFrom = -90 + angleRange;
                var angleTo = -90 - angleRange;

                // If rocket is climbing faster than vertical speed tolerance
                if (RocketEntity.RocketRigidbody.velocity.y > TargetVerticalSpeedTolerance)
                {
                    if (fixedRotationZ < angleTo && gimbalResponse > 0 || fixedRotationZ > angleTo && gimbalResponse < 0)
                    {
                        AddReward(-0.3f);
                    }
                }
                // If rocket is falling faster than vertical speed tolerance
                else if (RocketEntity.RocketRigidbody.velocity.y < -TargetVerticalSpeedTolerance)
                {
                    if (fixedRotationZ < angleFrom && gimbalResponse > 0 || fixedRotationZ > angleFrom && gimbalResponse < 0)
                    {
                        AddReward(-0.3f);
                    }
                }
                // If rocket is within desired vertical speed range
                else
                {
                    // Reward agent if thrust is throttled
                    if (thrustResponse >= 25)
                    {
                        AddReward(0.4f);
                    }
                }
            }
            // If rocket is within desired speed range
            else if (RocketEntity.RocketRigidbody.velocity.x >= RocketEntity.WorldParams.OrbitalSpeed - TargetHorizontalSpeedTolerance &&
                     RocketEntity.RocketRigidbody.velocity.x <= RocketEntity.WorldParams.OrbitalSpeed + TargetHorizontalSpeedTolerance)
            {
                // Reward agent if he stopped engine (because we don't need more speed)
                if (thrustResponse <= 10)
                {
                    AddReward(0.6f);
                    Done();
                }
                else
                {
                    AddReward(-0.6f);
                }
            }
        }
        // End episode if rocket is too high
        else
        {
            Done();
        }

        if (RocketEntity.transform.position.y <= RocketEntity.WorldParams.ZeroDragAltitude && Mathf.Abs(RocketEntity.AngleOfAttack) > AngleOfAttackTolerance)
        {
            // Punish agent if he doesn't try to decrease angle of attack
            if (RocketEntity.AngleOfAttack > 0 && gimbalResponse > 0 || RocketEntity.AngleOfAttack < 0 && gimbalResponse < 0)
            {
                AddReward(-0.2f);
            }
        }

        // Rocket is marked as destroyed, so end this episode
        if (RocketEntity.Destroyed || fixedAngleOfAttack >= MaxAngleOfAttack)
        {
            Done();
        }

        // Debug.Log($"Reward: {GetReward()}");
    }

    public override void AgentReset()
    {
        TargetAltitude = Random.Range(RocketEntity.WorldParams.ZeroDragAltitude + TargetAltitudeTolerance, RocketEntity.RocketParams.MaxOperationalAltitude - TargetAltitudeTolerance);
        RocketEntity.transform.position = _initialPosition;
        RocketEntity.transform.rotation = _initialRotation;
        RocketEntity.RocketRigidbody.velocity = Vector3.zero;
        RocketEntity.RocketRigidbody.angularVelocity = Vector3.zero;
        RocketEntity.RocketRigidbody.mass = _initialMass;
        RocketEntity.AccelerationMeter.Reset();
        RocketEntity.Destroyed = false;
    }

    public override float[] Heuristic()
    {
        var gimbalZ = 0.0f;
        var thrust = -1.0f;

        if (Input.GetKey(KeyCode.A))
        {
            gimbalZ = -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            gimbalZ = 1;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            thrust = 1;
        }

        return new[]
        {
            gimbalZ,
            thrust
        };
    }

    private float NormalizeValue(float value, float minValue, float maxValue)
    {
        var delta = maxValue - minValue;
        return (value - minValue) / delta * 2 - 1;
    }
}
