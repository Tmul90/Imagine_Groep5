using UnityEngine;

public class Hangable : Parkourable
{
    [Header("Hang Settings")] 
    [SerializeField] private float hangSpeed = 3f;
    [SerializeField] private float traverseSpeed = 3f;
    
    internal override ParkourType Type => ParkourType.Hangable;

    internal override Vector3 GetInteractionPoint(Transform playerTransform = null)
    {
        var collider = GetComponent<Collider>();
        if (collider is null) return transform.position;
        
        var closest = collider.ClosestPoint(playerTransform is not null 
            ? playerTransform.position 
            : transform.position);
        
        return new Vector3(closest.x, collider.bounds.min.y, closest.z);
    }

    internal override void Execute(ParkourController player)
    {
        player.PerformHang(GetInteractionPoint(player.transform), this, hangSpeed, traverseSpeed);
    }

    internal Vector3 ClampToSurface(Vector3 worldPosition)
    {
        var collider = GetComponent<Collider>();
        if (collider is null) return worldPosition;

        return new Vector3(
            Mathf.Clamp(worldPosition.x, collider.bounds.min.x, collider.bounds.max.x),
            collider.bounds.min.y,
            Mathf.Clamp(worldPosition.z, collider.bounds.min.z, collider.bounds.max.z)
        );
    }
    
    internal Vector3 GetPipeAxis()
    {
        var col = GetComponent<Collider>();
        if (col is null) return transform.right;

        var size = col.bounds.size;

        if (size.x >= size.y && size.x >= size.z) return Vector3.right;
        if (size.z >= size.x && size.z >= size.y) return Vector3.forward;
        return Vector3.up;
    }
    
    internal bool IsAtEnd(Vector3 worldPosition)
    {
        var collider = GetComponent<Collider>();
        if (collider is null) return false;
        var clamped = ClampToSurface(worldPosition);
        return Vector3.Distance(clamped, worldPosition) > 0.05f;
    }
    
}
