using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [Header("Tank Movement Settings")]
    public float moveSpeed = 10f;  // Speed for forward and backward movement
    public float turnSpeed = 50f;  // Speed for rotation
    public bool isPlayerOne = true; // Determines if this is Player 1 or Player 2

    private Rigidbody rb; // Reference to the tank's Rigidbody component

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Assign Rigidbody once
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        // Get movement input (Player 1 uses W/S, Player 2 uses Up/Down)
        float moveInput = isPlayerOne ? Input.GetAxis("Vertical") : Input.GetAxis("P2_Vertical");

        // Move the tank forward or backward
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z); // Preserve gravity
    }

    private void HandleRotation()
    {
        // Get rotation input (Player 1 uses A/D, Player 2 uses Left/Right)
        float turnInput = isPlayerOne ? Input.GetAxis("Horizontal") : Input.GetAxis("P2_Horizontal");

        // Rotate the tank left or right smoothly
        if (Mathf.Abs(turnInput) > 0.1f) // Prevent unnecessary calculations
        {
            Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}

