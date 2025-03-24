using UnityEngine;

public class TankPawn : Pawn
{
    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 5f;  // Rate of speed increase
    [SerializeField] private float deceleration = 4f;  // Rate of speed decrease
    [SerializeField] private float maxSpeed = 10f;     // Maximum movement speed

    private float currentSpeed = 0f; // Tracks the tank's current speed
    private Rigidbody rb; // Rigidbody reference

    /// <summary>
    /// Initializes the tank pawn and registers it with the GameManager.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError($"{gameObject.name} ERROR: Rigidbody is missing on TankPawn!");
        }

        GameManager.Instance.RegisterTank(this);
    }

    /// <summary>
    /// Unregisters the tank from the GameManager when destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterTank(this);
        }
    }

    /// <summary>
    /// Handles tank movement with acceleration and deceleration.
    /// </summary>
    public override void Move(float input)
    {
        if (rb == null) return;

        if (input != 0)
        {
            currentSpeed += acceleration * input * Time.deltaTime;
        }
        else
        {
            ApplyDeceleration();
        }

        // Clamp speed to the allowed range
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Move the tank forward based on the current speed
        rb.MovePosition(rb.position + transform.forward * currentSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Applies gradual deceleration when no movement input is detected.
    /// </summary>
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

    /// <summary>
    /// Handles tank rotation based on player input.
    /// </summary>
    public override void Rotate(float input)
    {
        if (rb == null)
        {
            Debug.LogWarning($"{gameObject.name} ERROR: Cannot rotate, Rigidbody is missing!");
            return;
        }

        if (input != 0)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, input * rotateSpeed * Time.deltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}



