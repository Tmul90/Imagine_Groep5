using UnityEngine;
using Util;

public class StimulationAuditoryManager : Singleton<StimulationAuditoryManager>
{
    [Header("References")]
    [SerializeField] private GameObject audioSource;
    [SerializeField] private AudioSource beep;
    private AudioEchoFilter echoFilter;
    private AudioLowPassFilter lowPassFilter;
    private AudioReverbFilter reverbFilter;

    [Header("Auditory Curves")]
    [SerializeField] private AnimationCurve beepCurve;
    

    protected override void Awake()
    {
        base.Awake();
        reverbFilter = audioSource.GetComponent<AudioReverbFilter>();
        echoFilter = audioSource.GetComponent<AudioEchoFilter>();
        lowPassFilter =  audioSource.GetComponent<AudioLowPassFilter>();
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
    
}
