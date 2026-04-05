using Sound;
using UnityEngine;

public class CrowSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] crowSounds;
    [SerializeField] private Vector2 randomCrowVolume = new Vector2(0.3f, 0.8f);
    [SerializeField] private float randomCrowPitch = 0.1f;
    [SerializeField] private Vector2 randomWaitTime = new Vector2(10f, 45f); // (In seconds)

    private float _timer = 0f;
    private float _currentTargetTime = 0f;

    private void Awake()
    {
        ResetTime();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _currentTargetTime)
        {
            ResetTime();
            float volume = Random.Range(randomCrowVolume.x, randomCrowVolume.y);
            SoundManager.Instance.PlayRandomClip(crowSounds, transform, volume, false, randomCrowPitch);
        }
    }

    private void ResetTime()
    {
        _timer = 0f;
        _currentTargetTime = Random.Range(randomWaitTime.x, randomWaitTime.y);
    }
}
