using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace Sound
{
    public class SoundManager : Singleton<SoundManager>
    {
        [Header("Main Settings")]
        [SerializeField] private AudioSource soundObject; // The AudioSource used for sound playback

        /// <summary>
        /// Plays a specific sound clip at the given spawn point.
        /// </summary>
        /// <param name="audioClip">The audio clip to be played</param>
        /// <param name="soundObjectSpawn">The transform position where the sound should be played</param>
        /// <param name="volume">The volume level for the sound</param>
        /// <param name="loop">Loops the sound, be aware: clips have to be removed manually for them to stop</param>
        /// <param name="randomPitch">If not 0, applies a random pitch between 1-randomPitch and 1+randomPitch</param>
        public void PlaySoundClip(AudioClip audioClip, Transform soundObjectSpawn, float volume, bool loop = false, float randomPitch = 0f)
        {
            var clipLength = audioClip.length;
            var audioSource = Instantiate(soundObject, soundObjectSpawn);
            
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play();
            audioSource.loop = loop;

            if (randomPitch != 0f)
            {
                audioSource.pitch = 1f + Random.Range(-randomPitch,randomPitch);
            }
            
            if (loop) {return;}
            Destroy(audioSource.gameObject, clipLength);
        }

        /// <summary>
        /// Plays a random clip from an array of audio clips at the specified location.
        /// </summary>
        /// <param name="audioClips">Array of audio clips to choose from</param>
        /// <param name="soundObjectSpawn">The transform position where the sound should be played</param>
        /// <param name="volume">The volume level for the sound</param>
        /// <param name="loop">Loops the sound, be aware: clips have to be removed manually for them to stop</param>
        /// <param name="randomPitch">If not 0, applies a random pitch between 1-randomPitch and 1+randomPitch</param>
        public void PlayRandomClip(AudioClip[] audioClips, Transform soundObjectSpawn, float volume, bool loop = false, float randomPitch = 0f)
        {
            if (audioClips.Length == 0) return;

            var randClip = audioClips[Random.Range(0, audioClips.Length)];
            PlaySoundClip(randClip, soundObjectSpawn, volume, loop, randomPitch);
        }
    }
}
