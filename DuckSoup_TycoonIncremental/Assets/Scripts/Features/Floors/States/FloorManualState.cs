using System;

public class FloorManualState : IFloorState
{
    private readonly FloorStateMachine stateMachine;
    private readonly FloorDefinitionSO definition;
    private readonly FloorRuntimeModel runtimeModel;
    private readonly IProductionService productionService;


    public FloorStateType StateType =>
        FloorStateType.Manual;

    public bool CanGenerate => true;


    public FloorManualState(FloorStateMachine stateMachine, FloorDefinitionSO definition, FloorRuntimeModel runtimeModel, IProductionService productionService)
    {
        this.stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));

        this.definition = definition ?? throw new ArgumentNullException(nameof(definition));

        this.runtimeModel = runtimeModel ?? throw new ArgumentNullException(nameof(runtimeModel));

        this.productionService = productionService ?? throw new ArgumentNullException(nameof(productionService));
    }


    public void Enter()
    {
    }


    public void Exit()
    {
    }


    public void HandleGenerate()
    {
        bool productionCompleted = productionService.GenerateManual(definition, runtimeModel);

        if (!productionCompleted)
        {
            return;
        }

        stateMachine.ChangeToAutomatic();
    }
}