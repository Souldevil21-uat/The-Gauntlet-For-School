using UnityEngine;
using System.Collections.Generic;

public class AIPatrolChase : AIController
{
    [Header("Patrol Settings")]
    private int currentPatrolIndex = 0;

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

        // ✅ Only increment patrol index **AFTER reaching the waypoint**
        Transform nextPoint = patrolPoints[currentPatrolIndex];

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;

        Debug.Log(gameObject.name + " 🔄 Moving to next patrol point: " + nextPoint.name);
        return nextPoint;
    }

    // ✅ Added function to restart patrol from the first waypoint
    public void RestartPatrol()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogError(gameObject.name + " ❗ Cannot restart patrol: No patrol points available.");
            return;
        }

        Debug.Log(gameObject.name + " 🔄 Restarting patrol from the first waypoint.");
        currentPatrolIndex = 0;
        ChangeState(new PatrolState(this));
    }
}






