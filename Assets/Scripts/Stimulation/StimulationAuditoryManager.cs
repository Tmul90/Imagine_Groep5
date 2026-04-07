using UnityEngine;
using Util;

public class StimulationAuditoryManager : Singleton<StimulationAuditoryManager>
{
    [Header("References")]
    [SerializeField] private AudioSource crowd;
    [SerializeField] private AudioSource nature;
    [SerializeField] private AudioSource beep;
    private AudioEchoFilter echoFilter;
    private AudioLowPassFilter lowPassFilter;
    private AudioReverbFilter reverbFilter;

    [Header("Auditory Curves")]
    [SerializeField] private AnimationCurve beepCurve;
    [SerializeField] private float natureSoundMultiplier = 1f;
    [SerializeField] private float crowdSoundMultiplier = 1f;
    

    protected override void Awake()
    {
        base.Awake();
        reverbFilter = crowd.GetComponent<AudioReverbFilter>();
        echoFilter = crowd.GetComponent<AudioEchoFilter>();
        lowPassFilter =  crowd.GetComponent<AudioLowPassFilter>();
    }

    private void OnEnable() => StimulationManager.OnStimulationChanged += HandleAuditoryFeedback;
    private void OnDisable() => StimulationManager.OnStimulationChanged -= HandleAuditoryFeedback;

    private void HandleAuditoryFeedback(float s)
    {
        float height = PlayerController.Instance.GetHeight();
        reverbFilter.dryLevel = 0 - (height * 35f);
        echoFilter.wetMix = s / 100f;
        lowPassFilter.cutoffFrequency = ((s * -1f) + 100f) * 220f;
        beep.volume = beepCurve.Evaluate(s / 100f);
    }

    internal void SetVolumes(float percentage = 0f)
    {
        crowd.volume = percentage * crowdSoundMultiplier;
        nature.volume = (1f - percentage) * natureSoundMultiplier;
    }
    
}
