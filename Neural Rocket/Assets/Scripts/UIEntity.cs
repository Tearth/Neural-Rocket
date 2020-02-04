using System;
using UnityEngine;
using UnityEngine.UI;

public class UIEntity : MonoBehaviour
{
    public GameObject CurrentRocket;
    public Text AltitudeText;
    public Text SpeedText;
    public Text AccelerationText;
    public Text ThrustText;
    public Text GForceText;
    public float UpdateInterval;

    private const string AltitudePattern = "Altitude: {0} m";
    private const string SpeedPattern = "Speed: {0} m/s";
    private const string AccelerationPattern = "Acceleration: {0} m/s^2";
    private const string ThrustPattern = "Thrust: {0} m";
    private const string GForcePattern = "G-force: {0} G";

    private DateTime _lastUpdateTime;

    void Start()
    {
        _lastUpdateTime = DateTime.MinValue;
    }

    void Update()
    {
        if ((DateTime.Now - _lastUpdateTime).TotalSeconds >= UpdateInterval)
        {
            if (CurrentRocket == null)
            {
                AltitudeText.text = string.Format(AltitudePattern, "---");
                SpeedText.text = string.Format(SpeedPattern, "---");
                AccelerationText.text = string.Format(AccelerationPattern, "---");
                ThrustText.text = string.Format(ThrustPattern, "---");
                GForceText.text = string.Format(GForcePattern, "---");
            }
            else
            {
                var rocketRigidBody = CurrentRocket.GetComponent<Rigidbody>();
                var rocketAccelerationMeter = CurrentRocket.GetComponent<AccelerationMeter>();

                AltitudeText.text = string.Format(AltitudePattern, CurrentRocket.transform.position.y.ToString("0.0"));
                SpeedText.text = string.Format(SpeedPattern, rocketRigidBody.velocity.magnitude.ToString("0.0"));
                AccelerationText.text = string.Format(AccelerationPattern, rocketAccelerationMeter.Acceleration.magnitude.ToString("0.0"));
                ThrustText.text = string.Format(ThrustPattern, 100);
                GForceText.text = string.Format(GForcePattern, rocketAccelerationMeter.GForce.ToString("0.0"));
            }

            _lastUpdateTime = DateTime.Now;
        }
    }
}
