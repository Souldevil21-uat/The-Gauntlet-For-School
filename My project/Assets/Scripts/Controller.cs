using UnityEngine;

/// <summary>
/// Base class for controllers managing Pawn behavior.
/// Used by PlayerController and AIController.
/// </summary>
public class Controller : MonoBehaviour
{
    public Pawn pawn; // Reference to the controlled Pawn (AI or Player)

    protected virtual void Update()
    {
        // This base class does not implement Update logic
    }

    protected virtual void Start()  // Ensures child classes can override
    {
        // Base Start logic (if needed)
    }

    protected virtual void FixedUpdate()
    {
        // Physics-based updates (if needed) for controllers
    }
}


