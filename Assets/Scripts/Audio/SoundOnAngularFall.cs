using System;
using Sound;
using UnityEngine;

public class Lantern : MonoBehaviour
{
    [SerializeField] private AudioClip lanternSound;
    [SerializeField] private float lanternVolume = 1f;
    [SerializeField] private float lanternRandomPitch = 0.1f;
    [SerializeField] private float velocityThreshold = 0.3f;
    
    private Rigidbody _rb;
    private float _previousAngularVelocity = 0f;
    private bool _fallen = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float vel = _rb.angularVelocity.magnitude;
        float distToZero = Mathf.Abs(vel);
        if (Mathf.Abs(_previousAngularVelocity - vel) > velocityThreshold && distToZero < 0.5f && !_fallen)
        {
            SoundManager.Instance.PlaySoundClip(lanternSound, transform, lanternVolume, false, lanternRandomPitch);
            _fallen = true;
            
            // DEBUG, to know where sounds are coming from if they fall on their own
            //print(transform.position.ToString() + transform.parent.name.ToString());
        }
        _previousAngularVelocity = vel;
    }
}
