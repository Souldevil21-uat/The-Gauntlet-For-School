using UnityEngine;

public class AmbushState : State
{
    public AmbushState(AIController ai) : base(ai) { }

    // Called when the AI enters Ambush mode
    public override void Enter()
    {
        // AI remains idle, waiting for the player to get close
    }

    // Continuously checks if the player is within detection range
    public override void Execute(AIController ai)
    {
        if (ai.CanSeePlayer())
        {
            // If the player is detected, transition to AttackState
            ai.ChangeState(new AttackState(ai));
        }
    }

    // Called when the AI exits Ambush mode
    public override void Exit()
    {
        // No additional exit behavior required
    }
}


