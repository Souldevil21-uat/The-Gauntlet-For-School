using UnityEngine;
using System.Collections.Generic;

public class AIFlee : AIController
{
    [Header("Flee Settings")]
    public float fleeDistance = 10f;        // Distance at which the AI decides to flee
    public float safeDistance = 15f;        // Distance at which the AI feels safe enough to stop fleeing
    private bool isFleeing = false;         // Whether the AI is currently fleeing
    private float fleeCooldown = 3f;        // Cooldown to prevent constant state switching

    [Header("Patrol Settings")]
    private int currentPatrolIndex = 0;     // Tracks the current waypoint index for patrol

    protected override void Start()
    {
        base.Start();

        // Locate the player by tag
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError(gameObject.name + " could not find the Player!");
        }

        // Begin patrolling if patrol points are defined
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            ChangeState(new PatrolState(this));
            Debug.Log(gameObject.name + " started in PatrolState.");
        }
        else
        {
            Debug.LogWarning(gameObject.name + " has no patrol points! Staying idle.");
        }
    }

    public override bool CanSeePlayer()
    {
        if (player == null) return false;

        // Calculate direction and distance to player
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Check if player is within FOV and detection range
        if (Vector3.Angle(transform.forward, directionToPlayer) < fieldOfView / 2 && distanceToPlayer < detectionRange)
        {
            // Use raycast to confirm clear line of sight
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

    public void StartFleeing()
    {
        if (!isFleeing)
        {
            isFleeing = true;
            ChangeState(new FleeState(this));  // Switch to flee state
            Debug.Log(gameObject.name + " started fleeing!");
        }
    }

    public void CheckFleeDistance()
    {
        if (!isFleeing) return;

        if (player == null)
        {
            RestartPatrol();
            return;
        }

        // Return to patrol once far enough from player
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer >= safeDistance)
        {
            RestartPatrol();
        }
    }

    public void RestartPatrol()
    {
        // Prevent re-entering patrol if already in it
        if (currentState is PatrolState) return;

        isFleeing = false;
        ChangeState(new PatrolState(this));
        Debug.Log(gameObject.name + " resumed patrolling.");
    }

    public override void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.Exit();  // Clean up current state
        }

        currentState = newState;
        currentState.Enter();    // Start the new state
    }

    public Transform GetNextPatrolPoint()
    {
        // Return null if patrol points are missing
        if (patrolPoints == null || patrolPoints.Count == 0) return null;

        // Cycle through patrol points
        Transform nextPoint = patrolPoints[currentPatrolIndex];
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        return nextPoint;
    }
}
















