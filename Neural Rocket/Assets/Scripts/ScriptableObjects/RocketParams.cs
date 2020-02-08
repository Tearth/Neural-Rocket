using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RocketParams", order = 2)]
public class RocketParams : ScriptableObject
{
    public float DryMass;
    public float MaxThrustForce;
    public float MaxGimbal;
    public float PayloadDrag;
    public float FuelUsage;
    public float MaxOperationalAltitude;
    public float MaxOperationalAcceleration;
}