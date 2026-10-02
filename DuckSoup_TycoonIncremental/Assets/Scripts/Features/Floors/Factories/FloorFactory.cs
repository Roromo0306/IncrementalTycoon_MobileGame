using System;
using System.Collections.Generic;

public class FloorFactory
{
    private readonly IProductionService productionService;
    private readonly ITimeService timeService;
    private readonly IUpgradeService upgradeService;
    private readonly IIncomeCalculator incomeCalculator;
    private readonly IFloorProgressionService
        floorProgressionService;
    private readonly IEventBus eventBus;


    public FloorFactory(
        IProductionService productionService,
        ITimeService timeService,
        IUpgradeService upgradeService,
        IIncomeCalculator incomeCalculator,
        IFloorProgressionService floorProgressionService,
        IEventBus eventBus)
    {
        this.productionService = productionService
            ?? throw new ArgumentNullException(
                nameof(productionService)
            );

        this.timeService = timeService
            ?? throw new ArgumentNullException(
                nameof(timeService)
            );

        this.upgradeService = upgradeService
            ?? throw new ArgumentNullException(
                nameof(upgradeService)
            );

        this.incomeCalculator = incomeCalculator
            ?? throw new ArgumentNullException(
                nameof(incomeCalculator)
            );

        this.floorProgressionService =
            floorProgressionService
            ?? throw new ArgumentNullException(
                nameof(floorProgressionService)
            );

        this.eventBus = eventBus
            ?? throw new ArgumentNullException(
                nameof(eventBus)
            );
    }


    public FloorInstance Create(
        FloorDefinitionSO definition,
        FloorRuntimeModel runtimeModel,
        FloorView view,
        IReadOnlyDictionary<int, FloorRuntimeModel>
            allRuntimeModels)
    {
        ValidateParameters(
            definition,
            runtimeModel,
            view,
            allRuntimeModels
        );

        FloorStateMachine stateMachine =
            new FloorStateMachine(
                definition,
                runtimeModel,
                productionService,
                timeService
            );

        FloorController floorController =
            new FloorController(
                definition,
                runtimeModel,
                view,
                stateMachine,
                upgradeService,
                incomeCalculator,
                floorProgressionService,
                allRuntimeModels
            );

        FloorVisualController visualController =
            new FloorVisualController(
                definition,
                runtimeModel,
                view,
                eventBus
            );

        return new FloorInstance(
            definition,
            runtimeModel,
            floorController,
            visualController
        );
    }


    private void ValidateParameters(
        FloorDefinitionSO definition,
        FloorRuntimeModel runtimeModel,
        FloorView view,
        IReadOnlyDictionary<int, FloorRuntimeModel>
            allRuntimeModels)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(
                nameof(definition)
            );
        }

        if (runtimeModel == null)
        {
            throw new ArgumentNullException(
                nameof(runtimeModel)
            );
        }

        if (view == null)
        {
            throw new ArgumentNullException(
                nameof(view)
            );
        }

        if (allRuntimeModels == null)
        {
            throw new ArgumentNullException(
                nameof(allRuntimeModels)
            );
        }

        if (definition.Id
            != runtimeModel.FloorId)
        {
            throw new ArgumentException(
                "Floor definition and runtime model must use the same Floor ID."
            );
        }
    }
}