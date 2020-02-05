using UnityEngine;

public class MouseOrbit : MonoBehaviour
{
    public Transform Target;
    public float Distance;
    public float XSpeed;
    public float YSpeed;
    public float ScrollSpeed;

    public float YMinLimit;
    public float YMaxLimit;

    public float DistanceMin;
    public float DistanceMax;

    private Rigidbody _rigidbody;
    private Vector2 _lastMousePosition;
    private float _x;
    private float _y;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _x = transform.eulerAngles.y;
        _y = transform.eulerAngles.x;

        if (_rigidbody != null)
        {
            _rigidbody.freezeRotation = true;
        }
    }

    private void LateUpdate()
    {
        if (Target != null)
        {
            if (Input.GetMouseButton(1))
            {
                Cursor.visible = false;

                _x += Input.GetAxis("Mouse X") * XSpeed * Distance * 0.02f;
                _y -= Input.GetAxis("Mouse Y") * YSpeed * 0.02f;
                _y = ClampAngle(_y, YMinLimit, YMaxLimit);
            }
            else
            {
                Cursor.visible = true;
            }

            var scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            Distance = Mathf.Clamp(Distance - scrollDelta * ScrollSpeed, DistanceMin, DistanceMax);

            var rotation = Quaternion.Euler(_y, _x, 0);
            var negDistance = new Vector3(0.0f, 0.0f, -Distance);
            var position = rotation * negDistance + Target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
        {
            angle += 360F;
        }

        if (angle > 360F)
        {
            angle -= 360F;
        }

        return Mathf.Clamp(angle, min, max);
    }
}