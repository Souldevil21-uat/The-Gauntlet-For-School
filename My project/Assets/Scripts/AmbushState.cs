using UnityEngine;

public class AmbushState : State
{
    private float maxAmbushTime = 15f; // Maximum time the AI stays in ambush mode
    private float ambushTimer;         // Tracks how long the AI has been ambushing

    public AmbushState(AIController ai) : base(ai) { }

    public override void Enter()
    {
        ambushTimer = 0f; // Reset the ambush timer when state starts
        Debug.Log($"{aiController.name} entered AmbushState.");
    }

    public override void Execute(AIController ai)
    {
        // If player is detected, switch to AttackState
        if (ai.CanSeePlayer())
        {
            Debug.Log(ai.gameObject.name + " detected the player! Switching to AttackState.");
            ai.ChangeState(new AttackState(ai));
        }

        // Note: ambushTimer exists but is not currently used for time-based fallback
    }

    public override void Exit()
    {
        Debug.Log($"{aiController.name} exited AmbushState.");
    }
}




