using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 25f;  // Movement speed
    private float runSpeed = 100f;  // Running speed
    private float usedSpeed;
    public float jumpForce = 10f;  // Jump force

    private Rigidbody rb;
    private bool isGrounded;
    private float groundCheckDistance = 0.1f;  // Distance for ground check
    private LayerMask groundLayer;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        groundLayer = LayerMask.GetMask("Ground");  // Ensure your ground objects are tagged correctly
    }

    void Update()
    {
        // Check if Left Shift is held for running
        usedSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal");  // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical");  // W/S or Up/Down

        // Calculate movement direction based on input
        Vector3 moveDirection = (transform.right * horizontal + transform.forward * vertical).normalized;

        // Apply movement to the rigidbody
        if (moveDirection.magnitude >= 0.1f)
        {
            rb.velocity = new Vector3(moveDirection.x * usedSpeed, rb.velocity.y, moveDirection.z * usedSpeed);
        }

        if (!Input.anyKey)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        // Jump when Space is pressed and the player is grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;  // Prevents jumping again mid-air
        }
    }

    // Check if the player is grounded by casting a small ray downward
    void FixedUpdate()
    {
        // Cast a ray downwards to detect if the player is grounded
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
