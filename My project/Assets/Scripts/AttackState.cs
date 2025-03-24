using UnityEngine;

public class AttackState : State
{
    private float attackCooldown = 1.5f; // Time between shots
    private float lostPlayerTime = 0f;   // Time since the player was last seen
    private float lostPlayerThreshold = 2f; // AI will wait 2 seconds before returning to ambush

    public AttackState(AIController ai) : base(ai) { }

    public override void Enter()
    {
        Debug.Log($"{aiController.name} entered AttackState.");
        lostPlayerTime = 0f;
    }

    public override void Execute(AIController ai)
    {
        if (ai.player == null)
        {
            Debug.LogWarning($"{ai.name} has no player reference in AttackState!");
            ai.ChangeState(new AmbushState(ai));
            return;
        }

        if (!ai.CanSeePlayer())
        {
            lostPlayerTime += Time.deltaTime;
            if (lostPlayerTime >= lostPlayerThreshold)
            {
                Debug.Log($"{ai.name} lost sight of the player. Returning to AmbushState.");
                ai.ChangeState(new AmbushState(ai));
            }
            return;
        }
        else
        {
            lostPlayerTime = 0f; // Reset lost player timer if they are visible
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

    public override void Exit()
    {
        Debug.Log($"{aiController.name} exited AttackState.");
    }
}




