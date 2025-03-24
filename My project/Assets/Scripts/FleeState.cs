using UnityEngine;

public class FleeState : State
{
    private float fleeSpeed;
    private float safeThreshold = 15f;
    private Vector3 fleeTarget;
    private float fleeDistance = 10f; // How far it should move away before re-evaluating

    public FleeState(AIController ai) : base(ai)
    {
        this.fleeSpeed = ai.fleeSpeed;
    }

    public override void Enter()
    {
        Debug.Log(aiController.gameObject.name + " entered FleeState!");
    }

    public override void Execute(AIController ai)
    {
        GameObject player = aiController.player;

        if (player == null)
        {
            Debug.Log(aiController.gameObject.name + " lost sight of the player. Returning to patrol.");
            aiController.ChangeState(new PatrolState(aiController));
            return;
        }

        float distanceToPlayer = Vector3.Distance(aiController.transform.position, player.transform.position);

        if (distanceToPlayer >= safeThreshold)
        {
            Debug.Log(aiController.gameObject.name + " reached safe distance. Returning to patrol.");
            aiController.ChangeState(new PatrolState(aiController));
            return;
        }

        // Move directly away from the player's position
        Vector3 fleeDirection = (aiController.transform.position - player.transform.position).normalized;
        fleeTarget = aiController.transform.position + fleeDirection * fleeDistance;

        // Check for obstacles
        if (Physics.Raycast(aiController.transform.position, fleeDirection, out RaycastHit hit, fleeDistance))
        {
            Debug.LogWarning(aiController.gameObject.name + " hit an obstacle! Adjusting course.");
            fleeTarget += aiController.transform.right * 3f; // Move slightly to the side to avoid getting stuck
        }

        aiController.RotateTowards(fleeTarget);
        aiController.MoveTowards(fleeTarget, fleeSpeed);
    }

    public override void Exit()
    {
        Debug.Log(aiController.gameObject.name + " exited FleeState!");
    }
}













