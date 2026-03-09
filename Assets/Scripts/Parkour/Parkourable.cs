using UnityEngine;

[RequireComponent(typeof(Collider))] 
public abstract class Parkourable : MonoBehaviour
{
    [Header("Parkour Settings")] 
    [SerializeField] protected float interactionRange = 1.5f;
    [SerializeField] protected bool isEnabled = true;
    
    public abstract ParkourType Type { get; }

    public abstract void Execute(ParkourController player);
    
    public virtual void OnPlayerInRange(ParkourController player) { }
    
    public virtual void OnPlayerExitRange(ParkourController player) { }
    
    public bool IsEnabled => isEnabled;
    public float InteractionRange => interactionRange;
    
    public virtual Vector3 GetInteractionPoint() => transform.position;

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(GetInteractionPoint(), 0.15f);
    }
}

public enum ParkourType { Vault, Climb, Mantle }