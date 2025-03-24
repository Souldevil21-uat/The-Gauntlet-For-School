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
        player = GameObject.FindWithTag("Player"); // Finds player automatically

        // Ensure AI has patrol points before starting patrol
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning("❗ AIPatrolChase: No patrol points assigned!");
            return; // AI remains idle if no waypoints exist
        }

        ChangeState(new PatrolState(this)); // Start in patrol mode
    }

    protected override void Update()
    {
        base.Update();

        // If AI sees the player, switch to chase mode
        if (CanSeePlayer() && !(currentState is ChaseState))
        {
            ChangeState(new ChaseState(this));
        }

        // If AI loses sight of player, return to patrol after delay
        if (!CanSeePlayer() && currentState is ChaseState)
        {
            Invoke(nameof(ReturnToPatrol), 3f); // Give AI 3 seconds before returning to patrol
        }
    }

    // Handles state transitions
    public override void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter();
    }

    // Retrieves the next patrol waypoint
    public Transform GetNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return null;

        // Stops patrolling if `loopPatrol` is false and AI reached last point
        if (!loopPatrol && currentPatrolIndex >= patrolPoints.Count)
        {
            return null;
        }

        Transform nextPoint = patrolPoints[currentPatrolIndex];

        if (loopPatrol)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count; // Loop patrol
        }
        else
        {
            currentPatrolIndex++; // Move to next waypoint but do not loop
        }

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

    // Called when AI loses sight of the player
    private void ReturnToPatrol()
    {
        if (!(currentState is ChaseState)) return; // Ensure AI was previously chasing
        ChangeState(new PatrolState(this));
    }
}









