using UnityEngine;
using UnityEditor;

public class FlipColliderAxis : MonoBehaviour
{
    public string prefabFolderPath = "Assets/Prefabs";

    [ContextMenu("Apply Flipped Scale To Scene + Prefabs")]
    public void ApplyMaterial()
    {
        ApplyToScene();
        ApplyToPrefabs();

        Debug.Log("Done applying physics material.");
    }

    void ApplyToScene()
    {
        BoxCollider[] colliders = FindObjectsOfType<BoxCollider>(true);

        int flipped = 0;
        foreach (BoxCollider col in colliders)
        {
            Undo.RecordObject(col, "Apply Physics Material");
            Vector3 scale = col.transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            scale.y = Mathf.Abs(scale.y);
            scale.z = Mathf.Abs(scale.z);

            if (scale != col.transform.localScale)
            {
                flipped += 1;
                col.transform.localScale = scale;
            }

            EditorUtility.SetDirty(col);
        }

        Debug.Log($"Scene → flipped: {flipped}");
    }

    void ApplyToPrefabs()
    {
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolderPath });

        int flipped = 0;

        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = PrefabUtility.LoadPrefabContents(path);

            BoxCollider[] colliders = prefab.GetComponentsInChildren<BoxCollider>(true);

            foreach (BoxCollider col in colliders)
            {
                Undo.RecordObject(col, "Apply Physics Material");
                Vector3 scale = col.transform.localScale;
                scale.x = Mathf.Abs(scale.x);
                scale.y = Mathf.Abs(scale.y);
                scale.z = Mathf.Abs(scale.z);

                if (scale != col.transform.localScale)
                {
                    flipped += 1;
                    col.transform.localScale = scale;
                }

                EditorUtility.SetDirty(col);
            }
            
            PrefabUtility.SaveAsPrefabAsset(prefab, path);
            PrefabUtility.UnloadPrefabContents(prefab);
        }

        Debug.Log($"Prefabs → flipped: {flipped}");
    }
}
