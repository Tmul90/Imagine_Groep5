using Sound;
using UnityEngine;

public class OasisCollisionChecker : MonoBehaviour
{
    public bool collide = false;
    public GameObject otherObject;
    [SerializeField] private AudioClip enterZoneClip;
    [SerializeField] private float volume = 1f;
    
    private void OnTriggerEnter(Collider other)
    {
        collide = true;
        otherObject = other.gameObject;
        SoundManager.Instance.PlaySoundClip(enterZoneClip, transform, volume);
    }

    private void OnTriggerExit(Collider other)
    {
        collide = false;
    }
}
