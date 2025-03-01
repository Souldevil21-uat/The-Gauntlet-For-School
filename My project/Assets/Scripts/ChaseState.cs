using UnityEngine;

public class ChaseState : State
{
    private float chaseSpeed;

    public ChaseState(AIController ai) : base(ai)
    {
        this.chaseSpeed = ai.chaseSpeed; // ✅ Ensure AI uses correct chase speed
    }

    public override void Enter()
    {
        Debug.Log(aiController.gameObject.name + " 🚨 Entered ChaseState!");
    }

    public override void Execute(AIController ai)
    {
        Debug.Log(ai.gameObject.name + " 🔄 Running ChaseState...");

        // ✅ If AI NO LONGER SEES the player, return to patrol
        if (!ai.CanSeePlayer())
        {
            // ✅ AI should ONLY use hearing if it **loses sight**
            if (ai.CanHearPlayer())
            {
                Debug.Log(ai.gameObject.name + " 👂 HEARD the player! Keeping movement but adjusting reaction.");
                return; // ✅ AI will continue chasing using hearing, but doesn’t stop
            }

            Debug.Log(ai.gameObject.name + " ❌ Lost sight of player. Returning to patrol.");
            ai.ChangeState(new PatrolState(ai));
            return;
        }

        // ✅ Ensure AI ROTATES towards player before moving
        ai.RotateTowards(ai.player.transform.position);

        // ✅ Ensure AI MOVES at the correct chase speed
        ai.MoveTowards(ai.player.transform.position, chaseSpeed);
    }

    public override void Exit()
    {
        Debug.Log(aiController.gameObject.name + " 🛑 Exiting ChaseState.");
    }
}







