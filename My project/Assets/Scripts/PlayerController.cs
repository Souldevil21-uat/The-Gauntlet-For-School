using UnityEngine;

public class PlayerController : Controller // Inherits from Controller
{
    protected override void Start()
    {
        base.Start(); // Calls Start() from Controller

        if (pawn == null)
        {
            pawn = GetComponent<TankPawn>(); // Auto-assign if on the same GameObject
        }

        GameManager.Instance.RegisterPlayer(this); // Register this PlayerController in GameManager
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterPlayer(this); // Unregister from GameManager
        }
    }

    protected override void Update()
    {
        base.Update(); // Calls Update() from Controller

        // Get player input
        float moveInput = Input.GetAxis("Vertical"); 
        float rotateInput = Input.GetAxis("Horizontal");

        // Send input to the Pawn (Tank)
        if (pawn != null)
        {
            pawn.Move(moveInput);
            pawn.Rotate(rotateInput);
        }
    }
}
   
