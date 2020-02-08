using UnityEngine;

public class AccelerationMeter : MonoBehaviour
{
    [Header("General")]
    public Rigidbody Target;

    public Vector3 Acceleration { get; internal set; }
    public Vector3 AccelerationWithGravity => Acceleration - Physics.gravity;
    public float GForce => AccelerationWithGravity.magnitude / Physics.gravity.magnitude;

    private Vector3 _lastVelocity;

    private void Start()
    {
        _lastVelocity = Target.velocity;
    }

    private void FixedUpdate()
    {
        UpdateAcceleration();
    }

    private void UpdateAcceleration()
    {
        Acceleration = (Target.velocity - _lastVelocity) / Time.fixedDeltaTime;
        _lastVelocity = Target.velocity;
    }

    public void Reset()
    {
        _lastVelocity = Target.velocity;
    }
}
