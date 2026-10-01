using System;
using UnityEngine;

public class GameCompositionRoot : IDisposable
{
    private IEventBus eventBus;
    private ITimeService timeService;

    private EconomyModel economyModel;
    private IEconomyService economyService;

    private IProductionService productionService;

    private FloorRuntimeModel floorRuntimeModel;
    private FloorStateMachine floorStateMachine;
    private FloorController floorController;


    public void Build()
    {
        CreateCoreServices();
        CreateGameServices();

        Debug.Log(
            "GameCompositionRoot: Dependencies created."
        );
    }


    private void CreateCoreServices()
    {
        eventBus =
            new EventBus();

        timeService =
            new UnityTimeService();
    }


    private void CreateGameServices()
    {
        economyModel =
            new EconomyModel(
                new Money(100)
            );

        economyService =
            new EconomyService(
                economyModel,
                eventBus
            );

        productionService =
            new ProductionService(
                economyService
            );
    }


    public void InitializeGameplay(
        FloorDefinitionSO definition,
        FloorView view)
    {
        floorRuntimeModel =
            new FloorRuntimeModel(
                definition.Id,
                true
            );

        floorStateMachine =
            new FloorStateMachine(
                definition,
                floorRuntimeModel,
                productionService,
                timeService
            );

        floorController =
            new FloorController(
                definition,
                floorRuntimeModel,
                view,
                floorStateMachine
            );
    }


    public void Tick()
    {
        floorController?.Tick();
    }


    public void Dispose()
    {
        floorController?.Dispose();
    }
}