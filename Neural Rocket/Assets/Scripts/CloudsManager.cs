using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
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
        
    }

    private void OnValidate()
    {
        RegenerateAllClouds();
    }

    public void CreateCloud()
    {
        var cloud = Instantiate(CloudPrefab, RandomCloudPosition(), Quaternion.identity, transform);
        cloud.name = $"Cloud {transform.childCount}";
    }

    public void RegenerateCloud(GameObject cloud)
    {
        cloud.transform.position = RandomCloudPosition();
    }

    public void RegenerateAllClouds()
    {
        foreach (Transform cloud in transform)
        {
            RegenerateCloud(cloud.gameObject);
        }
    }

    private Vector3 RandomCloudPosition()
    {
        var positionWithinSphere = Random.insideUnitSphere * GenerationRadius;
        return new Vector3(
            positionWithinSphere.x,
            CloudsHeight,
            positionWithinSphere.z
        );
    }
}
