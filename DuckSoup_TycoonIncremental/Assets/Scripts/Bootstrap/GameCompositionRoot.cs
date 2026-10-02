using System;
using System.Collections.Generic;
using UnityEngine;

public class GameCompositionRoot : IDisposable
{
    private IEventBus eventBus;
    private ITimeService timeService;

    private EconomyModel economyModel;
    private IEconomyService economyService;

    private IIncomeCalculator incomeCalculator;
    private IProductionService productionService;
    private IUpgradeService upgradeService;
    private IFloorProgressionService floorProgressionService;

    private FloorFactory floorFactory;

    private EconomyController economyController;

    private readonly Dictionary<int, FloorRuntimeModel>floorRuntimeModels = new();

    private readonly List<FloorInstance>floorInstances = new();


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

        economyService =new EconomyService(economyModel,eventBus);


        IIncomeModifier[] incomeModifiers ={new UpgradeIncomeModifier()};

        incomeCalculator =new IncomeCalculator(incomeModifiers);


        productionService = new ProductionService(economyService,incomeCalculator);


        upgradeService = new UpgradeService(economyService,eventBus);


        floorProgressionService = new FloorProgressionService(economyService,eventBus);


        floorFactory = new FloorFactory(productionService,timeService,upgradeService,incomeCalculator,eventBus);
    }


    public void InitializeGameplay(FloorDefinitionSO definition,FloorView view,EconomyView economyView)
    {
        economyController = new EconomyController(economyService,eventBus,economyView);

        FloorRuntimeModel runtimeModel = new FloorRuntimeModel(definition.Id,true);

        floorRuntimeModels.Add(definition.Id,runtimeModel);

        FloorInstance floorInstance =floorFactory.Create(definition,runtimeModel,view);

        floorInstances.Add(floorInstance);
    }


    public void Tick()
    {
        for (int i = 0; i < floorInstances.Count;i++)
        {
            floorInstances[i].Tick();
        }
    }


    public void Dispose()
    {
        for (int i = 0;i < floorInstances.Count;i++)
        {
            floorInstances[i].Dispose();
        }

        floorInstances.Clear();
        floorRuntimeModels.Clear();

        economyController?.Dispose();
    }
}