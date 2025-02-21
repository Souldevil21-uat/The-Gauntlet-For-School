using UnityEngine;

public class Pawn : MonoBehaviour
{
    [Header("Pawn Settings")]
    public float moveSpeed = 10f;  // Default movement speed
    public float rotateSpeed = 50f; // Default rotation speed

    protected Rigidbody rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get Rigidbody component
    }

    public virtual void Move(float input)
    {
        Debug.Log("Move() called in base Pawn class.");
    }

    public virtual void Rotate(float input)
    {
        Debug.Log("Rotate() called in base Pawn class.");
    }
}
