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

        float distanceToPlayer = Vector3.Distance(ai.transform.position, ai.player.transform.position);

        // ✅ If AI is far enough, return to patrol
        if (distanceToPlayer > safeThreshold)
        {
            Debug.Log(ai.gameObject.name + " ✅ Safe distance reached. Returning to patrol.");
            ai.ChangeState(new PatrolState(ai));
            return;
        }

        // ✅ AI should flee in the opposite direction
        Vector3 fleeDirection = (ai.transform.position - ai.player.transform.position).normalized;
        Vector3 fleeTarget = ai.transform.position + fleeDirection * 10f;

        // ✅ Ensure AI rotates first before moving
        ai.RotateTowards(fleeTarget);

        // ✅ Use flee speed instead of default move speed
        ai.pawn.Move(fleeSpeed);
    }

    public override void Exit()
    {
        Debug.Log(aiController.gameObject.name + " 🛑 Exiting FleeState.");
    }
}






