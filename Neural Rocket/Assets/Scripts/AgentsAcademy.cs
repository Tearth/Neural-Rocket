using MLAgents;
using UnityEngine;

public class AgentsAcademy : Academy
{
    [Header("General")]
    public GameObject RocketTarget;
    public GameObject Launchpad;
    public int AgentsCount;
    public float SpaceBetweenAgents;
    public int AgentsInRow;

    private void Start()
    {
        for (var i = 1; i < AgentsCount; i++)
        {
            Instantiate(RocketTarget, RocketTarget.transform.position + new Vector3((i % AgentsInRow) * SpaceBetweenAgents, 0, (i / AgentsInRow) * SpaceBetweenAgents), Quaternion.identity, transform);
            Instantiate(Launchpad, Launchpad.transform.position + new Vector3((i % AgentsInRow) * SpaceBetweenAgents, 0, (i / AgentsInRow) * SpaceBetweenAgents), Quaternion.identity, transform);
        }
    }
}
