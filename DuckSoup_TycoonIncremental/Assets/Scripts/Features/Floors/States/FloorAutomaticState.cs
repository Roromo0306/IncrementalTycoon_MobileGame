using System;

public class FloorAutomaticState : IFloorState
{
    private readonly FloorDefinitionSO definition;
    private readonly FloorRuntimeModel runtimeModel;


    public FloorStateType StateType => FloorStateType.Automatic;

    public bool CanGenerate => false;


    public FloorAutomaticState(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel)
    {
        this.definition = definition ?? throw new ArgumentNullException(nameof(definition));

        this.runtimeModel = runtimeModel ?? throw new ArgumentNullException(nameof(runtimeModel));
    }


    public void Enter()
    {
        runtimeModel.SetAutomaticTimeRemaining(definition.AutomaticDuration);
    }


    public void Exit()
    {
    }


    public void HandleGenerate()
    {
    }
}