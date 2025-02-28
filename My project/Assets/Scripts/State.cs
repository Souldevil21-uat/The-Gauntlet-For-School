public abstract class State
{
    protected AIController aiController;

    public State(AIController ai)
    {
        aiController = ai;
    }

    public abstract void Enter();
    public abstract void Execute(AIController ai); // Ensure subclasses override this version
    public abstract void Exit();
}


