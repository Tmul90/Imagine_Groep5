using System;
using System.Linq;
using UnityEngine;
using Util;

[RequireComponent(typeof(StimulationVisualManager))]
[RequireComponent(typeof(StimulationAuditoryManager))]
public class StimulationManager : Singleton<StimulationManager>
{
    public static event Action OnRespawn;
    public static event Action<float> OnStimulationChanged;

    [Header("Stimulation Settings")]
    [SerializeField] private float stimulationPercentage;
    [SerializeField] private AnimationCurve stimulationCurve;
    [SerializeField] private Vector2 stimulationHeightBounds;
    [SerializeField] private float addStimulationSpeed = 5f;
    [SerializeField] private float stimulationRecoverySpeed = 10f;
    [SerializeField] private Vector2 natureSoundDistance = new Vector2(10f, 20f);
    [SerializeField] private GameObject customPlayer = null;
    
    private StimulationAuditoryManager auditoryManager;

    private void Awake()
    {
        auditoryManager = GetComponent<StimulationAuditoryManager>();
    }
    
    private void Update()
    {
        AddStimulation();
        GetClosestOasisDistance();
        OnStimulationChanged?.Invoke(stimulationPercentage);
        SetRespawnCallback();
        
        // DEBUG
        if (Input.GetKeyDown(KeyCode.R))
        {
            stimulationPercentage = 100f;
        }

    }

    private void AddStimulation()
    {
        if (OasisManager.Instance.inGreenZone)
            stimulationPercentage -= stimulationRecoverySpeed * Time.deltaTime;
        else
        {
            var playerHeight = PlayerController.Instance.GetHeight();
            var clampedHeight = Mathf.Clamp(playerHeight, stimulationHeightBounds.x, stimulationHeightBounds.y);
            var curvePosition = (clampedHeight - stimulationHeightBounds.x) / (stimulationHeightBounds.y - stimulationHeightBounds.x);
            stimulationPercentage += stimulationCurve.Evaluate(curvePosition) * Time.deltaTime * addStimulationSpeed;
        }
        
        stimulationPercentage = Mathf.Clamp(stimulationPercentage, 0f, 100f);
    }
    private void SetRespawnCallback()
    {
        if (stimulationPercentage < 100.0f) return;
        OnRespawn?.Invoke();
        stimulationPercentage = 0;
    }
    
    private void GetClosestOasisDistance()
    {
        Transform oasisTransform = OasisManager.Instance.transform;
        float nearest = 99999999999f;
        for (int i = 0; i < oasisTransform.childCount; i++)
        {
            Vector3 oasisPos = oasisTransform.GetChild(i).GetChild(0).transform.position;
            Vector3 playerPos = customPlayer is null ? customPlayer.transform.position : PlayerController.Instance.transform.position;
            float distance = Vector3.Distance(oasisPos, playerPos);
            nearest = distance < nearest ? distance : nearest;
        }
        
        float min = natureSoundDistance.x;
        float max = natureSoundDistance.y;
        float minus = 1 / ((max - min) / min);
        float percentage = Mathf.Clamp01((nearest / (max - min)) - minus);
        auditoryManager.SetVolumes(percentage);
    }

}