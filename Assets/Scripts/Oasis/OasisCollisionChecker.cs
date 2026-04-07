using Sound;
using UnityEngine;
using UnityEngine.Events;

public class OasisCollisionChecker : MonoBehaviour
{
    public bool collide = false;
    public GameObject otherObject;
    
    public UnityEvent          SetKillBoxPosition;
    [SerializeField] private AudioClip enterZoneClip;
    [SerializeField] private float volume = 1f;
    
    private void OnTriggerEnter(Collider other)
    {
        collide = true;
        otherObject = other.gameObject;
        SoundManager.Instance.PlaySoundClip(enterZoneClip, transform, volume);
        SetKillBoxPosition?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        collide = false;
    }
}
