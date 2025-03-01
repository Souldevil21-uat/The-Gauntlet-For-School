using UnityEngine;

public class TankMovement : MonoBehaviour
{
    [Header("Tank Movement Settings")]
    public float moveSpeed = 10f; // Speed for forward and backward movement
    public float turnSpeed = 50f; // Speed for left and right rotation

    private Rigidbody rb; // Reference to the tank's Rigidbody component

    void Start()
    {
        // Get the Rigidbody component attached to the tank
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Get movement input from the player (W/S or Up/Down Arrow keys)
        float moveInput = Input.GetAxis("Vertical");

        // Get rotation input from the player (A/D or Left/Right Arrow keys)
        float turnInput = Input.GetAxis("Horizontal");

        // Move the tank forward or backward
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);

        // Rotate the tank left or right
        Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}
