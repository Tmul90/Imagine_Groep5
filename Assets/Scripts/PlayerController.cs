using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float MoveSpeed = 5f;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fallMultiplier = 2.5f; // Multiplies gravity when falling down
    [SerializeField] private float ascendMultiplier = 2f; // Multiplies gravity for ascending to peak of jump
    
    [Header("Camera Rotation")]
    
    // TODO move to different script that handles layers instead of player
    [SerializeField] private LayerMask groundLayer;
    
    // Ground Movement
    private Rigidbody rb;
    private float moveHorizontal;
    private float moveForward;

    // Jumping
    private bool isGrounded = true;
    
    private float groundCheckTimer = 0f;
    private float groundCheckDelay = 0.3f;
    private float playerHeight;
    private float raycastDistance;

    private void Start() => 
        Init();

    private void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveForward = Input.GetAxisRaw("Vertical");
        

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
        
        playerHeight = GetComponent<CapsuleCollider>().height * transform.localScale.y;
        raycastDistance = (playerHeight / 2) + 0.2f;
        
        // TODO move to cursor script that flips it on and off
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void GroundCheck()
    {
        if (!isGrounded && groundCheckTimer <= 0f)
        {
            var rayOrigin = transform.position + Vector3.up * 0.1f;
            isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);
        }
        else groundCheckTimer -= Time.deltaTime;
    }

    private void MovePlayer()
    {
        var movement = (transform.right * moveHorizontal + transform.forward * moveForward).normalized;
        var targetVelocity = movement * MoveSpeed;
        
        var velocity = rb.velocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;
        rb.velocity = velocity;
        
        if (isGrounded && moveHorizontal == 0 && moveForward == 0)
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }

    private void HandleJump()
    {
        isGrounded = false;
        groundCheckTimer = groundCheckDelay;
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    private void ApplyJumpPhysics()
    {
        switch (rb.velocity.y)
        {
            case < 0:
                rb.velocity += Vector3.up * (Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime);
                break;
            case > 0:
                rb.velocity += Vector3.up * (Physics.gravity.y * ascendMultiplier * Time.fixedDeltaTime);
                break;
        }
    }
}