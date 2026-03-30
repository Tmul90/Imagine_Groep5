using UnityEngine;
using UnityEditor;

public class ApplyVolumetricToLights : MonoBehaviour
{
    public string prefabFolderPath = "Assets/Prefabs";

    [ContextMenu("Apply VolumetricAdditionalScript To Scene + Prefabs")]
    public void ApplyMaterial()
    {
        ApplyToScene();
        ApplyToPrefabs();

        Debug.Log("Done applying physics material.");
    }

    void ApplyToScene()
    {
        Light[] lights = FindObjectsOfType<Light>(true);

        int applied = 0;

        foreach (Light l in lights)
        {
            Undo.RecordObject(l, "Apply Physics Material");

            if (l.GetComponent<VolumetricAdditionalLight>() == null)
            {
                l.gameObject.AddComponent<VolumetricAdditionalLight>();
                applied++;
            }
            
            EditorUtility.SetDirty(l);
        }

        Debug.Log($"Scene → Applied: {applied}");
    }

    void ApplyToPrefabs()
    {
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolderPath });

        int applied = 0;

        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = PrefabUtility.LoadPrefabContents(path);

            Light[] lights = prefab.GetComponentsInChildren<Light>(true);

            foreach (Light l in lights)
            {
                if (l.GetComponent<VolumetricAdditionalLight>() == null)
                {
                    l.gameObject.AddComponent<VolumetricAdditionalLight>();
                    applied++;
                }
            }

            PrefabUtility.SaveAsPrefabAsset(prefab, path);
            PrefabUtility.UnloadPrefabContents(prefab);
        }

        Debug.Log($"Prefabs → Applied: {applied}");
    }
}