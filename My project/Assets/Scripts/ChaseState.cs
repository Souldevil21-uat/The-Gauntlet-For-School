using UnityEngine;

public class ChaseState : State
{
    private float chaseSpeed;
    private Vector3 lastKnownPosition;
    private AIController ai; // Store AI reference

    public ChaseState(AIController ai) : base(ai)
    {
        this.ai = ai; // Store reference to AIController
        this.chaseSpeed = ai.chaseSpeed;
    }

    // Called when the AI enters the chase state
    public override void Enter()
    {
        if (ai.player != null)
        {
            lastKnownPosition = ai.player.transform.position;
        }
    }

    // Continuously executes while AI is in the chase state
    public override void Execute(AIController ai)
    {
        if (ai.player == null)
        {
            ai.ChangeState(new PatrolState(ai));
            return;
        }

        // If AI can see the player, chase them directly
        if (ai.CanSeePlayer())
        {
            lastKnownPosition = ai.player.transform.position;
            ai.RotateTowards(lastKnownPosition);
            ai.MoveTowards(lastKnownPosition, chaseSpeed);
        }
        // If AI can't see the player but can hear them, move towards the last heard position
        else if (ai.CanHearPlayer())
        {
            ai.MoveTowards(lastKnownPosition, chaseSpeed);
        }
        // If AI can neither see nor hear the player, return to patrol
        else
        {
            ai.ChangeState(new PatrolState(ai));
        }
    }

    // Called when the AI exits the chase state
    public override void Exit()
    {
        // No additional exit behavior required
    }
}










