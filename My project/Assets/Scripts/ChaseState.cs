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
        if (!ai.CanSeePlayer())
        {
            Debug.Log(ai.gameObject.name + " ❌ Lost sight of player. Returning to patrol.");
            ai.ChangeState(new PatrolState(ai));
            return;
        }

        // ✅ Rotate first before moving
        ai.RotateTowards(ai.player.transform.position);

        // ✅ Move towards the player with proper chase speed
        ai.MoveTowards(ai.player.transform.position, chaseSpeed);

        // ✅ Only shoot if AI is mostly facing the player
        Vector3 directionToPlayer = (ai.player.transform.position - ai.transform.position).normalized;
        float dotProduct = Vector3.Dot(ai.transform.forward, directionToPlayer);

        if (dotProduct > 0.9f) // AI is mostly aligned
        {
            if (Time.time >= ai.nextFireTime)
            {
                Debug.Log(ai.gameObject.name + " 🔫 AI Fires at Player!");
                ai.FireProjectile();
                ai.nextFireTime = Time.time + ai.fireRate; // Reset fire cooldown
            }
        }
    }

    public override void Exit()
    {
        Debug.Log(aiController.gameObject.name + " 🛑 Exiting ChaseState.");
    }
}






