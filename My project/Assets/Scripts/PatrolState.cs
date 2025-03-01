using UnityEngine;

public class PatrolState : State
{
    private Transform targetWaypoint;
    private bool restartingPatrol = false; // Prevents infinite recursion

    public PatrolState(AIController ai) : base(ai) { }

    public override void Enter()
    {
        // Ensure AI enters patrol mode properly
        if (aiController is AIFlee fleeAI && !restartingPatrol)
        {
            restartingPatrol = true;
            fleeAI.RestartPatrol();  // Prevents recursion issues
            restartingPatrol = false;
        }

        // Assign the first patrol waypoint
        GetNextWaypoint();
    }

    public override void Execute(AIController ai)
    {
        // If no waypoint exists, AI remains idle
        if (targetWaypoint == null) return;

        // Move AI towards the patrol waypoint
        ai.MoveTowards(targetWaypoint.position, ai.patrolSpeed);

        // Check if AI reached the waypoint
        if (Vector3.Distance(ai.transform.position, targetWaypoint.position) < 1f)
        {
            // Fetch the next patrol point
            GetNextWaypoint();
        }

        // If AI sees the player, switch state
        if (ai.CanSeePlayer())
        {
            if (ai is AIPatrolChase)
            {
                ai.ChangeState(new ChaseState(ai));
            }
            else if (ai is AIFlee)
            {
                ai.ChangeState(new FleeState(ai));
            }
        }
    }

    private void GetNextWaypoint()
    {
        if (aiController is AIPatrolChase patrolAI)
        {
            targetWaypoint = patrolAI.GetNextPatrolPoint();
        }
        else if (aiController is AIFlee fleeAI)
        {
            targetWaypoint = fleeAI.GetNextPatrolPoint();
        }
    }

    public override void Exit() { }
}









