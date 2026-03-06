using UnityEngine;

[ExecuteAlways]
public class PipeGenerator : MonoBehaviour
{
    // REFERENCES
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    [SerializeField] private Transform mesh;


    private void Update()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        Vector3 p1 = point1.position;
        Vector3 p2 = point2.position;
        mesh.position = (p1 + p2) / 2;
        mesh.localScale = new Vector3(1, Vector3.Magnitude(p1 - p2), 1);

        Vector3 rotationVector = p1 - p2;
        mesh.rotation = Quaternion.LookRotation(rotationVector);
        mesh.eulerAngles += new Vector3(90, 0, 0);
    }
}
