using System;
using UnityEngine;

public class CameraEntity : MonoBehaviour
{
    [Header("General")]
    public Camera Camera;
    public WorldParams WorldParams;

    [Header("Sky")]
    public Color BaseSkyColor;

    private void Update()
    {
        UpdateSkyColor();
    }

    private void UpdateSkyColor()
    {
        var fixedHeight = Mathf.Clamp(transform.position.y, 0, WorldParams.ZeroDragAltitude);
        var multiplier = Math.Abs(fixedHeight) < float.Epsilon ? 1 : 1 - fixedHeight / WorldParams.ZeroDragAltitude;

        Camera.backgroundColor = BaseSkyColor * multiplier;
    }
}
