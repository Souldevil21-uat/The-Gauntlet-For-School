using UnityEngine;

public class AttackState : State
{
    private float attackCooldown = 1.5f; // Time between AI shots
    private float lostPlayerTime = 0f;   // Time spent without seeing the player
    private float lostPlayerThreshold = 2f; // Time before giving up and returning to ambush

    public AttackState(AIController ai) : base(ai) { }

    public override void Enter()
    {
        // Reset lost player timer when entering the state
        Debug.Log($"{aiController.name} entered AttackState.");
        lostPlayerTime = 0f;
    }

    public override void Execute(AIController ai)
    {
        // Fail-safe if player reference is missing
        if (ai.player == null)
        {
            Debug.LogWarning($"{ai.name} has no player reference in AttackState!");
            ai.ChangeState(new AmbushState(ai));
            return;
        }

        // If the AI can’t see the player, count up the lost time
        if (!ai.CanSeePlayer())
        {
            lostPlayerTime += Time.deltaTime;

            // If player hasn't been seen for a while, return to ambush
            if (lostPlayerTime >= lostPlayerThreshold)
            {
                Debug.Log($"{ai.name} lost sight of the player. Returning to AmbushState.");
                ai.ChangeState(new AmbushState(ai));
            }
            return;
        }
        else
        {
            // Reset timer since player is visible
            lostPlayerTime = 0f;
        }

        // Rotate toward the player
        ai.RotateTowards(ai.player.transform.position);

        // Only shoot if AI is mostly facing the player
        Vector3 directionToPlayer = (ai.player.transform.position - ai.transform.position).normalized;
        float dotProduct = Vector3.Dot(ai.transform.forward, directionToPlayer);

        if (dotProduct > 0.95f)
        {
            // Fire projectile if cooldown has passed
            if (Time.time >= ai.nextFireTime)
            {
                ai.FireProjectile();
                ai.nextFireTime = Time.time + attackCooldown;
            }
        }
    }

    public override void Exit()
    {
        Debug.Log($"{aiController.name} exited AttackState.");
    }
}





