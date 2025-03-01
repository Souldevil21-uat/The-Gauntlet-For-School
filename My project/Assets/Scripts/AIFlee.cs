using UnityEngine;
using System.Collections.Generic;

public class AIFlee : AIController
{
    [Header("Flee Settings")]
    public float fleeDistance = 10f; // Distance at which AI flees
    public float safeDistance = 15f; // Distance at which AI stops fleeing

    [Header("Patrol Settings")]
    private int currentPatrolIndex = 0;
    private bool isFleeing = false; // ✅ Track whether AI is currently fleeing

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

        // ✅ Ensure AI starts in PatrolState if patrol points exist
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            ChangeState(new PatrolState(this));
        }
        else
        {
            Debug.LogWarning(gameObject.name + " ❗ No patrol points available. Staying idle.");
        }
    }

    public void RestartPatrol()
    {
        if (currentState is PatrolState)
        {
            Debug.Log(gameObject.name + " 🔄 Already in PatrolState, no need to restart.");
            return; // ✅ Prevents infinite loop
        }

        Debug.Log(gameObject.name + " 🔄 Restarting patrol...");
        isFleeing = false; // ✅ Reset fleeing flag when returning to patrol
        ChangeState(new PatrolState(this));
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

        if (angleToPlayer < fieldOfView / 2 && distanceToPlayer < detectionRange)
        {
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, detectionRange))
            {
                if (hit.collider.gameObject == player)
                {
                    Debug.Log(gameObject.name + " 🚨 Sees player! **Entering FleeState!**");
                    StartFleeing();  // ✅ Ensures AI enters FleeState
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
            isFleeing = true; // ✅ Track fleeing state
            ChangeState(new FleeState(this));
        }
    }

    public void CheckFleeDistance()
    {
        if (!isFleeing) return; // ✅ Only check if actually fleeing

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer >= safeDistance)
        {
            Debug.Log(gameObject.name + " ✅ Safe distance reached. Returning to patrol.");
            RestartPatrol();
        }
    }

    public override void ChangeState(State newState)
    {
        // ✅ Prevent unnecessary state switching
        if (currentState != null && currentState.GetType() == newState.GetType())
        {
            return;
        }

        if (newState is PatrolState && isFleeing)
        {
            Debug.Log(gameObject.name + " ❌ Cannot switch to PatrolState yet, still fleeing.");
            return;
        }

        if (newState is PatrolState)
        {
            isFleeing = false; // ✅ Reset fleeing flag when returning to patrol
        }

        if (currentState != null)
        {
            currentState.Exit();
        }

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

        // ✅ Ensure AI cycles through waypoints properly
        Transform nextPoint = patrolPoints[currentPatrolIndex];
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;

        Debug.Log(gameObject.name + " 🔄 Moving to next patrol point: " + nextPoint.name);
        return nextPoint;
    }
}











