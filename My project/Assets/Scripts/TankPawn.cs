using UnityEngine;

public class TankPawn : Pawn
{
    [Header("Acceleration Settings")]
    [SerializeField] private float acceleration = 5f;  // How fast the tank speeds up
    [SerializeField] private float deceleration = 4f;  // How fast the tank slows down
    [SerializeField] private float maxSpeed = 10f;     // Maximum movement speed

    private float currentSpeed = 0f; // Tracks current movement speed

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.RegisterTank(this);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterTank(this);
        }
    }

    public override void Move(float input)
    {
        if (input != 0)
        {
            // Apply acceleration based on input direction
            currentSpeed += acceleration * input * Time.deltaTime;
        }
        else
        {
            // Apply deceleration when no input is given
            if (currentSpeed > 0)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0); // Prevents reversing when stopping
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += deceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, 0); // Prevents forward movement when stopping
            }
        }

        // Clamp the speed within the maxSpeed range
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Move the tank forward based on currentSpeed
        Vector3 moveDirection = transform.forward * currentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + moveDirection);
    }

    public override void Rotate(float input)
    {
        if (input != 0)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, input * rotateSpeed * Time.deltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}
