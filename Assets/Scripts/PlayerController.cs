using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float MoveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f; // Multiplies gravity when falling down
    [SerializeField] private float ascendMultiplier = 2f; // Multiplies gravity for ascending to peak of jump
    [SerializeField] private Area jumpArea;

    [Header("Camera Rotation")]
    [SerializeField] private float mouseSensitivity = 2f;

    // TODO move to different script that handles layers instead of player
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private float moveHorizontal;
    private float moveForward;

    private bool isGrounded = true;

    private float groundCheckTimer = 0f;
    private float groundCheckDelay = 0.3f;

    private Vector2 _cameraRotation = Vector2.zero;
    private Transform _cameraTransform;

    private void Start() =>
        Init();

    private void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveForward = Input.GetAxisRaw("Vertical");

        RotateCamera();

        if (Input.GetButtonDown("Jump") && isGrounded)
            HandleJump();

        GroundCheck();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplyJumpPhysics();
    }

    private void Init()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (Camera.main is not null) { _cameraTransform = Camera.main.transform; }

        // TODO move to cursor script that flips it on and off
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void GroundCheck()
    {
        if (!isGrounded && groundCheckTimer <= 0f)
        {
            isGrounded = jumpArea.collide;
        }
        else groundCheckTimer -= Time.deltaTime;
    }

    private void MovePlayer()
    {
        var movement = (transform.right * moveHorizontal + transform.forward * moveForward).normalized;
        var targetVelocity = movement * MoveSpeed;

        var velocity = rb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;
        rb.linearVelocity = velocity;

        if (isGrounded && moveHorizontal == 0 && moveForward == 0)
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    private void RotateCamera()
    {
        var horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        var verticalRotation = Input.GetAxis("Mouse Y") * mouseSensitivity;

        _cameraRotation += new Vector2(horizontalRotation, -verticalRotation);
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y, -90f, 90f);

        transform.eulerAngles = new Vector3(0, _cameraRotation.x, 0);
        _cameraTransform.eulerAngles = new Vector3(_cameraRotation.y, _cameraRotation.x, 0);
    }

    private void HandleJump()
    {
        isGrounded = false;
        groundCheckTimer = groundCheckDelay;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    private void ApplyJumpPhysics()
    {
        switch (rb.linearVelocity.y)
        {
            case < 0:
                rb.linearVelocity += Vector3.up * (Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime);
                break;
            case > 0:
                rb.linearVelocity += Vector3.up * (Physics.gravity.y * ascendMultiplier * Time.fixedDeltaTime);
                break;
        }
    }
}