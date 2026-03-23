using UnityEngine;

public class Vaultable : Parkourable
{
    [Header("Vaultable Settings")] 
    [SerializeField] private float vaultSpeed = 5f;
    [SerializeField] private Transform vaultOverPoint;

    internal override ParkourType Type => ParkourType.Vaultable;

    internal override Vector3 GetInteractionPoint(Transform playerTransform = null)
    {
        if (vaultOverPoint is not null) return vaultOverPoint.position;
        
        var collider = GetComponent<Collider>();
        return collider is not null 
            ? new Vector3(transform.position.x, collider.bounds.max.y, transform.position.z)
            : transform.position;
    }
    internal override void Execute(ParkourController player)
    {
        player.PerformVault(GetInteractionPoint(player.transform), vaultSpeed);
    }
}
