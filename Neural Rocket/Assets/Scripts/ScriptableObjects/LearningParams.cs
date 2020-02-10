using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LearningParams", order = 3)]
public class LearningParams : ScriptableObject
{
    public float TargetAltitudeTolerance;
    public float TargetHorizontalSpeedTolerance;
    public float TargetVerticalSpeedTolerance;
    public float AngleOfAttackTolerance;
    public float MaxAngleOfAttack;
    public float AngleRangeDuringAscending;
    public float AngleRangeDuringStabilizing;
    public int MillisecondsOnOrbitToFinish;
}