using UnityEngine;
using System.Collections.Generic;

public class AIPatrolChase : AIController
{
    [Header("Patrol Settings")]
    private int currentPatrolIndex = 0;
    public bool loopPatrol = true; // ✅ Allows AI to loop waypoints or stop at the last one

    protected override void Start()
    {
        base.Start();

        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogError(gameObject.name + " ❌ NO patrol points assigned at start!");
        }
        else
        {
            Debug.Log(gameObject.name + " ✅ Patrol points loaded: " + patrolPoints.Count);
            ChangeState(new PatrolState(this)); // Start in Patrol Mode
        }
    }

    public override void ChangeState(State newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public Transform GetNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning(gameObject.name + " ❗ PatrolPoints list is EMPTY. Staying idle.");
            return null;
        }

        // ✅ If loopPatrol is false and AI reached the last waypoint, transition to an idle state
        if (!loopPatrol && currentPatrolIndex >= patrolPoints.Count - 1)
        {
            Debug.Log(gameObject.name + " ⏹️ Reached last waypoint, stopping patrol.");
            return null;
        }

        Transform nextPoint = patrolPoints[currentPatrolIndex];

        // ✅ Only loop if loopPatrol is true
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;

        Debug.Log(gameObject.name + " 🔄 Moving to next patrol point: " + nextPoint.name);
        return nextPoint;
    }

    // ✅ FIX: Restart Patrol now ensures AI moves immediately
    public void RestartPatrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogError(gameObject.name + " ❗ Cannot restart patrol: No patrol points available.");
            return;
        }

        Debug.Log(gameObject.name + " 🔄 Restarting patrol from the first waypoint.");
        currentPatrolIndex = 0;

        // ✅ Immediately move towards the first patrol point
        Transform nextPoint = patrolPoints[currentPatrolIndex];
        if (nextPoint != null)
        {
            MoveTowards(nextPoint.position, patrolSpeed);
        }

        ChangeState(new PatrolState(this)); // ✅ Ensure state is properly switched
    }
}








