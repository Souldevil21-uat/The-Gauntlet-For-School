using UnityEngine;
using System.Collections.Generic;

public class AIFlee : AIController
{
    [Header("Flee Settings")]
    public float fleeDistance = 10f; // Distance at which AI flees
    public float safeDistance = 15f; // Distance at which AI stops fleeing

    [Header("Patrol Settings")]
    private int currentPatrolIndex = 0;
    private bool isFleeing = false; // Tracks whether AI is currently fleeing

    protected override void Start()
    {
        base.Start();

        // Ensure AI starts in PatrolState if patrol points exist
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            ChangeState(new PatrolState(this));
        }
    }

    // Restarts patrol when AI reaches a safe distance
    public void RestartPatrol()
    {
        if (currentState is PatrolState) return; // Prevents unnecessary state switching

        isFleeing = false; // Reset fleeing flag
        ChangeState(new PatrolState(this));
    }

    // Checks if the AI can see the player and initiates fleeing if necessary
    public override bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (angleToPlayer < fieldOfView / 2 && distanceToPlayer < detectionRange)
        {
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRange))
            {
                if (hit.collider.gameObject == player)
                {
                    StartFleeing();
                    return true;
                }
            }
        }
        return false;
    }

    // Initiates fleeing state
    public void StartFleeing()
    {
        if (!isFleeing)
        {
            isFleeing = true;
            ChangeState(new FleeState(this));
        }
    }

    // Checks if AI has reached a safe distance and can return to patrol
    public void CheckFleeDistance()
    {
        if (!isFleeing) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer >= safeDistance)
        {
            RestartPatrol();
        }
    }

    // Handles AI state transitions
    public override void ChangeState(State newState)
    {
        if (currentState != null && currentState.GetType() == newState.GetType()) return;

        if (newState is PatrolState && isFleeing) return;

        if (newState is PatrolState)
        {
            isFleeing = false;
        }

        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter();
    }

    // Retrieves the next patrol point for the AI
    public Transform GetNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return null;

        Transform nextPoint = patrolPoints[currentPatrolIndex];
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;

        return nextPoint;
    }
}












