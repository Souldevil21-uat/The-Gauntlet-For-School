using UnityEngine;
using System.Collections.Generic;

public class AIPatrol : AIController
{
    [Header("Patrol Settings")]
    public List<Transform> patrolPoints; // Waypoints
    public float waypointThreshold = 1f; // Distance to switch waypoints

    private int currentPatrolIndex = 0;

    protected override void Start()
    {
        base.Start();
        ChangeState(new PatrolState(this));
    }

    public override void ChangeState(State newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public Transform GetNextPatrolPoint()
    {
        if (patrolPoints.Count == 0) return null;

        Transform patrolPoint = patrolPoints[currentPatrolIndex];
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count; // Loop patrol
        return patrolPoint;
    }
}

