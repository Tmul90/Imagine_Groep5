using UnityEngine;

public class Vaultable : Parkourable
{
    [Header("Vaultable Settings")] 
    [SerializeField] private float vaultSpeed = 5f;
    [SerializeField] private Transform vaultOverPoint;

    public override ParkourType Type => ParkourType.Vault;

    public override Vector3 GetInteractionPoint()
    {
        if (vaultOverPoint is not null) return vaultOverPoint.position;
        
        var collider = GetComponent<Collider>();
        return collider is not null 
            ? new Vector3(transform.position.x, collider.bounds.max.y, transform.position.z)
            : transform.position;
    }
    public override void Execute(ParkourController player)
    {
        player.PerformVault(GetInteractionPoint(), vaultSpeed);
    }
}
