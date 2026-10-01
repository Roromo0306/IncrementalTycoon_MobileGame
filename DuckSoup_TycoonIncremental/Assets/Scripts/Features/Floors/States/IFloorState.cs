public interface IFloorState
{
    FloorStateType StateType { get; }

    bool CanGenerate { get; }

    void Enter();

    void Exit();

    void HandleGenerate();

    void Tick();
}