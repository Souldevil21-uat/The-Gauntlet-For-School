using UnityEngine;

public class PatrolState : State
{
    private Transform targetWaypoint;
    private bool restartingPatrol = false; // ✅ Prevent infinite recursion

    public PatrolState(AIController ai) : base(ai) { }

    public override void Enter()
    {
        Debug.Log(aiController.gameObject.name + " ▶ Entered PatrolState.");

        if (aiController is AIFlee fleeAI && !restartingPatrol)
        {
            restartingPatrol = true;
            fleeAI.RestartPatrol();  // ✅ Only restart patrol ONCE to avoid recursion
            restartingPatrol = false;
        }

        // ✅ Assign the first patrol waypoint
        GetNextWaypoint();
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
        ai.MoveTowards(targetWaypoint.position, ai.patrolSpeed);
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

            if (!patrolAI.loopPatrol && targetWaypoint == null)
            {
                Debug.Log(patrolAI.gameObject.name + " ⏹️ Reached last waypoint, stopping patrol.");
            }
        }
        else if (aiController is AIFlee fleeAI)  // ✅ Now works for AIFlee too
        {
            targetWaypoint = fleeAI.GetNextPatrolPoint();
        }

        if (targetWaypoint != null)
        {
            Debug.Log(aiController.gameObject.name + " ✅ Next patrol waypoint set: " + targetWaypoint.name);
        }
        else
        {
            Debug.LogWarning(aiController.gameObject.name + " ❗ Patrol points missing, staying idle.");
        }
    }

    public override void Exit()
    {
        Debug.Log(aiController.gameObject.name + " 🟡 Exiting PatrolState.");
    }
}









