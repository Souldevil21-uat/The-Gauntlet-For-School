using UnityEngine;
using System.Collections.Generic;

public class AIFlee : AIController
{
    [Header("Flee Settings")]
    public float fleeDistance = 10f; // Distance at which AI flees
    public float safeDistance = 15f; // Distance at which AI stops fleeing
    private bool isFleeing = false; // Tracks fleeing status
    private float fleeCooldown = 3f; // Cooldown to prevent constant switching

    [Header("Patrol Settings")]
    private int currentPatrolIndex = 0;

    protected override void Start()
    {
        base.Start();

        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError(gameObject.name + " could not find the Player!");
        }

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

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (Vector3.Angle(transform.forward, directionToPlayer) < fieldOfView / 2 && distanceToPlayer < detectionRange)
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

    public void StartFleeing()
    {
        if (!isFleeing)
        {
            isFleeing = true;
            ChangeState(new FleeState(this));
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

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer >= safeDistance)
        {
            RestartPatrol();
        }
    }

    public void RestartPatrol()
    {
        if (currentState is PatrolState) return;

        isFleeing = false;
        ChangeState(new PatrolState(this));
        Debug.Log(gameObject.name + " resumed patrolling.");
    }

    public override void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter();
    }

    public Transform GetNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return null;

        Transform nextPoint = patrolPoints[currentPatrolIndex];
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        return nextPoint;
    }
}















