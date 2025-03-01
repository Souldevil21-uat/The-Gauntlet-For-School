using UnityEngine;
using System.Collections.Generic;

public class AIPatrol : AIController
{
    [Header("Patrol Settings")]
    public List<Transform> patrolPoints; // List of waypoints for patrolling
    public float waypointThreshold = 1f; // Distance threshold to switch waypoints

    private int currentPatrolIndex = 0;

    protected override void Start()
    {
        base.Start();
        ChangeState(new PatrolState(this)); // Initialize in patrol state
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
        if (patrolPoints.Count == 0) return null;

        Transform patrolPoint = patrolPoints[currentPatrolIndex];
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count; // Loops patrol points
        return patrolPoint;
    }
}


