using System;

public class FloorFactory
{
    private readonly IProductionService productionService;
    private readonly ITimeService timeService;
    private readonly IUpgradeService upgradeService;
    private readonly IIncomeCalculator incomeCalculator;
    private readonly IEventBus eventBus;


    public FloorFactory(IProductionService productionService,ITimeService timeService,IUpgradeService upgradeService,IIncomeCalculator incomeCalculator,IEventBus eventBus)
    {
        this.productionService = productionService ?? throw new ArgumentNullException(nameof(productionService));

        this.timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));

        this.upgradeService = upgradeService ?? throw new ArgumentNullException(nameof(upgradeService));

        this.incomeCalculator = incomeCalculator ?? throw new ArgumentNullException(nameof(incomeCalculator));

        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }


    public FloorInstance Create(FloorDefinitionSO definition,FloorRuntimeModel runtimeModel,FloorView view)
    {
        ValidateParameters(definition,runtimeModel,view);

        FloorStateMachine stateMachine = CreateStateMachine(definition,runtimeModel);

        FloorController floorController = CreateFloorController(definition,runtimeModel,view,stateMachine);

        FloorVisualController visualController = new FloorVisualController(definition,runtimeModel,view,eventBus);

        return new FloorInstance(definition,runtimeModel,floorController,visualController);
    }


    private FloorStateMachine CreateStateMachine(FloorDefinitionSO definition,FloorRuntimeModel runtimeModel)
    {
        return new FloorStateMachine(definition,runtimeModel,productionService,timeService);
    }


    private FloorController CreateFloorController(FloorDefinitionSO definition,FloorRuntimeModel runtimeModel,FloorView view,FloorStateMachine stateMachine)
    {
        return new FloorController(definition,runtimeModel,view,stateMachine,upgradeService,incomeCalculator);
    }


    private void ValidateParameters(FloorDefinitionSO definition,FloorRuntimeModel runtimeModel,FloorView view)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        if (runtimeModel == null)
        {
            throw new ArgumentNullException(nameof(runtimeModel));
        }

        if (view == null)
        {
            throw new ArgumentNullException(nameof(view));
        }

        if (definition.Id != runtimeModel.FloorId)
        {
            throw new ArgumentException("Floor definition and runtime model must use the same Floor ID.");
        }
    }
}