using UnityEngine;

public class Mantleable : Parkourable
{
    [Header("Mantle Settings")]
    [SerializeField] private float mantleSpeed = 4f;
    [SerializeField] private float ledgeHeightOffset = 0.05f;
    
    public override ParkourType Type => ParkourType.Mantle;

    public override Vector3 GetInteractionPoint()
    {
        var collider = GetComponent<Collider>();
        return collider is not null
            ? new Vector3(transform.position.x, collider.bounds.max.y + ledgeHeightOffset, transform.position.z)
            : transform.position;
    }

    public override void Execute(ParkourController player)
    {
        player.PerformMantle(GetInteractionPoint(), mantleSpeed);
    }
}
