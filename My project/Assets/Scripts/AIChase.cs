using UnityEngine;

public class AIChase : AIController
{
    protected override void Start()
    {
        base.Start();
        ChangeState(new ChaseState(this)); // AI starts in Chase mode
    }

    // Checks if the AI can see the player
    public override bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Ensure the player is within detection range and field of view
        if (distanceToPlayer < detectionRange && angleToPlayer < fieldOfView / 2)
        {
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRange))
            {
                return hit.collider.gameObject == player;
            }
        }
        return false;
    }

    // Changes the AI state
    public override void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        base.ChangeState(newState);
    }
}





