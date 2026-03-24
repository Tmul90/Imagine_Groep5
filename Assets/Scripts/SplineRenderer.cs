using UnityEngine;

public class SplineRenderer : MonoBehaviour
{
    
    private void Start()
    {
        var linePoints : Vector3[] = SomeKindOfSplineFunctionThatReturnsAnArray();
        var line = new VectorLine ("Spline", linePoints, lineMaterial, lineWidth);
        Vector.DrawLine (line);
    }
    
}
