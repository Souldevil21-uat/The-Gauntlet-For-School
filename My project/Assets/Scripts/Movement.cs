using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public float moveSpeed = 10f; // Speed of forward and backward movement
    public float turnSpeed = 50f; // Speed of turning left and right
    private Rigidbody rb; // Reference to the tank's Rigidbody component

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the tank
    }

    void FixedUpdate()
    {
        // Get input from the keyboard
        float moveInput = Input.GetAxis("Vertical"); // W/S or Up/Down Arrow keys
        float turnInput = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow keys

        // Move the tank forward or backward
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);

        // Rotate the tank left or right
        Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}