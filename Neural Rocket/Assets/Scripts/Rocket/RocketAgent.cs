using System;
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
        TargetAltitude = Random.Range(RocketEntity.WorldParams.ZeroDragAltitude, RocketEntity.RocketParams.MaxOperationalAltitude);
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
         * I3 - target x speed - from -OrbitalSpeed * 2 to OrbitalSpeed * 2
         * I4 - y speed - from -OrbitalSpeed to OrbitalSpeed
         * I5 - z rotation - from -180 to 180 degrees
         * I6 - angle of attack - from -90 to 90 degrees
         * I7 - z rotation speed - from -PI/2 to PI/2
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
        var xSpeedTargetNormalized = NormalizeValue(RocketEntity.WorldParams.OrbitalSpeed, -RocketEntity.WorldParams.OrbitalSpeed * 2, RocketEntity.WorldParams.OrbitalSpeed * 2);
        AddVectorObs(xSpeedTargetNormalized);

        // I4
        var ySpeedNormalized = NormalizeValue(RocketEntity.RocketRigidbody.velocity.y, -RocketEntity.WorldParams.OrbitalSpeed, RocketEntity.WorldParams.OrbitalSpeed);
        AddVectorObs(ySpeedNormalized);

        // I5
        var rotation = RocketEntity.RocketRigidbody.rotation.eulerAngles;
        var normalizedRotation = new Vector3(
            rotation.x < 180 ? rotation.x : rotation.x - 360,
            rotation.y < 180 ? rotation.y : rotation.y - 360,
            rotation.z < 180 ? rotation.z : rotation.z - 360
        );
        var zRotationNormalized = NormalizeValue(normalizedRotation.z, -180, 180);
        AddVectorObs(zRotationNormalized);

        // I6
        var zRotationSpeedNormalized = NormalizeValue(RocketEntity.RocketRigidbody.angularVelocity.z, (float)-Math.PI / 2, (float)Math.PI / 2);
        AddVectorObs(zRotationSpeedNormalized);

        // I7
        var angleOfAttackNormalized = NormalizeValue(RocketEntity.AngleOfAttack, -90, 90);
        AddVectorObs(angleOfAttackNormalized);

        /*Debug.Log($"Alt: {altitudeNormalized:0.000}, " +
                  $"TAlt: {targetAltitudeNormalized:0.000}, " +
                  $"XSpeed: {xSpeedNormalized:0.000}, " +
                  $"XSpeedTarget: {xSpeedTargetNormalized:0.000}, " +
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

        // If rocket is below half target altitude, then reward only if rocket is climbing
        if (RocketEntity.transform.position.y < TargetAltitude / 2 - TargetAltitudeTolerance)
        {
            // Rocket must be rotated within I quarter
            if (fixedRotationZ < 0 && fixedRotationZ > -60)
            {
                if (thrustResponse >= 50)
                {
                    AddReward(0.2f);
                }
            }

            if (fixedRotationZ > 0 && gimbalResponse < 0 || fixedRotationZ < -60 && gimbalResponse > 0)
            {
                AddReward(-0.2f);
            }
        }
        else if (RocketEntity.transform.position.y >= TargetAltitude / 2 - TargetAltitudeTolerance &&
                 RocketEntity.transform.position.y < TargetAltitude - TargetAltitudeTolerance)
        {
            // Rocket must be rotated within I quarter
            if (fixedRotationZ < -60 && fixedRotationZ > -90)
            {
                if (thrustResponse >= 25)
                {
                    AddReward(0.2f);
                }
            }

            if (fixedRotationZ > -60 && gimbalResponse < 0 || fixedRotationZ < -90 && gimbalResponse > 0)
            {
                AddReward(-0.2f);
            }
        }
        else if (RocketEntity.transform.position.y >= TargetAltitude - TargetAltitudeTolerance &&
                 RocketEntity.transform.position.y <= TargetAltitude + TargetAltitudeTolerance)
        {
            if (RocketEntity.RocketRigidbody.velocity.x >= RocketEntity.WorldParams.OrbitalSpeed - TargetHorizontalSpeedTolerance &&
                RocketEntity.RocketRigidbody.velocity.x <= RocketEntity.WorldParams.OrbitalSpeed + TargetHorizontalSpeedTolerance)
            {
                if (thrustResponse <= 10)
                {
                    AddReward(0.6f);
                }
            }
            else
            {
                if (RocketEntity.RocketRigidbody.velocity.y > TargetVerticalSpeedTolerance)
                {
                    if (fixedRotationZ < -120 && gimbalResponse > 0 || fixedRotationZ > -120 && gimbalResponse < 0)
                    {
                        AddReward(-0.3f);
                    }
                }
                else if (RocketEntity.RocketRigidbody.velocity.y < -TargetVerticalSpeedTolerance)
                {
                    if (fixedRotationZ < -60 && gimbalResponse > 0 || fixedRotationZ > -60 && gimbalResponse < 0)
                    {
                        AddReward(-0.3f);
                    }
                }
                else
                {
                    if (thrustResponse >= 25)
                    {
                        AddReward(0.4f);
                    }
                }
            }
        }
        else
        {
            Done();
        }

        // Reward if agent is trying to fix its rotation
        if (Mathf.Abs(RocketEntity.AngleOfAttack) > AngleOfAttackTolerance)
        {
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
        TargetAltitude = Random.Range(RocketEntity.WorldParams.ZeroDragAltitude, RocketEntity.RocketParams.MaxOperationalAltitude);
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

        return new []
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
