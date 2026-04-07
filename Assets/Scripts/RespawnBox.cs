using System;
using UnityEngine;
using Util;

[RequireComponent(typeof(BoxCollider))]
public class RespawnBox : Singleton<RespawnBox>
{
    public event Action Respawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
            Respawn?.Invoke();
    }
}
