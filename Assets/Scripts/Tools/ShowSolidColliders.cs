using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class ColliderGizmoDrawers
{
    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawBoxColliderGizmo(BoxCollider col, GizmoType gizmoType)
    {
        Gizmos.color = new Color(0, 1, 0, 0.25f); // semi-transparent green

        Gizmos.matrix = col.transform.localToWorldMatrix;
        Gizmos.DrawCube(col.center, col.size);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(col.center, col.size);

        Gizmos.matrix = Matrix4x4.identity;
    }
}
#endif