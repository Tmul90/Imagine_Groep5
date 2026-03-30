using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Util;

public class StimulationVisualManager : Singleton<StimulationVisualManager>
{
    [Header("References")]
    [SerializeField] private Volume globalVolume;

    [Header("Visual Curves")]
    [SerializeField] private AnimationCurve vignetteCurve;
    [SerializeField] private AnimationCurve focusDistanceCurve;

    private Camera _camera;
    private CameraShake _shake;
    private float _startFOV;

    private ChromaticAberration CA;
    private LensDistortion LD;
    private Vignette VN;
    private DepthOfField DOF;
    private Bloom BL;
    private ColorAdjustments CAD;

    protected override void Awake()
    {
        base.Awake();
        _camera = Camera.main;
        _startFOV = _camera.fieldOfView;
        _shake = _camera.GetComponent<CameraShake>();
    }

    private void OnEnable() => StimulationManager.OnStimulationChanged += HandleVisualFeedback;
    private void OnDisable() => StimulationManager.OnStimulationChanged -= HandleVisualFeedback;

    private void HandleVisualFeedback(float s)
    {
        globalVolume.profile.TryGet(out CA);
        globalVolume.profile.TryGet(out LD);
        globalVolume.profile.TryGet(out VN);
        globalVolume.profile.TryGet(out DOF);
        globalVolume.profile.TryGet(out CAD);
        
        if(CA is not null && LD is not null && VN is not null && DOF is not null && CAD is not null)
        {
            CA.intensity.value = Mathf.Lerp(s / 100, CA.intensity.value, 0.9f); // Colour Aberration
            LD.intensity.value = (s / 100) * 0.5f; // Lens Distortion
            VN.intensity.value = Mathf.Lerp(vignetteCurve.Evaluate(s / 100), VN.intensity.value, 0.9f); // Vignette
            DOF.focusDistance.value = Mathf.Lerp(focusDistanceCurve.Evaluate(s / 100) * 100, DOF.focusDistance.value, 0.9f); // Depth of field
            
            // Fade to black (only apply with a stimulation of > 955)
            if (s > 95)
                CAD.postExposure.value -= 10f * Time.deltaTime;
            else
                CAD.postExposure.value = 0f;
            
        } else
            Debug.LogWarning("Missing profile on global volume!");

        
        
        if (s > 50)
        {
            // Camera adjustments (only apply with a stimulation of >50%)
            _shake.SetShake((s - 50) * 0.0005f, (s - 50) * 0.001f);
            _camera.fieldOfView = _startFOV - ((s - 50) * 0.2f);
        }
        else
        {
            _camera.fieldOfView = _startFOV;
        }
        
        // Set bloom when entering a greenzone
        globalVolume.profile.TryGet(out BL);
        if (BL is not null)
            BL.intensity.value = OasisManager.Instance.inGreenZone
                ? Mathf.Lerp(50f, BL.intensity.value, 0.9f) 
                : Mathf.Lerp(0f, BL.intensity.value, 0.9f);

    }
    
}
