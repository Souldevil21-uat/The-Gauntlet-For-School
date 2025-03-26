// Base abstract class for AI states used in a state machine
public abstract class State
{
    // Reference to the AIController using this state
    protected AIController aiController;

    // Constructor sets the controller reference
    public State(AIController ai)
    {
        aiController = ai;
    }

    // Called when the state is entered
    public abstract void Enter();

    // Called every frame while the state is active
    public abstract void Execute(AIController ai);

    // Called when the state is exited
    public abstract void Exit();
}



