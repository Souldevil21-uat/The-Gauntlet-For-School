using UnityEngine;

public class ChaseState : State
{
    private float chaseSpeed;

    public ChaseState(AIController ai) : base(ai)
    {
        this.chaseSpeed = ai.chaseSpeed; // Ensures AI uses correct chase speed
    }

    // Called when the AI enters the chase state
    public override void Enter()
    {
        // AI is now actively pursuing the player
    }

    // Continuously executes while AI is in the chase state
    public override void Execute(AIController ai)
    {
        // If AI loses sight of the player, determine the next action
        if (!ai.CanSeePlayer())
        {
            // If AI can still hear the player, maintain movement but adjust its reaction
            if (ai.CanHearPlayer())
            {
                return; // AI will continue chasing based on hearing, but without stopping
            }

            // If the player is neither seen nor heard, return to patrol
            ai.ChangeState(new PatrolState(ai));
            return;
        }

        // Ensure AI rotates towards the player before moving
        ai.RotateTowards(ai.player.transform.position);

        // Ensure AI moves at the designated chase speed
        ai.MoveTowards(ai.player.transform.position, chaseSpeed);
    }

    // Called when the AI exits the chase state
    public override void Exit()
    {
        // No additional exit behavior required
    }
}








