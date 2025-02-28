using UnityEngine;

public class Controller : MonoBehaviour
{
    public Pawn pawn; // Generic reference to any Pawn

    protected virtual void Start()
    {
        Debug.Log("Controller Base Class Initialized.");
    }

    protected virtual void Update()
    {
        
    }
}
