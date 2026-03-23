using UnityEngine;

public class Mantleable : Parkourable
{
    [Header("Mantle Settings")]
    [SerializeField] private float mantleSpeed = 4f;
    [SerializeField] private float ledgeHeightOffset = 0.05f;
    
    internal override ParkourType Type => ParkourType.Mantleable;

    internal override Vector3 GetInteractionPoint(Transform playerTransform = null)
    {
        var collider = GetComponent<Collider>();
        
        if (collider is null) return transform.position;
        
        var ledgeY = collider.bounds.max.y + ledgeHeightOffset;

        if (playerTransform is not null)
        {
            var clampedX = Mathf.Clamp(playerTransform.position.x, collider.bounds.min.x, collider.bounds.max.x);
            var clampedZ = Mathf.Clamp(playerTransform.position.z, collider.bounds.min.z, collider.bounds.max.z);
            return new Vector3(clampedX, ledgeY, clampedZ);
        }
        
        return new Vector3(transform.position.x, ledgeY, transform.position.z);
    }

    internal override void Execute(ParkourController player)
    {
        player.PerformMantle(GetInteractionPoint(), mantleSpeed);
    }
}
