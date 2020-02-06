using UnityEngine;

public class CloudsManager : MonoBehaviour
{
    [Header("Camera")]
    public Camera MainCamera;
    
    [Header("Clouds")]
    public int CloudsCount;
    public float CloudsHeight;
    public float GenerationRadius;
    public GameObject CloudPrefab;

    private void Start()
    {
        while (transform.childCount < CloudsCount)
        {
            CreateCloud();
        }
    }

    private void Update()
    {
        ProcessClouds();
    }

    private void ProcessClouds()
    {
        foreach (Transform cloud in transform)
        {
            var fixedCameraPosition = new Vector3(
                MainCamera.transform.position.x,
                CloudsHeight,
                MainCamera.transform.position.z);

            if (Vector3.Distance(fixedCameraPosition, cloud.position) > GenerationRadius)
            {
                // RegenerateCloud(cloud.gameObject, false);
                RegenerateCloud(cloud.gameObject, true);
            }
        }
    }

    private void CreateCloud()
    {
        var cloud = Instantiate(CloudPrefab, RandomCloudPosition(true), Quaternion.identity, transform);
        cloud.name = $"Cloud {transform.childCount}";
    }

    private void RegenerateCloud(GameObject cloud, bool allowNear)
    {
        cloud.transform.position = RandomCloudPosition(allowNear);
    }

    private Vector3 RandomCloudPosition(bool allowNear)
    {
        var positionWithinCircle = allowNear ? Random.insideUnitCircle : Random.insideUnitCircle.normalized;
        var positionAtFixedHeight = positionWithinCircle * GenerationRadius;

        return new Vector3(
            positionAtFixedHeight.x + MainCamera.transform.position.x,
            CloudsHeight,
            positionAtFixedHeight.y + MainCamera.transform.position.z
        );
    }
}
