using UnityEngine;
using System.Collections.Generic;

public class AIPatrolChase : AIController
{
    [Header("Patrol Settings")]
    private int currentPatrolIndex = 0;
    public bool loopPatrol = true; // Determines if AI loops waypoints or stops at the last one

    protected override void Start()
    {
        base.Start();

        // Ensure AI has patrol points before starting patrol
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            return; // No patrol points available, AI remains idle
        }

        ChangeState(new PatrolState(this)); // Start in patrol mode
    }

    // Handles state transitions
    public override void ChangeState(State newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // Retrieves the next patrol waypoint
    public Transform GetNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return null;

        // If loopPatrol is false and AI reached the last waypoint, stop patrol
        if (!loopPatrol && currentPatrolIndex >= patrolPoints.Count - 1)
        {
            return null;
        }

        Transform nextPoint = patrolPoints[currentPatrolIndex];
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count; // Loop through patrol points

        return nextPoint;
    }

    // Resets patrol behavior and moves AI to the first waypoint
    public void RestartPatrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return;

        currentPatrolIndex = 0; // Reset patrol index to start from the first waypoint
        Transform nextPoint = patrolPoints[currentPatrolIndex];

        // Move towards the first patrol point if available
        if (nextPoint != null)
        {
            MoveTowards(nextPoint.position, patrolSpeed);
        }

        ChangeState(new PatrolState(this)); // Ensure AI enters patrol state
    }
}









