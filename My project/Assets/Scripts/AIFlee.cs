using UnityEngine;
using System.Collections.Generic;

public class AIFlee : AIController
{
    [Header("Flee Settings")]
    public float fleeDistance = 10f; // Distance at which AI flees
    public float safeDistance = 15f; // Distance at which AI stops fleeing

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
        }

        // ✅ Ensure AI always starts in PatrolState
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            ChangeState(new PatrolState(this));
        }
        else
        {
            Debug.LogWarning(gameObject.name + " ❗ No patrol points available. Staying idle.");
        }
    }

    public override bool CanSeePlayer()
    {
        if (player == null)
        {
            Debug.LogWarning(gameObject.name + " ❗ Cannot see player: Player is null.");
            return false;
        }

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // ✅ More readable logs
        Debug.Log(gameObject.name + $" 🔍 Checking vision: Distance={distanceToPlayer:F2}, Angle={angleToPlayer:F2}");

        if (angleToPlayer < fieldOfView / 2)
        {
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRange))
            {
                Debug.Log(gameObject.name + " 🔵 Raycast hit: " + hit.collider.gameObject.name);

                if (hit.collider.gameObject == player)
                {
                    Debug.Log(gameObject.name + " 👀 Sees the player!");
                    return true;
                }
                else
                {
                    Debug.LogWarning(gameObject.name + " ❌ Raycast hit " + hit.collider.gameObject.name + " instead of player.");
                }
            }
            else
            {
                Debug.LogWarning(gameObject.name + " ❌ Raycast did not hit anything.");
            }
        }
        else
        {
            Debug.Log(gameObject.name + " ❌ Player is outside FOV.");
        }

        return false;
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

        Transform nextPoint = patrolPoints[currentPatrolIndex];

        // ✅ Only increment the index **AFTER** reaching the waypoint
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;

        Debug.Log(gameObject.name + " 🔄 Moving to next patrol point: " + nextPoint.name);
        return nextPoint;
    }
}









