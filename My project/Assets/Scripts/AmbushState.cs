using UnityEngine;

public class AmbushState : State
{
    public AmbushState(AIController ai) : base(ai) { }

    public override void Enter()
    {
        Debug.Log(aiController.gameObject.name + " 🏕️ Entered AmbushState (Idle, Waiting for Player).");
    }

    public override void Execute(AIController ai)
    {
        if (ai.CanSeePlayer()) // ✅ Player gets close, switch to attack mode
        {
            Debug.Log(ai.gameObject.name + " 🚨 Player detected! Switching to AttackState!");
            ai.ChangeState(new AttackState(ai));
        }
    }

    public override void Exit()
    {
        Debug.Log(aiController.gameObject.name + " 🛑 Exiting AmbushState.");
    }
}

