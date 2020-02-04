using UnityEngine;

public class RocketEntity : MonoBehaviour
{
    [Header("General")]
    public Rigidbody RocketRigidbody;
    public AccelerationMeter AccelerationMeter;
    public Transform CenterOfThrust;

    [Header("Control")]
    public float Thrust;
    public float MaxGimbal;
}
