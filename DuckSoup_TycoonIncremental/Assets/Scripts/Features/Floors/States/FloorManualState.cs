using System;

public class FloorManualState : IFloorState
{
    private readonly FloorStateMachine stateMachine;
    private readonly FloorDefinitionSO definition;
    private readonly FloorRuntimeModel runtimeModel;
    private readonly IProductionService productionService;
    private readonly ITimeService timeService;

    private float timeSinceLastGenerate;


    public FloorStateType StateType => FloorStateType.Manual;

    public bool CanGenerate => true;


    public FloorManualState(FloorStateMachine stateMachine, FloorDefinitionSO definition, FloorRuntimeModel runtimeModel, IProductionService productionService, ITimeService timeService)
    {
        this.stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));

        this.definition = definition ?? throw new ArgumentNullException(nameof(definition));

        this.runtimeModel = runtimeModel ?? throw new ArgumentNullException(nameof(runtimeModel));

        this.productionService = productionService ?? throw new ArgumentNullException(nameof(productionService));

        this.timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
    }


    public void Enter()
    {
        timeSinceLastGenerate = 0f;
    }


    public void Exit()
    {
    }


    public void HandleGenerate()
    {
        timeSinceLastGenerate = 0f;

        bool productionCompleted = productionService.GenerateManual(definition, runtimeModel);

        if (!productionCompleted)
        {
            return;
        }

        stateMachine.ChangeToAutomatic();
    }


    public void Tick()
    {
        if (runtimeModel.Progress <= 0f)
        {
            timeSinceLastGenerate = 0f;
            return;
        }

        float deltaTime = timeService.DeltaTime;

        if (deltaTime <= 0f)
        {
            return;
        }

        float previousIdleTime = timeSinceLastGenerate;

        timeSinceLastGenerate += deltaTime;

        if (timeSinceLastGenerate <= definition.ManualDecayDelay)
        {
            return;
        }

        float decayStartTime = Math.Max(previousIdleTime, definition.ManualDecayDelay);

        float decayDeltaTime = timeSinceLastGenerate - decayStartTime;

        if (decayDeltaTime <= 0f)
        {
            return;
        }

        float decayAmount = definition.ManualDecayPerSecond * decayDeltaTime;

        runtimeModel.SetProgress(runtimeModel.Progress - decayAmount);
    }
}