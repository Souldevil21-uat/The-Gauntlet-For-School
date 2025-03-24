using UnityEngine;

public class Pawn : MonoBehaviour
{
    [Header("Pawn Settings")]
    public float moveSpeed = 10f;   // Default movement speed
    public float rotateSpeed = 50f; // Default rotation speed

    protected Rigidbody rb; // Reference to Rigidbody component

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.isKinematic = false; // Ensures AI moves properly
        }
    }

    /// Rotates the Pawn towards a target position smoothly.
    public void RotateTowards(Vector3 targetPosition)
    {
        if (rb == null) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction == Vector3.zero) return; // Prevents rotation errors

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, rotateSpeed * Time.deltaTime));
    }

    /// Moves the Pawn forward or backward based on input.
    public virtual void Move(float input)
    {
        if (rb == null)
        {
            Debug.LogError(gameObject.name + " ERROR: Rigidbody is missing!");
            return;
        }

        Vector3 moveDirection = transform.forward * input * moveSpeed * Time.deltaTime; // 🔥 **Fix: Apply `moveSpeed`**

        rb.MovePosition(rb.position + moveDirection);

        Debug.Log(gameObject.name + " moving with speed: " + input * moveSpeed);
    }

    /// Rotates the Pawn left or right based on input.
    public virtual void Rotate(float input)
    {
        if (rb == null) return;

        float rotationAmount = input * rotateSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotationAmount, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}




