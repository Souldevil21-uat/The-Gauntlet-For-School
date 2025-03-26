using UnityEngine;

public class PatrolState : State
{
    private Transform targetWaypoint;           // The current waypoint the AI is moving toward
    private bool restartingPatrol = false;      // Prevents recursive call when restarting patrol

    public PatrolState(AIController ai) : base(ai) { }

    public override void Enter()
    {
        // If this AI is a Flee-type and not already restarting, reset patrol behavior
        if (aiController is AIFlee fleeAI && !restartingPatrol)
        {
            restartingPatrol = true;
            fleeAI.RestartPatrol();             // Calls a reset on patrol behavior to avoid stuck logic
            restartingPatrol = false;
        }

        // Get the initial patrol waypoint
        GetNextWaypoint();
    }

    public override void Execute(AIController ai)
    {
        // If no current waypoint is assigned, fetch a new one
        if (targetWaypoint == null)
        {
            GetNextWaypoint();
            return;
        }

        // Move AI toward the current waypoint at its patrol speed
        ai.MoveTowards(targetWaypoint.position, ai.patrolSpeed);

        // Default tolerance for reaching a waypoint
        float tolerance = 1f;

        // Override tolerance if using a specific AI subclass
        if (aiController is AIPatrolChase patrolAI)
        {
            tolerance = patrolAI.waypointTolerance;
        }

        // If close enough to the waypoint, go to the next one
        if (Vector3.Distance(ai.transform.position, targetWaypoint.position) < tolerance)
        {
            GetNextWaypoint();
        }

        // Check if the player is visible to the AI
        CheckForPlayer(ai);
    }

    private void CheckForPlayer(AIController ai)
    {
        // If the player is seen, transition into an appropriate state
        if (ai.CanSeePlayer())
        {
            if (ai is AIPatrolChase)
            {
                ai.ChangeState(new ChaseState(ai)); // Switch to chasing behavior
            }
            else if (ai is AIFlee)
            {
                ai.ChangeState(new FleeState(ai));  // Switch to fleeing behavior
            }
        }
    }

    private void GetNextWaypoint()
    {
        // Get the next patrol point based on AI type
        if (aiController is AIPatrolChase patrolAI)
        {
            targetWaypoint = patrolAI.GetNextPatrolPoint();
        }
        else if (aiController is AIFlee fleeAI)
        {
            targetWaypoint = fleeAI.GetNextPatrolPoint();
        }

        // Log warning if no waypoints are available
        if (targetWaypoint == null)
        {
            Debug.LogWarning(aiController.name + " has no patrol points assigned.");
        }
    }

    public override void Exit()
    {
        // Optional: could include cleanup logic if needed when exiting patrol state
    }
}











