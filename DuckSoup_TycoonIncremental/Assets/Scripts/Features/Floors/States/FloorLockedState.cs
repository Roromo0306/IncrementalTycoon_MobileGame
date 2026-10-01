public class FloorLockedState : IFloorState
{
    public FloorStateType StateType => FloorStateType.Locked;

    public bool CanGenerate => false;


    public void Enter()
    {
    }


    public void Exit()
    {
    }


    public void HandleGenerate()
    {
    }

    public void Tick()
    {

    }
}