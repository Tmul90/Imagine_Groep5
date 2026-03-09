using UnityEngine;

[RequireComponent(typeof(Rigidbody))] 
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f; // Multiplies gravity when falling down
    [SerializeField] private float ascendMultiplier = 2f; // Multiplies gravity for ascending to peak of jump
    
    [Header("Camera Rotation")]
    [SerializeField] private float mouseSensitivity = 2f;
    
    // TODO move to different script that handles layers instead of player
    [SerializeField] private LayerMask groundLayer;
    
    private Rigidbody _rb;
    private float _moveHorizontal;
    private float _moveForward;
    
    private bool _isGrounded = true;
    private bool _movementEnabled = true;
    
    private float _groundCheckTimer = 0f;
    
    private float _playerHeight;
    private float _raycastDistance;

    private float _verticalRotation = 0f;
    private Transform _cameraTransform;
    
    private const float GroundCheckDelay = 0.3f;
    
    public void SetMovementEnabled(bool enabled) => _movementEnabled = enabled;

    private void Start() => 
        Init();

    private void Update()
    {
        if (!_movementEnabled) return;
        
        _moveHorizontal = Input.GetAxisRaw("Horizontal");
        _moveForward = Input.GetAxisRaw("Vertical");

        RotateCamera();

        if (Input.GetButtonDown("Jump") && _isGrounded) { HandleJump(); }

        GroundCheck();
    }

    private void FixedUpdate()
    {
        if (!_movementEnabled) return;
        
        MovePlayer();
        ApplyJumpPhysics();
    }

    private void Init()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        if (Camera.main is not null) { _cameraTransform = Camera.main.transform; }

        _playerHeight = GetComponent<CapsuleCollider>().height * transform.localScale.y;
        _raycastDistance = (_playerHeight / 2) + 0.2f;
        
        // TODO move to cursor script that flips it on and off
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    private void GroundCheck()
    {
        if (!_isGrounded && _groundCheckTimer <= 0f)
        {
            var rayOrigin = transform.position + Vector3.up * 0.1f;
            _isGrounded = Physics.Raycast(rayOrigin, Vector3.down, _raycastDistance, groundLayer);
        }
        else { _groundCheckTimer -= Time.deltaTime; }
    }

    private void MovePlayer()
    {
        var movement = (transform.right * _moveHorizontal + transform.forward * _moveForward).normalized;
        var targetVelocity = movement * moveSpeed;
        
        var velocity = _rb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;
        _rb.linearVelocity = velocity;
        
        if (_isGrounded && _moveHorizontal == 0 && _moveForward == 0) { _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0); }
    }
    
    private void RotateCamera()
    {
        var horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, horizontalRotation, 0);

        _verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -90f, 90f);

        _cameraTransform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
    }

    private void HandleJump()
    {
        _isGrounded = false;
        _groundCheckTimer = GroundCheckDelay;
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, jumpForce, _rb.linearVelocity.z);
    }

    private void ApplyJumpPhysics()
    {
        switch (_rb.linearVelocity.y)
        {
            case < 0:
                _rb.linearVelocity += Vector3.up * (Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime);
                break;
            case > 0:
                _rb.linearVelocity += Vector3.up * (Physics.gravity.y * ascendMultiplier * Time.fixedDeltaTime);
                break;
        }
    }
}