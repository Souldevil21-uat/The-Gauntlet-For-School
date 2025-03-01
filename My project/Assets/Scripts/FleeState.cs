using UnityEngine;

public class FleeState : State
{
    private float fleeSpeed = 8f; // ✅ AI flees faster than it chases
    private float safeThreshold = 15f; // ✅ AI stops fleeing at this distance

    public FleeState(AIController ai) : base(ai) { }

    public override void Enter()
    {
        Debug.Log(aiController.gameObject.name + " 🏃 Entered FleeState.");
    }

    public override void Execute(AIController ai)
    {
        Debug.Log(ai.gameObject.name + " 🔄 Running Execute() in FleeState!");

        if (ai.player == null)
        {
            Debug.LogWarning(ai.gameObject.name + " ❗ No player detected. Returning to patrol.");
            ai.ChangeState(new PatrolState(ai));
            return;
        }

        float distanceToPlayer = Vector3.Distance(ai.transform.position, ai.player.transform.position);

        // ✅ AI will stop fleeing if it's far enough
        if (distanceToPlayer >= safeThreshold)
        {
            Debug.Log(ai.gameObject.name + " ✅ Safe distance reached. Returning to patrol.");
            ai.ChangeState(new PatrolState(ai));
            return;
        }

        // ✅ AI should flee AWAY from the player
        Vector3 fleeDirection = (ai.transform.position - ai.player.transform.position).normalized;
        Vector3 safePoint = ai.transform.position + fleeDirection * 10f;  // ✅ Moves further away

        ai.RotateTowards(safePoint); // ✅ Rotate first before moving
        ai.MoveTowards(safePoint, ai.fleeSpeed);
    }

    public override void Exit()
    {
        Debug.Log(aiController.gameObject.name + " 🛑 Exiting FleeState.");
    }
}







