using UnityEngine;
using System.Collections.Generic;

public class AIPatrol : AIController
{
    public float waypointThreshold = 1f; // Distance threshold to switch waypoints
    private int currentPatrolIndex = 0;

    protected override void Start()
    {
        base.Start();

        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name} has no patrol points assigned! AI will remain idle.");
            return;
        }

        ChangeState(new PatrolState(this)); // Initialize in patrol state
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
            Debug.LogWarning($"{gameObject.name} has no patrol points to navigate to.");
            return null;
        }

        if (patrolPoints.Count == 1) return patrolPoints[0]; // Avoid unnecessary looping

        Transform patrolPoint = patrolPoints[currentPatrolIndex];
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        return patrolPoint;
    }
}




