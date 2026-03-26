using System;
using UnityEngine;
using Util;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(ParkourController))]
[RequireComponent(typeof(JumpManager))]
public class PlayerController : Singleton<PlayerController>
{
    [Header("Spawnpoint")]
    // Flip access to only have to get the spawnpoint never let anything set a value of the player like this
    // The player should always be the one requesting information
    // What if a gnome decides to change the spawnpoint they can now and we dont want that
    public Vector3 spawnPoint;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f; // Multiplies gravity when falling down
    [SerializeField] private float ascendMultiplier = 2f; // Multiplies gravity for ascending to peak of jump
    
    // TODO move to different script that handles layers instead of player
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody _rb;
    private float _moveHorizontal;
    private float _moveForward;

    private bool _isGrounded = true;
    private float _groundCheckTimer = 0f;

    private JumpManager jumpManager { get; set; }

    private const float GroundCheckDelay = 0.3f;

    private bool _movementEnabled = true;
    
    private Collider _collider;
    
    private void Start() =>
        Init();

    private void Update()
    {
        if (!_movementEnabled) return;
        
        _moveHorizontal = Input.GetAxisRaw("Horizontal");
        _moveForward = Input.GetAxisRaw("Vertical");
        
        _collider = GetComponent<Collider>();
        
        if (Input.GetButtonDown("Jump") && _isGrounded)
            HandleJump();
        
        MovePlayer();
        GroundCheck();
        ApplyJumpPhysics();
        
        
        // DEBUG
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
    }

    internal float GetHeight()
        { return _collider.bounds.min.y; }

    internal void SetMovementEnabled(bool enabled) => _movementEnabled = enabled; 

    private void Init()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        
        jumpManager = GetComponent<JumpManager>();
        jumpManager.groundLayer = groundLayer;

        StimulationManager.OnRespawn += Respawn;

        // TODO move to cursor script that flips it on and off
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        spawnPoint = transform.position;
    }

    private void MovePlayer()
    {
        var movement = (transform.right * _moveHorizontal + transform.forward * _moveForward).normalized;
        
        
        var speed = Input.GetKey(runKey) ? runSpeed : moveSpeed;
        var targetVelocity = movement * speed;

        var velocity = _rb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;
        _rb.linearVelocity = velocity;

        if (_isGrounded && _moveHorizontal == 0 && _moveForward == 0)
            _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
    }
    

    
    private void HandleJump()
    {
        _isGrounded = false;
        _groundCheckTimer = GroundCheckDelay;
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, jumpForce, _rb.linearVelocity.z);
    }
    
    private void GroundCheck()
    {
        if (!_isGrounded && _groundCheckTimer <= 0f)
        {
            _isGrounded = jumpManager.canCollide;
        }
        else _groundCheckTimer -= Time.deltaTime;
    }
    
    private void ApplyJumpPhysics()
    {
        switch (_rb.linearVelocity.y)
        {
            case < 0:
                _rb.linearVelocity += Vector3.up * (Physics.gravity.y * fallMultiplier * Time.deltaTime);
                break;
            case > 0:
                _rb.linearVelocity += Vector3.up * (Physics.gravity.y * ascendMultiplier * Time.deltaTime);
                break;
        }
    }
    
    private void Respawn()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.position = spawnPoint;
        _rb.rotation = Quaternion.identity;
    }
}