using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sound
{
    public class SoundService : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private AudioClip[] audioClips; // Array of audio clips that can be played
        [SerializeField] private float volume; // Volume level for audio playback
        
        [Header("Audio Settings")]
        [SerializeField] private Transform soundObjectSpawn; // The position where sound objects spawn
        
        /// <summary>
        /// Plays a random footstep sound effect.
        /// </summary>
        public void FootStep()
        {
            SoundManager.Instance.PlayRandomClip(audioClips, soundObjectSpawn, volume);
        }
    }
}