using UnityEngine;

public class Pawn : MonoBehaviour
{
    [Header("Pawn Settings")]
    public float moveSpeed = 10f;   // Default movement speed
    public float rotateSpeed = 50f; // Default rotation speed

    protected Rigidbody rb; // Reference to Rigidbody component

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get Rigidbody component on start
    }

    /// Rotates the Pawn towards a target position smoothly.
    /// <param name="targetPosition">The world position to rotate towards.</param>
    public void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, rotateSpeed * Time.deltaTime));
    }

    /// Placeholder function for movement. To be overridden in child classes.
    /// <param name="input">Movement input value.</param>
    public virtual void Move(float input)
    {
        // To be implemented in subclasses
    }
    /// Placeholder function for rotation. To be overridden in child classes.
    /// <param name="input">Rotation input value.</param>
    public virtual void Rotate(float input)
    {
        // To be implemented in subclasses
    }
}

