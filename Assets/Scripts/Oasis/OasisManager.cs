using System;
using UnityEngine;
using Util;

public class OasisManager : Singleton<OasisManager>
{
    public static event Action<Vector3> OnRespawnChange;
    
    public bool inGreenZone => activeZoneCount > 0;

    private int activeZoneCount = 0;

    private void OnEnable()
    {
        Oasis.OnPlayerEnter += HandlePlayerEnter;
        Oasis.OnPlayerExit  += HandlePlayerExit;
    }

    private void OnDisable()
    {
        Oasis.OnPlayerEnter -= HandlePlayerEnter;
        Oasis.OnPlayerExit  -= HandlePlayerExit;
    }

    private void HandlePlayerEnter(Vector3 spawnPosition)
    {
        activeZoneCount++;
        OnRespawnChange?.Invoke(spawnPosition);
    }

    private void HandlePlayerExit()
    {
        activeZoneCount = Mathf.Max(0, activeZoneCount - 1);
    }
}
