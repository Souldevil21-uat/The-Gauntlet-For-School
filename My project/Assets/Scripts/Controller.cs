using UnityEngine;

public class Controller : MonoBehaviour
{
    public Pawn pawn; // Reference to the controlled Pawn (AI or Player)

    protected virtual void Start()
    {
        // No need for debug logs in the base class
    }

    protected virtual void Update()
    {
        // This base class does not implement Update logic
    }
}

