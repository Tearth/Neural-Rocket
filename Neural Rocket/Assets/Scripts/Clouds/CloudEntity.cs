using UnityEngine;

public class CloudEntity : MonoBehaviour
{
    [Header("General")]
    public Vector3 Speed;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position += Speed;
    }
}
