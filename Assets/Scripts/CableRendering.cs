using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class CableRendering : MonoBehaviour
{
    [SerializeField] private float hang = 2f;
    [SerializeField] private int hangPoints = 5;
    
    private LineRenderer _lr;

    private List<Vector3> _points = new List<Vector3>();
    private List<Vector3> _prevPoints =  new List<Vector3>();
    
    private void Awake()
    {
        _lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        SetPoints(_points);
        if (_points != _prevPoints)
        {
            UpdateLine();
        }
        SetPoints(_prevPoints);
    }

    private void SetPoints(List<Vector3> list)
    {
        list.Clear();
        foreach (var child in transform.GetComponentsInChildren<Transform>())
        {
            list.Add(child.position);
        }
    }

    private void UpdateLine()
    {
        // Configure points
        _points.RemoveAt(0);
        var referencePoints = new List<Vector3>();
        
        for (int i = 0; i < _points.Count; i++)
        {
            var point = _points[i];
            
            if (i != 0)
            {
                var hangMultiplier = Vector3.Distance(_points[i - 1], _points[i]);
                
                for (int j = 0; j <= hangPoints - 1; j++)
                {
                    var t = (j + 1f) / (hangPoints + 1f);
                    var center = Vector3.Lerp(_points[i - 1], point, t);
                    var hangAmount = 1f - Mathf.Pow((t - 0.5f) * 2f, 2f);
                    center.y -= hang * hangAmount * hangMultiplier;
                    referencePoints.Add(center);
                }

            }
            
            referencePoints.Add(point);
        }
        
        // Apply points
        _lr.positionCount = referencePoints.Count;
        _lr.SetPositions(referencePoints.ToArray());
    }
}
