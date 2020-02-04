using UnityEngine;

public class AccelerationMeter : MonoBehaviour
{
    public Rigidbody Target;
    public Vector3 Acceleration = Vector3.zero;
    public Vector3 AccelerationWithGravity => Acceleration - Physics.gravity;
    public float GForce => AccelerationWithGravity.magnitude / Physics.gravity.magnitude;

    private Vector3 _lastVelocity;

    private void Start()
    {
        _lastVelocity = Target.velocity;
    }

    void FixedUpdate()
    {
        Acceleration = (Target.velocity - _lastVelocity) / Time.fixedDeltaTime;
        _lastVelocity = Target.velocity;
    }
}
