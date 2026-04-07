using System;
using Sound;
using UnityEngine;

public class SoundOnMove : MonoBehaviour
{
    [SerializeField] private AudioClip rollingClip;
    [SerializeField] private float rollingVolume = 3f;
    [SerializeField] private float rollingSpeedThreshold = 0.01f;
    [SerializeField] private float basePitch = 1f;
    [SerializeField] private float randomAddPitch = 0f;
    
    private Rigidbody _rb;

    private float timer = -2f;
    private AudioSource soundObject;

    private int baseChildren = 0;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        baseChildren = transform.childCount;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < 0f){return;}
        
        float velMagnitude = _rb.linearVelocity.magnitude;
        if (transform.childCount == baseChildren)
        {
            if (velMagnitude > rollingSpeedThreshold)
            {
                SoundManager.Instance.PlaySoundClip(rollingClip, transform, rollingVolume * velMagnitude, true, randomAddPitch, basePitch);
                soundObject = transform.GetChild(transform.childCount - 1).gameObject.GetComponent<AudioSource>();
                
                // Debug
                print("Pos: " + transform.position.ToString() + ", Name: " + name.ToString() + ", Parent: " + transform.parent.name.ToString());
            }
        }
        else
        {
            if (soundObject is null)
            {
                Debug.LogWarning("SoundOnMove - Sound Object is null, might grab wrong object");
                soundObject = transform.GetChild(transform.childCount - 1).gameObject.GetComponent<AudioSource>();
            }
            if (velMagnitude < rollingSpeedThreshold)
            {
                Destroy(soundObject.gameObject);
                soundObject = null;
            }
            else
            {
                soundObject.volume = rollingVolume * velMagnitude;
            }
        }
    }
}
