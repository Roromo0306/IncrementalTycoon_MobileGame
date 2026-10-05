using System;

public class FloorAutomaticState : IFloorState
{
    private readonly FloorStateMachine stateMachine;
    private readonly FloorDefinitionSO definition;
    private readonly FloorRuntimeModel runtimeModel;
    private readonly IProductionService productionService;
    private readonly ITimeService timeService;

    private float automaticIncomeElapsed;


    public FloorStateType StateType => FloorStateType.Automatic;

    public bool CanGenerate => false;


    public FloorAutomaticState(FloorStateMachine stateMachine, FloorDefinitionSO definition, FloorRuntimeModel runtimeModel, IProductionService productionService, ITimeService timeService)
    {
        this.stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));

        this.definition = definition ?? throw new ArgumentNullException(nameof(definition));

        this.runtimeModel = runtimeModel ?? throw new ArgumentNullException(nameof(runtimeModel));

        this.productionService = productionService ?? throw new ArgumentNullException(nameof(productionService));

        this.timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));
    }


    public void Enter()
    {
        automaticIncomeElapsed = 0f;

        if (runtimeModel.AutomaticTimeRemaining > 0f)
        {
            return;
        }

        runtimeModel.SetAutomaticTimeRemaining(definition.AutomaticDuration);
    }


    public void Exit()
    {
        runtimeModel.SetAutomaticTimeRemaining(0f);

        runtimeModel.ResetProgress();
    }


    public void HandleGenerate()
    {
    }


    public void Tick()
    {
        float remainingTime = runtimeModel.AutomaticTimeRemaining;

        if (remainingTime <= 0f)
        {
            FinishAutomaticProduction();
            return;
        }

        float deltaTime = timeService.DeltaTime;

        if (deltaTime <= 0f)
        {
            return;
        }

        float activeDeltaTime = Math.Min(deltaTime, remainingTime);

        automaticIncomeElapsed += activeDeltaTime;

        GenerateAutomaticIncome();

        remainingTime -= activeDeltaTime;

        runtimeModel.SetAutomaticTimeRemaining(remainingTime);

        if (runtimeModel.AutomaticTimeRemaining <= 0f)
        {
            FinishAutomaticProduction();
        }
    }


    private void GenerateAutomaticIncome()
    {
        float interval = Math.Max(0.01f, definition.AutomaticIncomeInterval);

        while (automaticIncomeElapsed >= interval)
        {
            productionService.GenerateAutomaticIncome(definition, runtimeModel);

            automaticIncomeElapsed -= interval;
        }
    }


    private void FinishAutomaticProduction()
    {
        stateMachine.ChangeToManual();
    }
}