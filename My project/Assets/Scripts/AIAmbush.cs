using UnityEngine;

public class AIAmbush : AIController
{
    [Header("Ambush Settings")]
    public float ambushRange = 10f; // Distance at which the AI will attack

    protected override void Start()
    {
        base.Start();
        ChangeState(new AmbushState(this)); // AI starts in Ambush mode
    }

    // Checks if the player is within ambush range and visible
    public override bool CanSeePlayer()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Only detect player within the ambush range
        if (distanceToPlayer < ambushRange)
        {
            return base.CanSeePlayer(); // Uses the AI's normal vision system
        }
        return false;
    }

    // Prevents the Ambush AI from switching to ChaseState
    public override void ChangeState(State newState)
    {
        if (newState is ChaseState)
        {
            return; // Ensures that Ambush AI does not chase the player
        }

        base.ChangeState(newState);
    }
}

