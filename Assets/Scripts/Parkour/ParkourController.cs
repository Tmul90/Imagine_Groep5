using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerController))]
public class ParkourController : MonoBehaviour
{
    
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 1.8f;
    [SerializeField] private LayerMask parkourLayer;
    [SerializeField] private KeyCode parkourKeyCode = KeyCode.LeftShift;

    private int _detectionFrame = 0;
    private Parkourable _currentTarget;
    private Rigidbody _rb;
    private PlayerController _playerController;

    public bool IsPerformingAction { get; private set; } = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerController = GetComponent<PlayerController>();
    }
    // ReSharper disable Unity.PerformanceAnalysis
    private void Update()
    {
        if (IsPerformingAction) { return; }

        if (_detectionFrame++ % 3 == 0) { DetectParkourable(); }

        if (_currentTarget is not null && Input.GetKey(parkourKeyCode)) { _currentTarget.Execute(this); }
    }
    
    private void DetectParkourable()
    {
        var best = Physics.OverlapSphere(transform.position, detectionRadius, parkourLayer)
            .Select(hit => hit.GetComponent<Parkourable>())
            .Where(parkourable => parkourable is not null && parkourable.IsEnabled)
            .Where(parkourable => Vector3.Distance(transform.position, parkourable.GetInteractionPoint()) <= parkourable.InteractionRange)
            .OrderBy(parkourable => Vector3.Distance(transform.position, parkourable.GetInteractionPoint()))
            .FirstOrDefault();

        if (best != _currentTarget)
        {
            _currentTarget?.OnPlayerExitRange(this);
            _currentTarget = best;
            _currentTarget?.OnPlayerInRange(this);
        }
    }

    public void PerformVault(Vector3 overPoint, float speed)
    {
        StartCoroutine(VaultRoutine(overPoint, speed));
    }

    private IEnumerator VaultRoutine(Vector3 overPoint, float speed)
    {
        IsPerformingAction = true;
        _playerController.SetMovementEnabled(false);
        
        var startPos = transform.position;
        var topPos = new Vector3(transform.position.x, overPoint.y + 0.1f, transform.position.z);
        var endPos = topPos + transform.forward * 1.2f;
        
        yield return MoveToPoint(startPos, topPos, speed);
        yield return MoveToPoint(topPos, endPos, speed * 1.5f);

        _rb.linearVelocity = Vector3.zero;
        _playerController.SetMovementEnabled(true);
        IsPerformingAction = false;
    }

    private IEnumerator MoveToPoint(Vector3 from, Vector3 to, float speed)
    {
        var t = 0f;
        var duration = Vector3.Distance(from, to) / speed;

        while (t < duration)
        {
            t += Time.fixedDeltaTime;
            _rb.MovePosition(Vector3.Lerp(from, to, t / duration));
            yield return new WaitForFixedUpdate();
        }
        
        _rb.MovePosition(to);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
