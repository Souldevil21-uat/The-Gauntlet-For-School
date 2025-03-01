using UnityEngine;

public class AttackState : State
{
    private float attackCooldown = 1.5f; // Time between shots

    public AttackState(AIController ai) : base(ai) { }

    // Called when the AI enters Attack mode
    public override void Enter()
    {
        // AI prepares to fire at the player
    }

    // Continuously executes attack logic while AI is in Attack mode
    public override void Execute(AIController ai)
    {
        // If the player is no longer visible, return to Ambush mode
        if (!ai.CanSeePlayer())
        {
            ai.ChangeState(new AmbushState(ai));
            return;
        }

        // Rotate towards the player before firing
        ai.RotateTowards(ai.player.transform.position);

        // Ensure AI is mostly facing the player before firing
        Vector3 directionToPlayer = (ai.player.transform.position - ai.transform.position).normalized;
        float dotProduct = Vector3.Dot(ai.transform.forward, directionToPlayer);

        if (dotProduct > 0.95f) // AI is nearly facing the player
        {
            if (Time.time >= ai.nextFireTime)
            {
                ai.FireProjectile();
                ai.nextFireTime = Time.time + attackCooldown;
            }
        }
    }

    // Called when the AI exits Attack mode
    public override void Exit()
    {
        // No additional exit behavior required
    }
}


