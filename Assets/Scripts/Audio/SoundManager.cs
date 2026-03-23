using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Sound
{
    public class SoundManager : Singleton<SoundManager>
    {
        [Header("Main Settings")]
        [SerializeField] private AudioSource soundObject; // The AudioSource used for sound playback
        [SerializeField] private AudioClip kachingSound; // The "kaching" sound for successful transactions
        [SerializeField] private float kachingSoundVolume;
        
        [Header("Customer Clips")]
        [SerializeField] private AudioClip[] audioClipsBye; // Array of goodbye audio clips
        [SerializeField] private float byeSoundVolume;
        [SerializeField] private AudioClip[] audioClipsHello; // Array of hello audio clips
        [SerializeField] private float helloSoundVolume;
        [SerializeField] private AudioClip[] audioClipsGrunt; // Array of grunt audio clips
        [SerializeField] private float gruntSoundVolume;
        
        [Header("Customer Clips")]
        [SerializeField] private AudioClip doorSoundClip; // The sound that plays when the door opens
        [SerializeField] private float doorSoundVolume;
        [FormerlySerializedAs("doorBellClip")] [SerializeField] private AudioClip doorBellSoundClip; // The sound that plays when the door opens
        [SerializeField] private float doorBellSoundVolume;

        /// <summary>
        /// Plays a specific sound clip at the given spawn point.
        /// </summary>
        /// <param name="audioClip">The audio clip to be played</param>
        /// <param name="soundObjectSpawn">The transform position where the sound should be played</param>
        /// <param name="volume">The volume level for the sound</param>
        public void PlaySoundClip(AudioClip audioClip, Transform soundObjectSpawn, float volume)
        {
            var clipLength = audioClip.length;
            var audioSource = Instantiate(soundObject, soundObjectSpawn);
            
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
            Destroy(audioSource.gameObject, clipLength);
        }
        
        /// <summary>
        /// Plays a random clip from an array of audio clips at the specified location.
        /// </summary>
        /// <param name="audioClips">Array of audio clips to choose from</param>
        /// <param name="soundObjectSpawn">The transform position where the sound should be played</param>
        /// <param name="volume">The volume level for the sound</param>
        public void PlayRandomClip(AudioClip[] audioClips, Transform soundObjectSpawn, float volume)
        {
            if (audioClips.Length == 0) return;
            
            var randClip = audioClips[Random.Range(0, audioClips.Length)];
            PlaySoundClip(randClip, soundObjectSpawn, volume);
        }
        
        /// <summary>
        /// Plays the "kaching" sound to signify a successful transaction.
        /// </summary>
        public void PlayKachingSound() =>
            PlaySoundClip(kachingSound, Camera.main?.transform, kachingSoundVolume);

        /// <summary>
        /// Plays a random hello sound when a customer is at the counter.
        /// </summary>
        public void IsAtCounter() =>
            PlayRandomClip(audioClipsHello, Camera.main?.transform, helloSoundVolume);

        /// <summary>
        /// Plays a random goodbye sound when a customer leaves.
        /// </summary>
        public void CustomerLeave() =>
            PlayRandomClip(audioClipsBye, Camera.main?.transform, byeSoundVolume);

        /// <summary>
        /// Plays a random grunt sound when a customer grunts.
        /// </summary>
        public void OnCustomerGrunt() =>
            PlayRandomClip(audioClipsGrunt, Camera.main?.transform, gruntSoundVolume);

        public void OnDoorSound()
        {
            PlaySoundClip(doorSoundClip, Camera.main?.transform, doorSoundVolume);
            PlaySoundClip(doorBellSoundClip, Camera.main?.transform, doorBellSoundVolume);
        }
    }
}
