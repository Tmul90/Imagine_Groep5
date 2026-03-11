using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; 

public class OverStim : MonoBehaviour
{
    // References
    [SerializeField] private Volume globalVolume;
    public Transform player;
    public GreenZones greenZones;
    private Camera cam;
    private CameraShake shake;
    // Variables
    public float stimulationPercentage = 0f; // Should be between 0 and 100
    [SerializeField] private AnimationCurve stimCurve; // Adding stimulation is based on height, calculated with this curve, 0 is minHeight, 1 is maxHeight. Value is added every second.
    [SerializeField] private Vector2 stimHeightBounds; // x : minHeight, y : maxHeight
    [SerializeField] private float addStimulationSpeed = 1.3f;
    [SerializeField] private float stimulationRecoverySpeed = 10f;
    [SerializeField] private AnimationCurve focusDistanceCurve;
    // Adjustments
    private ChromaticAberration CA;
    private LensDistortion LD;
    private Vignette VN;
    private DepthOfField DOF;

    private void Awake()
    {
        cam = Camera.main;
        shake = cam.GetComponent<CameraShake>();
    }
    
    private void Update()
    {
        AddStimulation();
        VisualFeedback();
        print(stimulationPercentage);
    }

    private void AddStimulation()
    {
        if (greenZones.inGreenZone)
        {
            stimulationPercentage -= stimulationRecoverySpeed * Time.deltaTime;
        } else
        {
            float playerHeight = player.position.y;
            float clampedHeight = Mathf.Clamp(playerHeight, stimHeightBounds.x, stimHeightBounds.y);
            float curvePosition = (clampedHeight - stimHeightBounds.x) / (stimHeightBounds.y - stimHeightBounds.x);
            float addStimulation = stimCurve.Evaluate(curvePosition);
        
            stimulationPercentage += addStimulation * Time.deltaTime * addStimulationSpeed;
        }
        
        stimulationPercentage = Mathf.Clamp(stimulationPercentage, 0f, 100f);
    }

    private void VisualFeedback()
    {
        float s = stimulationPercentage;
        
        globalVolume.profile.TryGet(out CA);
        globalVolume.profile.TryGet(out LD);
        globalVolume.profile.TryGet(out VN);
        globalVolume.profile.TryGet(out DOF);
        if(CA != null && LD != null && VN != null && DOF != null)
        {

            CA.intensity.value = s / 100;
            LD.intensity.value = (s / 100) * 0.5f;
            VN.intensity.value = s / 100 * 0.45f;
            DOF.focusDistance.value = focusDistanceCurve.Evaluate(s / 100) * 100;
        }

        if (s > 50)
        {
            float posStrength = (s - 50) * 0.006f;
            float rotStrength = (s - 50) * 0.004f;

            shake.SetShake(posStrength, rotStrength);
            
            cam.fieldOfView = 70 - ((s - 50) * 0.6f);
            
        }

    }
    
}
