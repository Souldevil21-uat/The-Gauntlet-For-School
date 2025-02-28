using UnityEngine;

public class TankPawn : Pawn
{
    [Header("Acceleration Settings")]
    [SerializeField] private float acceleration = 5f;  // How fast the tank speeds up
    [SerializeField] private float deceleration = 4f;  // How fast the tank slows down
    [SerializeField] private float maxSpeed = 10f;     // Maximum movement speed

    private float currentSpeed = 0f; // Tracks current movement speed
    [SerializeField] private new Rigidbody rb;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>(); // ✅ Ensure Rigidbody is assigned
        if (rb == null)
        {
            Debug.LogError(gameObject.name + " ❌ ERROR: Rigidbody is missing on TankPawn!");
        }
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
            Debug.Log(gameObject.name + " 🚜 Move() called with input: " + input);
            currentSpeed += acceleration * input * Time.deltaTime;
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += deceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, 0);
            }
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        Vector3 moveDirection = transform.forward * currentSpeed * Time.deltaTime;
        Debug.Log(gameObject.name + " 🏎 Moving in direction: " + moveDirection);
        rb.MovePosition(rb.position + moveDirection);
    }


    public override void Rotate(float input)
    {
        if (rb == null)
        {
            Debug.LogWarning(gameObject.name + " ❌ Cannot rotate: Rigidbody is null!");
            return;
        }

        if (input != 0)
        {
            float rotationAmount = input * rotateSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, rotationAmount, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}

