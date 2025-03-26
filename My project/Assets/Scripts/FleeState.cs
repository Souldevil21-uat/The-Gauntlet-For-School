using UnityEngine;

public class FleeState : State
{
    private float fleeSpeed;
    private float safeThreshold = 15f;
    private Vector3 fleeTarget;
    private float fleeDistance = 10f;
    private bool targetSet = false;

    public FleeState(AIController ai) : base(ai)
    {
        this.fleeSpeed = ai.fleeSpeed;
    }

    public override void Enter()
    {
        Debug.Log(aiController.gameObject.name + " entered FleeState!");
        SetFleeTarget();
    }

    public override void Execute(AIController ai)
    {
        GameObject player = aiController.player;

        if (player == null)
        {
            aiController.ChangeState(new PatrolState(aiController));
            return;
        }

        float distanceToPlayer = Vector3.Distance(aiController.transform.position, player.transform.position);
        if (distanceToPlayer >= safeThreshold)
        {
            aiController.ChangeState(new PatrolState(aiController));
            return;
        }

        // If close to flee target, pick a new one
        if (Vector3.Distance(aiController.transform.position, fleeTarget) < 1f)
        {
            SetFleeTarget();
        }

        aiController.RotateTowards(fleeTarget);
        aiController.MoveTowards(fleeTarget, fleeSpeed);
    }

    private void SetFleeTarget()
    {
        GameObject player = aiController.player;
        if (player == null) return;

        Vector3 fleeDirection = (aiController.transform.position - player.transform.position).normalized;
        Vector3 proposedTarget = aiController.transform.position + fleeDirection * fleeDistance;

        // Check if there's a wall directly behind
        if (Physics.Raycast(aiController.transform.position, fleeDirection, out RaycastHit hit, fleeDistance))
        {
            Debug.LogWarning(aiController.gameObject.name + " flee path blocked. Adjusting...");

            // Try to strafe left
            Vector3 left = Quaternion.Euler(0, -90, 0) * fleeDirection;
            if (!Physics.Raycast(aiController.transform.position, left, 3f))
            {
                proposedTarget = aiController.transform.position + left * 3f;
            }
            else
            {
                // Try to strafe right instead
                Vector3 right = Quaternion.Euler(0, 90, 0) * fleeDirection;
                if (!Physics.Raycast(aiController.transform.position, right, 3f))
                {
                    proposedTarget = aiController.transform.position + right * 3f;
                }
                else
                {
                    // Totally boxed in, fallback to rotate in place
                    Debug.Log(aiController.gameObject.name + " is surrounded!");
                }
            }
        }

        fleeTarget = proposedTarget;
    }

    public override void Exit()
    {
        Debug.Log(aiController.gameObject.name + " exited FleeState!");
    }
}














