using System;
using UnityEngine;

public class CameraEntity : MonoBehaviour
{
    [Header("General")]
    public Camera Camera;

    [Header("Sky")]
    public Color BaseSkyColor;
    public float BlackSkyHeight;

    private void Update()
    {
        UpdateSkyColor();
    }

    private void UpdateSkyColor()
    {
        var fixedHeight = Mathf.Clamp(transform.position.y, 0, BlackSkyHeight);
        var multiplier = Math.Abs(fixedHeight) < float.Epsilon ? 1 : 1 - fixedHeight / BlackSkyHeight;

        Camera.backgroundColor = BaseSkyColor * multiplier;
    }
}
