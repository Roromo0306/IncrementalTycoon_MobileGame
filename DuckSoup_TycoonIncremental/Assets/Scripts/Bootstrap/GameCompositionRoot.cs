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
    private IUpgradeService upgradeService;
    private EconomyController economyController;


    public void Build()
    {
        CreateCoreServices();
        CreateGameServices();

        Debug.Log("GameCompositionRoot: Dependencies created.");
    }


    private void CreateCoreServices()
    {
        eventBus = new EventBus();

        timeService = new UnityTimeService();
    }


    private void CreateGameServices()
    {
        economyModel = new EconomyModel(new Money(100));

        economyService = new EconomyService(economyModel,eventBus);

        productionService = new ProductionService(economyService);

        upgradeService = new UpgradeService(economyService,eventBus);

        eventBus.Subscribe<UpgradePurchasedEvent>(OnUpgradePurchased);
    }


    public void InitializeGameplay(FloorDefinitionSO definition,FloorView view, EconomyView economyView)
    {
        floorRuntimeModel = new FloorRuntimeModel(definition.Id,true);

        floorStateMachine = new FloorStateMachine(definition, floorRuntimeModel, productionService, timeService);

        floorController = new FloorController(definition, floorRuntimeModel, view, floorStateMachine, upgradeService);

        economyController = new EconomyController(economyService, eventBus, economyView);
    }


    public void Tick()
    {
        floorController?.Tick();
    }


    public void Dispose()
    {
        floorController?.Dispose();
        economyController?.Dispose();
    }

    private void OnUpgradePurchased(UpgradePurchasedEvent upgradeEvent)
    {
        Debug.Log(
            $"Upgrade purchased - Floor: {upgradeEvent.FloorId}, " +
            $"Level: {upgradeEvent.UpgradeLevel}, " +
            $"Visual: {upgradeEvent.VisualStage}"
        );
    }
}