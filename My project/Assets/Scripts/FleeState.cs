using UnityEngine;

public class FleeState : State
{
    private float fleeSpeed = 8f; // AI moves at a faster speed while fleeing
    private float safeThreshold = 15f; // AI stops fleeing if this distance from the player is reached

    public FleeState(AIController ai) : base(ai) { }

    public override void Enter()
    {
        // AI enters the FleeState when detecting a threat
    }

    public override void Execute(AIController ai)
    {
        // If the player is missing (e.g., destroyed or not detected), return to patrol
        if (ai.player == null)
        {
            ai.ChangeState(new PatrolState(ai));
            return;
        }

        float distanceToPlayer = Vector3.Distance(ai.transform.position, ai.player.transform.position);

        // If AI reaches a safe distance, it returns to patrolling
        if (distanceToPlayer >= safeThreshold)
        {
            ai.ChangeState(new PatrolState(ai));
            return;
        }

        // AI calculates the opposite direction from the player to flee
        Vector3 fleeDirection = (ai.transform.position - ai.player.transform.position).normalized;
        Vector3 safePoint = ai.transform.position + fleeDirection * 10f;

        ai.RotateTowards(safePoint); // Rotate toward the escape direction before moving
        ai.MoveTowards(safePoint, ai.fleeSpeed); // Move away from the player at flee speed
    }

    public override void Exit()
    {
        // AI exits the FleeState when it's no longer threatened
    }
}








