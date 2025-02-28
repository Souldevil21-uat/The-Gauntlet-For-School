using UnityEngine;

public class ChaseState : State
{
    private float chaseSpeed;

    public ChaseState(AIController ai) : base(ai)
    {
        this.chaseSpeed = ai.chaseSpeed; // Ensure AI uses correct chase speed
    }

    public override void Enter()
    {
        Debug.Log(aiController.gameObject.name + " 🚨 Entered ChaseState!");
    }

    public override void Execute(AIController ai)
    {
        Debug.Log(ai.gameObject.name + " 🔄 Running ChaseState...");

        if (!ai.CanSeePlayer())
        {
            Debug.Log(ai.gameObject.name + " ❌ Lost sight of player. Returning to patrol.");
            ai.ChangeState(new PatrolState(ai));
            return;
        }

        if (ai.CanHearPlayer())
        {
            Debug.Log(ai.gameObject.name + " 👂 HEARD the player! Reacting but KEEPING MOVEMENT.");
            // AI can react, but should NOT stop movement
        }


        ai.RotateTowards(ai.player.transform.position);
        ai.MoveTowards(ai.player.transform.position);
    }


    public override void Exit()
    {
        Debug.Log(aiController.gameObject.name + " 🛑 Exiting ChaseState.");
    }
}






