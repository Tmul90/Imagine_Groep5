using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(OasisCollisionChecker))]
public class Oasis : MonoBehaviour
{
    public static event Action<Vector3> OnPlayerEnter;
    public static event Action          OnPlayerExit;
    
    [SerializeField] private Transform spawnPoint;
    private OasisCollisionChecker _oasisCollisionChecker;
    private bool playerInside = false;

    private void Awake() => _oasisCollisionChecker = GetComponent<OasisCollisionChecker>();

    private void Update()
    {
        var playerPresent = _oasisCollisionChecker.collide
                            && _oasisCollisionChecker.otherObject is not null
                            && _oasisCollisionChecker.otherObject.GetComponent<PlayerController>() is not null;

        switch (playerPresent)
        {
            case true when !playerInside:
                playerInside = true;
                OnPlayerEnter?.Invoke(spawnPoint.position);
                break;
            case false when playerInside:
                playerInside = false;
                OnPlayerExit?.Invoke();
                break;
        }
    }

}
