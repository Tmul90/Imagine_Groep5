using System;
using Sound;
using UnityEngine;

public class Trashcan : MonoBehaviour
{
    [SerializeField] private AudioClip rollingClip;
    [SerializeField] private float rollingVolume = 3f;
    [SerializeField] private float rollingSpeedThreshold = 0.01f;
    
    private Rigidbody _rb;

    private float timer = -2f;
    private AudioSource soundObject;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < 0f){return;}
        
        float velMagnitude = _rb.linearVelocity.magnitude;
        if (transform.childCount == 2)
        {
            if (velMagnitude > rollingSpeedThreshold)
            {
                SoundManager.Instance.PlaySoundClip(rollingClip, transform, rollingVolume * velMagnitude, true);
                soundObject = transform.GetChild(transform.childCount - 1).gameObject.GetComponent<AudioSource>();
            }
        }
        else
        {
            if (velMagnitude < rollingSpeedThreshold)
            {
                Destroy(soundObject.gameObject);
            }
            else
            {
                soundObject.volume = rollingVolume * velMagnitude;
            }
        }
    }
}
