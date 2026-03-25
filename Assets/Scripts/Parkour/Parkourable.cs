using UnityEngine;

[RequireComponent(typeof(Collider))] 
public abstract class Parkourable : MonoBehaviour
{
    [Header("Parkour Settings")] 
    [SerializeField] protected float interactionRange = 1.5f;
    [SerializeField] protected bool isEnabled = true;
    
    internal abstract ParkourType Type { get; }

    internal abstract void Execute(ParkourController player);
    
    internal virtual void OnPlayerInRange(ParkourController player) { }
    
    internal virtual void OnPlayerExitRange(ParkourController player) { }
    
    internal bool IsEnabled => isEnabled;
    internal float InteractionRange => interactionRange;
    
    internal virtual Vector3 GetInteractionPoint(Transform playerTransform = null) => transform.position;

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(GetInteractionPoint(), 0.15f);
    }
}

public enum ParkourType { Vaultable, Climbable, Mantleable, Hangable }