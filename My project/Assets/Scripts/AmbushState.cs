using UnityEngine;

public class AmbushState : State
{
    private float maxAmbushTime = 15f; // Max time in ambush before switching to patrol
    private float ambushTimer;

    public AmbushState(AIController ai) : base(ai) { }

    public override void Enter()
    {
        ambushTimer = 0f;
        Debug.Log($"{aiController.name} entered AmbushState.");
    }

    public override void Execute(AIController ai)
    {
        if (ai.CanSeePlayer())
        {
            Debug.Log(ai.gameObject.name + " detected the player! Switching to AttackState.");
            ai.ChangeState(new AttackState(ai)); // Switch to attacking
        }
    }

    public override void Exit()
    {
        Debug.Log($"{aiController.name} exited AmbushState.");
    }
}



