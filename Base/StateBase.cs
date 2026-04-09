

public abstract class StateBase
{
    public abstract void Init(IstateMachineOwner owner);

    public abstract void Enter();

    public abstract void Exit();

    public abstract void Destroy();
}
