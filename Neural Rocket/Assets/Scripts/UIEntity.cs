using System;
using UnityEngine;
using UnityEngine.UI;

public class UIEntity : MonoBehaviour
{
    [Header("General")]
    public RocketEntity CurrentRocket;

    [Header("GUI elements")]
    public Text AltitudeText;
    public Text SpeedText;
    public Text AccelerationText;
    public Text ThrustText;
    public Text GForceText;

    [Header("Update settings")]
    public float UpdateInterval;

    private const string AltitudePattern = "Altitude: {0} m";
    private const string SpeedPattern = "Speed: {0} m/s";
    private const string AccelerationPattern = "Acceler: {0} m/s^2";
    private const string ThrustPattern = "Thrust: {0} %";
    private const string GForcePattern = "G-force: {0} G";

    private DateTime _lastUpdateTime;

    private void Start()
    {
        _lastUpdateTime = DateTime.MinValue;
    }

    private void Update()
    {
        if ((DateTime.Now - _lastUpdateTime).TotalSeconds >= UpdateInterval)
        {
            UpdateRocketInfo();
            _lastUpdateTime = DateTime.Now;
        }
    }

    private void UpdateRocketInfo()
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
            var altitude = CurrentRocket.CenterOfThrust.transform.position.y.ToString("0.0");
            var speed = CurrentRocket.RocketRigidbody.velocity.magnitude.ToString("0.0");
            var acceleration = CurrentRocket.AccelerationMeter.Acceleration.magnitude.ToString("0.0");
            var gForce = CurrentRocket.AccelerationMeter.GForce.ToString("0.0");

            AltitudeText.text = string.Format(AltitudePattern, altitude);
            SpeedText.text = string.Format(SpeedPattern, speed);
            AccelerationText.text = string.Format(AccelerationPattern, acceleration);
            ThrustText.text = string.Format(ThrustPattern, 100);
            GForceText.text = string.Format(GForcePattern, gForce);
        }
    }
}
