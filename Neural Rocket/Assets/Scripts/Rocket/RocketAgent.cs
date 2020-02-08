using MLAgents;
using UnityEngine;

public class RocketAgent : Agent
{
    public RocketEntity RocketEntity;
    public float TargetAltitude;
    public float TargetAltitudeTolerance;
    public float TargetHorizontalSpeedTolerance;
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
         * I5 - x rotation - from -180 to 180 degrees
         * I6 - angle of attack - from -90 to 90 degrees
         * I7 - x acceleration - from -MaxOperationalAcceleration to MaxOperationalAcceleration
         * I8 - y acceleration - from -MaxOperationalAcceleration to MaxOperationalAcceleration
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
        var xRotationNormalized = NormalizeValue(normalizedRotation.z, -180, 180);
        AddVectorObs(xRotationNormalized);

        // I6
        var angleOfAttackNormalized = NormalizeValue(RocketEntity.AngleOfAttack, -90, 90);
        AddVectorObs(angleOfAttackNormalized);

        // I7
        var xAccelerationNormalized = NormalizeValue(RocketEntity.AccelerationMeter.Acceleration.x, -RocketEntity.RocketParams.MaxOperationalAcceleration, RocketEntity.RocketParams.MaxOperationalAcceleration);
        AddVectorObs(xAccelerationNormalized);

        // I8
        var yAccelerationNormalized = NormalizeValue(RocketEntity.AccelerationMeter.Acceleration.y, -RocketEntity.RocketParams.MaxOperationalAcceleration, RocketEntity.RocketParams.MaxOperationalAcceleration);
        AddVectorObs(yAccelerationNormalized);

        /*
        Debug.Log($"Altitude: {altitudeNormalized}, " +
                  $"TAltitude: {targetAltitudeNormalized}, " +
                  $"XSpeed: {xSpeedNormalized}, " +
                  $"XSpeedTarget: {xSpeedTargetNormalized}, " +
                  $"YSpeed: {ySpeedNormalized}, " +
                  $"XRotation: {xRotationNormalized}, " +
                  $"AngleOfAttack: {angleOfAttackNormalized}, " +
                  $"XAcceleration: {xAccelerationNormalized}, " +
                  $"YAcceleration: {yAccelerationNormalized}");*/
    }

    public override void AgentAction(float[] vectorAction)
    {
        var altitudeIsValid = false;
        var horizontalSpeedIsValid = false;

        RocketEntity.SetGimbal(vectorAction[0], 0);
        RocketEntity.SetThrust(vectorAction[1]);

        // If rocket is below target altitude, then reward only if rocket is climbing
        if (RocketEntity.transform.position.y < TargetAltitude - TargetAltitudeTolerance)
        {
            if (RocketEntity.RocketRigidbody.velocity.y > 0)
            {
                // Base reward
                AddReward(0.1f);

                // Altitude reward
                AddReward(0.2f * (RocketEntity.transform.position.y / TargetAltitude));
            }
            else
            {
                AddReward(-0.2f);
            }
        }
        // If rocket is at the target altitude, then reward agent
        else if (RocketEntity.transform.position.y >= TargetAltitude - TargetAltitudeTolerance &&
                 RocketEntity.transform.position.y <= TargetAltitude + TargetAltitudeTolerance)
        {
            altitudeIsValid = true;
            AddReward(0.5f);
        }
        // If rocket is above the target altitude, then punish agent
        else
        {
            AddReward(-0.2f);
        }

        // If angle of attack is smaller than tolerance, then reward agent
        if (Mathf.Abs(RocketEntity.AngleOfAttack) <= AngleOfAttackTolerance)
        {
            AddReward(0.1f);
        }
        // If angle of attack is larger than tolerance, then punish agent
        else
        {
            AddReward(-0.3f);
        }

        // If rocket's horizontal speed is smaller than target, then reward only if rocket is accelerating in the valid direction
        if (RocketEntity.RocketRigidbody.velocity.x < RocketEntity.WorldParams.OrbitalSpeed - TargetHorizontalSpeedTolerance)
        {
            if (RocketEntity.AccelerationMeter.Acceleration.x > 0)
            {
                // Base reward
                AddReward(0.1f);

                // Speed reward
                AddReward(0.2f * (RocketEntity.RocketRigidbody.velocity.x / RocketEntity.WorldParams.OrbitalSpeed));
            }
            else
            {
                AddReward(-0.2f);
            }
        }
        // If rocket's horizontal speed is at target, then reward agent
        else if (RocketEntity.RocketRigidbody.velocity.x >= RocketEntity.WorldParams.OrbitalSpeed - TargetHorizontalSpeedTolerance &&
                 RocketEntity.RocketRigidbody.velocity.x <= RocketEntity.WorldParams.OrbitalSpeed + TargetHorizontalSpeedTolerance)
        {
            horizontalSpeedIsValid = true;
            AddReward(0.5f);
        }
        // If rocket is going too fast, then punish agent
        else
        {
            AddReward(-0.2f);
        }

        // If rocket reached the specified altitude and speed, then mission is completed
        if (altitudeIsValid && horizontalSpeedIsValid)
        {
            AddReward(1);
            Done();
        }

        // If rocket is destroyed, then stop whole episode
        if (Mathf.Abs(RocketEntity.AngleOfAttack) >= MaxAngleOfAttack || RocketEntity.Destroyed)
        {
            SetReward(-1);
            Done();
        }
    }

    public override void AgentReset()
    {
        TargetAltitude = Random.Range(RocketEntity.WorldParams.ZeroDragAltitude, RocketEntity.RocketParams.MaxOperationalAltitude);
        RocketEntity.transform.position = _initialPosition;
        RocketEntity.transform.rotation = _initialRotation;
        RocketEntity.RocketRigidbody.velocity = Vector3.zero;
        RocketEntity.RocketRigidbody.angularVelocity = Vector3.zero;
        RocketEntity.RocketRigidbody.mass = _initialMass;
        RocketEntity.Destroyed = false;
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
}
