using UnityEngine;

public class PatrolState : State
{
    private Transform targetWaypoint;

    public PatrolState(AIController ai) : base(ai) { }

    public override void Enter()
    {
        Debug.Log(aiController.gameObject.name + " ▶ Entered PatrolState.");

        // ✅ Immediately assign the first patrol waypoint
        GetNextWaypoint();

        if (targetWaypoint == null)
        {
            Debug.LogWarning(aiController.gameObject.name + " ❗ No valid waypoints available. Restarting patrol...");
            RestartPatrol();
        }
    }

    public override void Execute(AIController ai)
    {
        Debug.Log(ai.gameObject.name + " 🔄 Executing PatrolState...");

        if (targetWaypoint == null)
        {
            Debug.LogWarning(ai.gameObject.name + " ❗ No valid waypoint, staying idle.");
            return;
        }

        // ✅ Move AI towards the patrol waypoint
        ai.MoveTowards(targetWaypoint.position);
        Debug.Log(ai.gameObject.name + " 🚜 Moving towards: " + targetWaypoint.position);

        // ✅ Check if AI reached the waypoint
        if (Vector3.Distance(ai.transform.position, targetWaypoint.position) < 1f)
        {
            Debug.Log(ai.gameObject.name + " 🎯 Reached waypoint: " + targetWaypoint.name);

            // ✅ Fetch the next patrol point
            GetNextWaypoint();
        }

        // ✅ If AI sees the player, switch state
        if (ai.CanSeePlayer())
        {
            Debug.Log(ai.gameObject.name + " 🚨 Player spotted!");

            if (ai is AIPatrolChase)
            {
                Debug.Log(ai.gameObject.name + " ⚠️ Switching to ChaseState!");
                ai.ChangeState(new ChaseState(ai));
                return;
            }
            else if (ai is AIFlee)
            {
                Debug.Log(ai.gameObject.name + " ⚠️ Switching to FleeState!");
                ai.ChangeState(new FleeState(ai));
                return;
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

        if (targetWaypoint != null)
        {
            Debug.Log(aiController.gameObject.name + " ✅ Next patrol waypoint set: " + targetWaypoint.name);
        }
        else
        {
            Debug.LogWarning(aiController.gameObject.name + " ❗ Patrol points missing, attempting restart...");
            RestartPatrol();
        }
    }

    private void RestartPatrol()
    {
        if (aiController is AIPatrolChase patrolAI)
        {
            patrolAI.RestartPatrol();
        }
        else if (aiController is AIFlee fleeAI)
        {
            fleeAI.RestartPatrol();
        }
    }

    public override void Exit()
    {
        Debug.Log(aiController.gameObject.name + " 🟡 Exiting PatrolState.");
    }
}








