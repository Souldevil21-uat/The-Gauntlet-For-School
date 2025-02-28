using UnityEngine;

public class AttackState : State
{
    private float attackCooldown = 1.5f; // Time between shots

    public AttackState(AIController ai) : base(ai) { }

    public override void Enter()
    {
        Debug.Log(aiController.gameObject.name + " 🔫 Entered AttackState (Firing at Player)!");
    }

    public override void Execute(AIController ai)
    {
        if (!ai.CanSeePlayer()) // ✅ If player leaves, return to ambush mode
        {
            Debug.Log(ai.gameObject.name + " ❌ Lost sight of player. Returning to AmbushState.");
            ai.ChangeState(new AmbushState(ai));
            return;
        }

        // ✅ Rotate towards the player before shooting
        ai.RotateTowards(ai.player.transform.position);

        // ✅ Ensure AI is mostly facing the player before firing
        Vector3 directionToPlayer = (ai.player.transform.position - ai.transform.position).normalized;
        float dotProduct = Vector3.Dot(ai.transform.forward, directionToPlayer);

        if (dotProduct > 0.95f) // AI is almost perfectly facing the player
        {
            if (Time.time >= ai.nextFireTime)
            {
                ai.FireProjectile();
                ai.nextFireTime = Time.time + attackCooldown;
            }
        }
        else
        {
            Debug.Log(ai.gameObject.name + " 🔄 Rotating towards player before firing.");
        }
    }

    public override void Exit()
    {
        Debug.Log(aiController.gameObject.name + " 🛑 Exiting AttackState.");
    }
}


