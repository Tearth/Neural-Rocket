using UnityEngine;

public class LaunchComplexesManager : MonoBehaviour
{
    [Header("General")]
    public GameObject LaunchComplex;
    public int AgentsCount;
    public float SpaceBetweenAgents;
    public int AgentsInRow;

    private void Start()
    {
        GenerateLaunchComplexClones();
    }

    private void GenerateLaunchComplexClones()
    {
        for (var i = 1; i < AgentsCount; i++)
        {
            var absolutePosition = new Vector3(
                (i % AgentsInRow) * SpaceBetweenAgents,
                0,
                (i / AgentsInRow) * SpaceBetweenAgents);
            var relativePosition = LaunchComplex.transform.position + absolutePosition;

            var launchComplex = Instantiate(LaunchComplex, relativePosition, Quaternion.identity, transform);
            launchComplex.name = $"Launch Complex {i}";
        }
    }
}
