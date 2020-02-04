using UnityEngine;

public class GroundEntity : MonoBehaviour
{
    [Header("General")]
    public Camera MainCamera;

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        transform.position = new Vector3(
            MainCamera.transform.position.x,
            transform.position.y,
            MainCamera.transform.position.z
        );
    }
}
