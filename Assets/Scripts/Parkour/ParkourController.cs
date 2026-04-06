using System;
using System.Collections;
using System.Linq;
using Sound;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParkourController : MonoBehaviour
{
    
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 1.8f;
    [SerializeField] private LayerMask parkourLayer;
    [SerializeField] private KeyCode parkourKeyCode = KeyCode.LeftShift;
    [SerializeField] private KeyCode dropKeyCode = KeyCode.LeftControl;
    [SerializeField] private KeyCode climbKeyCode = KeyCode.Space;
    [Header("Audio")]
    [SerializeField] private AudioClip[] parkourSounds;
    [SerializeField] private Vector2 parkourSoundVolume = new Vector2(-2f, 0.3f);
    [SerializeField] private float parkourSoundRandomPitch = 0.1f;
    
    private int _detectionFrame = 0;
    private Parkourable _currentTarget;
    private Rigidbody _rb;
    private PlayerController _playerController;
    
    private bool _hangClimb;
    private bool _hangDrop;

    public bool IsPerformingAction { get; private set; } = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerController = GetComponent<PlayerController>();
    }
    // ReSharper disable Unity.PerformanceAnalysis
    private void Update()
    {
        if (Input.GetKeyDown(dropKeyCode))
            _hangDrop  = true;
        if (Input.GetKeyDown(climbKeyCode))
            _hangClimb = true;
        
        if (IsPerformingAction) 
            return;

        if (_detectionFrame++ % 3 == 0) 
            DetectParkourable();

        SpriteAnimation.Instance.PlayAnimation(_currentTarget is null ? -1 : 1);
        
        if (_currentTarget is not null && Input.GetKey(parkourKeyCode)) 
            _currentTarget.Execute(this);
    }
    
    public void PerformVault(Vector3 overPoint, float speed)
    {
        StartCoroutine(VaultRoutine(overPoint, speed));
        StartSound();
    }

    public void PerformMantle(Vector3 ledgePoint, float speed)
    {
        StartCoroutine(MantleRoutine(ledgePoint, speed));
        StartSound();
    }    
    
    public void PerformHang(Vector3 gripPoint, Hangable surface, float hangSpeed, float traverseSpeed)
    {
        _hangDrop  = false;
        _hangClimb = false;
        StartCoroutine(HangRoutine(gripPoint, surface, hangSpeed, traverseSpeed));
        StartSound();
    }

    private void StartSound()
    {
        SoundManager.Instance.PlayRandomClip(parkourSounds, transform, Random.Range(parkourSoundVolume.x, parkourSoundVolume.y), false, parkourSoundRandomPitch);
    }
    
    private void DetectParkourable()
    {
        var best = Physics.OverlapSphere(transform.position, detectionRadius, parkourLayer)
            .Select(hit => hit.GetComponent<Parkourable>())
            .Where(parkourable => parkourable is not null && parkourable.IsEnabled)
            .Where(parkourable => Vector3.Distance(transform.position, parkourable.GetInteractionPoint(transform)) <= parkourable.InteractionRange)
            .OrderBy(parkourable => Vector3.Distance(transform.position, parkourable.GetInteractionPoint(transform)))
            .FirstOrDefault();

        if (best == _currentTarget) return;
        
        _currentTarget?.OnPlayerExitRange(this);
        _currentTarget = best;
        _currentTarget?.OnPlayerInRange(this);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private IEnumerator VaultRoutine(Vector3 overPoint, float speed)
    {
        IsPerformingAction = true;
        _playerController.SetMovementEnabled(false);
        
        var startPos = transform.position;
        var topPos = new Vector3(overPoint.x, overPoint.y + 0.1f, overPoint.z);
        
        var approachDir = (overPoint - transform.position).normalized;
        var endPos = new Vector3(
            overPoint.x + approachDir.x * 1.2f,
            overPoint.y,
            overPoint.z + approachDir.z * 1.2f
        );
        
        yield return MoveToPoint(startPos, topPos, speed);
        yield return MoveToPoint(topPos, endPos, speed * 1.5f);

        _rb.linearVelocity = Vector3.zero;
        _playerController.SetMovementEnabled(true);
        IsPerformingAction = false;
    }    
    
    private IEnumerator HangRoutine(Vector3 gripPoint, Hangable surface, float hangSpeed, float traverseSpeed)
    {
        IsPerformingAction = true;
        _playerController.SetMovementEnabled(false);
        _rb.useGravity = false;
        _rb.linearVelocity = Vector3.zero;
        
        var capsuleHeight = GetComponent<CapsuleCollider>().height * transform.localScale.y;
        var hangOffset = Vector3.down * (capsuleHeight * 0.5f);
        
        var targetPosition = gripPoint + hangOffset;
        yield return MoveToPoint(transform.position, targetPosition, hangSpeed);
        
        var pipeAxis = surface.GetPipeAxis();
        
        while (true)
        {
            var rawInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            
            var camera = Camera.main.transform;
            var cameraForward = Vector3.ProjectOnPlane(camera.forward, Vector3.up).normalized;
            var cameraRight = Vector3.ProjectOnPlane(camera.right, Vector3.up).normalized;
            var worldInput = (cameraForward * rawInput.z + cameraRight * rawInput.x);

            if (rawInput.sqrMagnitude > 0.01f)
            {
                var move = pipeAxis * Vector3.Dot(worldInput, pipeAxis) * traverseSpeed * Time.fixedDeltaTime;
                var nextGrip = surface.ClampToSurface(gripPoint + move);

                if (surface.IsAtEnd(gripPoint + move)) 
                    break;

                gripPoint = nextGrip;
                _rb.MovePosition(gripPoint + hangOffset);
            }
            
            // Drop
            if (_hangClimb)
                break;

            // Climb up
            if (_hangDrop)
            {
                yield return MoveToPoint(
                    transform.position,
                    gripPoint + Vector3.up * (capsuleHeight * 0.6f),
                    hangSpeed
                );
                
                _rb.useGravity = true;
                _rb.linearVelocity = Vector3.zero;
                _playerController.SetMovementEnabled(true);
                IsPerformingAction = false;
                
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

        _rb.useGravity = true;
        _playerController.SetMovementEnabled(true);
        IsPerformingAction = false;
    }

    private IEnumerator MantleRoutine(Vector3 ledgePoint, float speed)
    {
        IsPerformingAction = true;
        _playerController.SetMovementEnabled(false);
        _rb.linearVelocity = Vector3.zero;
        _rb.useGravity = false;
        
        var capsuleHeight = GetComponent<CapsuleCollider>().height * transform.localScale.y;
        
        var hangPos = new Vector3(transform.position.x, ledgePoint.y - capsuleHeight * 0.3f, transform.position.z);
        yield return MoveToPoint(transform.position, hangPos, speed);
        
        var directionToLedge = (ledgePoint - transform.position).normalized;
        var standPos = new Vector3(
            ledgePoint.x + directionToLedge.x * 0.3f,
            ledgePoint.y + capsuleHeight / 2f,
            ledgePoint.z + directionToLedge.z * 0.3f
        );
        
        yield return MoveToPoint(transform.position, standPos, speed * 0.8f);
        
        _rb.useGravity = true;
        _rb.linearVelocity = Vector3.zero;
        _playerController.SetMovementEnabled(true);
        IsPerformingAction = false;
    }

    private IEnumerator MoveToPoint(Vector3 from, Vector3 to, float speed)
    {
        var deltaTime = 0f;
        var duration = Vector3.Distance(from, to) / speed;

        while (deltaTime < duration)
        {
            deltaTime += Time.fixedDeltaTime;
            _rb.MovePosition(Vector3.Lerp(from, to, deltaTime / duration));
            yield return new WaitForFixedUpdate();
        }
        
        _rb.MovePosition(to);
    }
}
