using UnityEngine;

public class TankPawn : Pawn
{
    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 5f;  // How quickly the tank speeds up
    [SerializeField] private float deceleration = 4f;  // How quickly the tank slows down
    [SerializeField] private float maxSpeed = 10f;     // Maximum forward/backward speed

    private float currentSpeed = 0f; // Tracks how fast the tank is currently moving

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"{gameObject.name} ERROR: Rigidbody is missing on TankPawn!");
        }

        // Register this tank with the GameManager for tracking
        GameManager.Instance.RegisterTank(this);
    }

    public void ResetMovement()
    {
        currentSpeed = 0f;
    }


    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            // Unregister this tank when it is destroyed
            GameManager.Instance.UnregisterTank(this);
        }
    }

    public override void Move(float input)
    {

        Debug.DrawRay(rb.position, transform.forward * 2f, Color.red, 0.1f);
        Debug.Log("Moving with speed: " + currentSpeed + " and direction: " + transform.forward);

        if (rb == null) return;

        // Accelerate or decelerate based on input
        if (input != 0)
        {
            currentSpeed += acceleration * input * Time.deltaTime;
        }
        else
        {
            ApplyDeceleration(); // Slow down smoothly when no input
        }

        // Clamp the speed within allowed limits
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Move the tank using Rigidbody for physics interaction
        rb.MovePosition(rb.position + transform.forward * currentSpeed * Time.deltaTime);
    }

    private void ApplyDeceleration()
    {
        if (currentSpeed > 0)
        {
            currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.deltaTime, 0);
        }
        else if (currentSpeed < 0)
        {
            currentSpeed = Mathf.Min(currentSpeed + deceleration * Time.deltaTime, 0);
        }
    }

    public void ResetPhysics()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }


    public override void Rotate(float input)
    {
        if (rb == null)
        {
            Debug.LogWarning($"{gameObject.name} ERROR: Cannot rotate, Rigidbody is missing!");
            return;
        }

        // Always zero angular velocity to prevent spinning
        rb.angularVelocity = Vector3.zero;

        if (input != 0)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, input * rotateSpeed * Time.deltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }



    public void MoveInDirection(Vector3 direction, float speed)
    {
        if (rb == null) return;

        // Ensure movement stays on the XZ plane
        direction.y = 0;
        direction.Normalize();

        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

    }


}




