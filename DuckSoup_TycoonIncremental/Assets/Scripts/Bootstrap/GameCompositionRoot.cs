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

    private ISaveRepository saveRepository;
    private SaveService saveService;
    private LoadService loadService;

    private FloorFactory floorFactory;
    private EconomyController economyController;


    private readonly Dictionary<int, FloorRuntimeModel>floorRuntimeModels = new();

    private readonly List<FloorInstance>floorInstances = new();


    public void Build()
    {
        CreateCoreServices();
        CreateGameServices();
        CreatePersistenceServices();

        Debug.Log("GameCompositionRoot: Dependencies created.");
    }


    private void CreateCoreServices()
    {
        eventBus = new EventBus();

        timeService = new UnityTimeService();
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


        IIncomeModifier[] incomeModifiers =
        {
            new UpgradeIncomeModifier()
        };

        incomeCalculator =
            new IncomeCalculator(
                incomeModifiers
            );


        productionService =
            new ProductionService(
                economyService,
                incomeCalculator
            );


        upgradeService =
            new UpgradeService(
                economyService,
                eventBus
            );


        floorProgressionService =
            new FloorProgressionService(
                economyService,
                eventBus
            );


        floorFactory =
            new FloorFactory(
                productionService,
                timeService,
                upgradeService,
                incomeCalculator,
                floorProgressionService,
                eventBus
            );
    }


    public void InitializeGameplay(
    FloorDefinitionSO[] definitions,
    TowerView towerView,
    EconomyView economyView)
    {
        if (definitions == null
            || definitions.Length == 0)
        {
            throw new ArgumentException(
                "At least one floor definition is required.",
                nameof(definitions)
            );
        }

        if (towerView == null)
        {
            throw new ArgumentNullException(
                nameof(towerView)
            );
        }

        if (economyView == null)
        {
            throw new ArgumentNullException(
                nameof(economyView)
            );
        }


        GameSaveData saveData =
            loadService.LoadGame();


        Money loadedMoney =
            loadService.GetMoneyOrDefault(
                saveData,
                new Money(100)
            );

        economyModel.SetMoney(
            loadedMoney
        );


        economyController =
            new EconomyController(
                economyService,
                eventBus,
                economyView
            );


        List<FloorDefinitionSO>
            orderedDefinitions =
                new List<FloorDefinitionSO>(
                    definitions
                );

        orderedDefinitions.Sort(
            (first, second) =>
                first.Id.CompareTo(
                    second.Id
                )
        );


        CreateRuntimeModels(
            orderedDefinitions,
            saveData
        );

        CreateFloorInstances(
            orderedDefinitions,
            towerView
        );

        towerView.ScrollToBottom();


        if (saveData == null)
        {
            Debug.Log(
                "LoadService: New game created."
            );
        }
        else
        {
            Debug.Log(
                "LoadService: Save loaded successfully."
            );
        }
    }


    private void CreateRuntimeModels(
     IReadOnlyList<FloorDefinitionSO>
         definitions,
     GameSaveData saveData)
    {
        for (int i = 0;
             i < definitions.Count;
             i++)
        {
            FloorDefinitionSO definition =
                definitions[i];

            if (definition == null)
            {
                throw new InvalidOperationException(
                    $"Floor definition at index {i} is null."
                );
            }

            if (definition.Id <= 0)
            {
                throw new InvalidOperationException(
                    "Every floor must have an ID greater than zero."
                );
            }

            if (floorRuntimeModels.ContainsKey(
                definition.Id))
            {
                throw new InvalidOperationException(
                    $"Duplicate Floor ID: {definition.Id}."
                );
            }


            FloorRuntimeModel runtimeModel =
                loadService
                    .CreateFloorRuntimeModel(
                        definition,
                        saveData
                    );


            floorRuntimeModels.Add(
                definition.Id,
                runtimeModel
            );
        }
    }

    private void CreateFloorInstances(
        IReadOnlyList<FloorDefinitionSO>
            definitions,
        TowerView towerView)
    {
        for (int i = 0;
             i < definitions.Count;
             i++)
        {
            FloorDefinitionSO definition =
                definitions[i];

            FloorRuntimeModel runtimeModel =
                floorRuntimeModels[
                    definition.Id
                ];

            FloorView view =
                towerView.CreateFloorView(
                    definition
                );

            FloorInstance instance =
                floorFactory.Create(
                    definition,
                    runtimeModel,
                    view,
                    floorRuntimeModels
                );

            floorInstances.Add(
                instance
            );
        }
    }


    public void Tick()
    {
        for (int i = 0;
             i < floorInstances.Count;
             i++)
        {
            floorInstances[i].Tick();
        }

        saveService?.Tick();
    }


    public void Dispose()
    {
        saveService?.SaveGame();

        for (int i = 0;
             i < floorInstances.Count;
             i++)
        {
            floorInstances[i].Dispose();
        }

        floorInstances.Clear();

        economyController?.Dispose();

        saveService?.Dispose();

        floorRuntimeModels.Clear();
    }

    private void CreatePersistenceServices()
    {
        saveRepository =
            new JsonSaveRepository();

        loadService =
            new LoadService(
                saveRepository
            );

        saveService =
            new SaveService(
                economyService,
                timeService,
                saveRepository,
                eventBus,
                floorRuntimeModels
            );
    }

    public void SaveGame()
    {
        saveService?.SaveGame();
    }
}