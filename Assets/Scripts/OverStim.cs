using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; 

public class OverStim : MonoBehaviour
{
    // REFERENCES
    [Header("References")]
    [SerializeField] private Volume globalVolume;
    public GameObject player;
    private PlayerController playerController;
    public GreenZones greenZones;
    private Camera cam;
    private CameraShake shake;
    // VARIABLES
    public float stimulationPercentage = 0f; // Should be between 0 and 100
    [Header("Stimulation settings")]
    [SerializeField] private AnimationCurve stimulationCurve; // Adding stimulation is based on height, calculated with this curve, 0 is minHeight, 1 is maxHeight. Value is added every second.
    [SerializeField] private Vector2 stimulationHeightBounds; // x : minHeight, y : maxHeight
    [SerializeField] private float addStimulationSpeed = 5f; // How much stimulation is added per second (if it adds 100%, see stimulationCurve)
    [SerializeField] private float stimulationRecoverySpeed = 10f; // How much stimulation is subtracted per second in greenzones
    [Header("Stimulation Visual settings")]
    [SerializeField] private AnimationCurve focusDistanceCurve; // Needed a curve to handle focus distance due to it being exponential
    [SerializeField] private AnimationCurve vignetteCurve; // Needed a curve to handle vignette because it should increase rapidly at 100%
    // ADJUSTMENTS
    private ChromaticAberration CA;
    private LensDistortion LD;
    private Vignette VN;
    private DepthOfField DOF;
    private Bloom BL;
    // PRIVATE VARIABLES
    private float startFOV = 0;

    
    private void Awake()
    {
        cam = Camera.main;
        startFOV = cam.fieldOfView;
        shake = cam.GetComponent<CameraShake>();
        playerController = player.GetComponent<PlayerController>();
    }
    
    
    private void Update()
    {
        AddStimulation();
        VisualFeedback();
        Respawn();
    }

    
    private void AddStimulation()
    {
        if (greenZones.inGreenZone)
            // In greenzone
            stimulationPercentage -= stimulationRecoverySpeed * Time.deltaTime;
        else
        {
            // Outside greenzone
            float playerHeight = player.transform.position.y; // Get player height
            float clampedHeight = Mathf.Clamp(playerHeight, stimulationHeightBounds.x, stimulationHeightBounds.y); // Clamp height to bounds
            float curvePosition = (clampedHeight - stimulationHeightBounds.x) / (stimulationHeightBounds.y - stimulationHeightBounds.x); // Get a value between 0 and 1
            float addStimulation = stimulationCurve.Evaluate(curvePosition); // Evaluate curve based on the player's height
        
            stimulationPercentage += addStimulation * Time.deltaTime * addStimulationSpeed; // Apply stimulation
        }
        
        stimulationPercentage = Mathf.Clamp(stimulationPercentage, 0f, 100f); // Clamp stimulationn
        print(stimulationPercentage); // DEBUG
    }

    
    private void VisualFeedback()
    {
        float s = stimulationPercentage;
        
        // Set stimulation params
        globalVolume.profile.TryGet(out CA);
        globalVolume.profile.TryGet(out LD);
        globalVolume.profile.TryGet(out VN);
        globalVolume.profile.TryGet(out DOF);
        if(CA != null && LD != null && VN != null && DOF != null)
        {
            CA.intensity.value = Mathf.Lerp(s / 100, CA.intensity.value, 0.9f); // Colour Aberration
            LD.intensity.value = Mathf.Lerp((s / 100) * 0.5f, LD.intensity.value, 0.9f); // Lens Distortion
            VN.intensity.value = Mathf.Lerp(vignetteCurve.Evaluate(s / 100), VN.intensity.value, 0.9f); // Vignette
            DOF.focusDistance.value = Mathf.Lerp(focusDistanceCurve.Evaluate(s / 100) * 100, DOF.focusDistance.value, 0.9f); // Depth of field
        }
        
        // Camera adjustments (only apply with a stimulation of >50%)
        if (s > 50)
        {
            // Shake
            float posStrength = (s - 50) * 0.0005f;
            float rotStrength = (s - 50) * 0.001f;
            shake.SetShake(posStrength, rotStrength);
            // FOV
            cam.fieldOfView = startFOV - ((s - 50) * 0.2f);
        }
        else
        {
            cam.fieldOfView = startFOV;
        }
        
        // Set bloom when entering a greenzone
        globalVolume.profile.TryGet(out BL);
        if (BL != null)
        {
            if (greenZones.inGreenZone)
                BL.intensity.value = Mathf.Lerp(50f, BL.intensity.value, 0.9f);
            else
                BL.intensity.value = Mathf.Lerp(0f, BL.intensity.value, 0.9f);
        }
    }
    
    private void Respawn()
    {
        if (stimulationPercentage == 100)
        {
            playerController.Respawn();
            stimulationPercentage = 0;
        }
    }
}
