/*using UnityEngine;
using UnityEditor;

public class PhysicsMaterialApplier : MonoBehaviour
{
    public PhysicsMaterial material;
    public string prefabFolderPath = "Assets/Prefabs";

    [ContextMenu("Apply Physics Material To Scene + Prefabs")]
    public void ApplyMaterial()
    {
        if (material == null)
        {
            Debug.LogError("No PhysicMaterial assigned!");
            return;
        }

        ApplyToScene();
        ApplyToPrefabs();

        Debug.Log("Done applying physics material.");
    }

    void ApplyToScene()
    {
        Collider[] colliders = FindObjectsOfType<Collider>(true);

        int applied = 0;
        int removed = 0;

        foreach (Collider col in colliders)
        {
            Undo.RecordObject(col, "Apply Physics Material");

            Rigidbody rb = col.GetComponent<Rigidbody>();
            Rigidbody parentRB = col.GetComponentInParent<Rigidbody>();

            if (parentRB != null || col.isTrigger || rb != null)
            {
                if (col.sharedMaterial != null)
                {
                    col.sharedMaterial = null;
                    removed++;
                }
            }
            else
            {
                if (col.sharedMaterial != material)
                {
                    col.sharedMaterial = material;
                    applied++;
                }
            }

            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log("Found Player");
                col.sharedMaterial = material;
            }

            EditorUtility.SetDirty(col);
        }

        Debug.Log($"Scene → Applied: {applied}, Removed: {removed}");
    }

    void ApplyToPrefabs()
    {
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolderPath });

        int applied = 0;
        int removed = 0;

        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = PrefabUtility.LoadPrefabContents(path);

            Collider[] colliders = prefab.GetComponentsInChildren<Collider>(true);

            foreach (Collider col in colliders)
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                Rigidbody parentRB = col.GetComponentInParent<Rigidbody>();
                
                if (parentRB != null || col.isTrigger || rb != null)
                {
                    if (col.sharedMaterial != null)
                    {
                        col.sharedMaterial = null;
                        removed++;
                    }
                }
                else
                {
                    if (col.sharedMaterial != material)
                    {
                        col.sharedMaterial = material;
                        applied++;
                    }
                }
                
                if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Debug.Log("Found Player");
                    col.sharedMaterial = material;
                }
            }
            

            PrefabUtility.SaveAsPrefabAsset(prefab, path);
            PrefabUtility.UnloadPrefabContents(prefab);
        }

        Debug.Log($"Prefabs → Applied: {applied}, Removed: {removed}");
    }
}*/