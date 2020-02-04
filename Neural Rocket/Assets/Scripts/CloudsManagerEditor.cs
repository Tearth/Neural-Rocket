using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CloudsManager))]
class CloudsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var cloudsManager = (CloudsManager)target;
        if (GUILayout.Button("Regenerate all clouds"))
        {
            cloudsManager.RegenerateAllClouds();
        }
    }
}