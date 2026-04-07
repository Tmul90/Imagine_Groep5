using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(OasisCollisionChecker))]
public class Oasis : MonoBehaviour
{
    public static event Action<Vector3> OnPlayerEnter;
    public static event Action          OnPlayerExit;
    
    [SerializeField] private Transform spawnPoint;
    private OasisCollisionChecker _oasisCollisionChecker;
    private bool _playerInside = false;

    private void Awake() =>
        _oasisCollisionChecker = GetComponent<OasisCollisionChecker>();

    private void Update()
    {
        var playerPresent = _oasisCollisionChecker.collide
                            && _oasisCollisionChecker.otherObject is not null
                            && _oasisCollisionChecker.otherObject.GetComponent<PlayerController>() is not null;

        switch (playerPresent)
        {
            case true when !_playerInside:
                _playerInside = true;
                OnPlayerEnter?.Invoke(spawnPoint.position);
                break;
            case false when _playerInside:
                _playerInside = false;
                OnPlayerExit?.Invoke();
                break;
        }
    }

}
