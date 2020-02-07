using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WorldParams", order = 1)]
public class WorldParams : ScriptableObject
{
    public float ZeroDragAltitude;
    public float OrbitalSpeed;
    public Vector3 Gravity => Physics.gravity;
}
