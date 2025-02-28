using UnityEngine;

public class AIAmbush : AIController
{
    [Header("Ambush Settings")]
    public float ambushRange = 10f; // Distance at which the AI will attack

    protected override void Start()
    {
        base.Start();
        ChangeState(new AmbushState(this)); // Start in Ambush mode
    }

    public override bool CanSeePlayer()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < ambushRange) // ✅ Only detect player at close range
        {
            return base.CanSeePlayer(); // Use normal vision checks
        }
        return false;
    }

    public override void ChangeState(State newState)
    {
        if (newState is ChaseState)
        {
            Debug.LogWarning(gameObject.name + " ❌ AIAmbush should NEVER enter ChaseState!");
            return; // Prevents accidental transition
        }

        base.ChangeState(newState);
    }

}

