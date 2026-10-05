using System;
using System.Collections.Generic;
using System.Globalization;

public class SaveService : IDisposable
{
    private const int CurrentSaveVersion = 1;

    private const float AutosaveIntervalSeconds = 30f;


    private readonly IEconomyService economyService;

    private readonly ITimeService timeService;

    private readonly ISaveRepository saveRepository;

    private readonly IEventBus eventBus;

    private readonly IReadOnlyDictionary<int,FloorRuntimeModel> floorRuntimeModels;


    private float autosaveElapsed;

    private bool isDisposed;


    public SaveService(IEconomyService economyService,ITimeService timeService,ISaveRepository saveRepository,IEventBus eventBus,IReadOnlyDictionary<int,FloorRuntimeModel> floorRuntimeModels)
    {
        this.economyService = economyService ?? throw new ArgumentNullException(nameof(economyService));

        this.timeService = timeService ?? throw new ArgumentNullException(nameof(timeService));

        this.saveRepository = saveRepository ?? throw new ArgumentNullException(nameof(saveRepository));

        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

        this.floorRuntimeModels = floorRuntimeModels ?? throw new ArgumentNullException(nameof(floorRuntimeModels));

        SubscribeToEvents();
    }


    public void Tick()
    {
        if (floorRuntimeModels.Count == 0)
        {
            return;
        }

        autosaveElapsed += timeService.DeltaTime;

        if (autosaveElapsed < AutosaveIntervalSeconds)
        {
            return;
        }

        SaveGame();
    }


    public void SaveGame()
    {
        if (floorRuntimeModels.Count == 0)
        {
            return;
        }

        GameSaveData saveData = CreateSaveData();

        saveRepository.Save(saveData);

        autosaveElapsed = 0f;
    }


    private GameSaveData CreateSaveData()
    {
        GameSaveData saveData = new GameSaveData
            {
                saveVersion = CurrentSaveVersion,

                money = economyService.CurrentMoney.Value.ToString(CultureInfo.InvariantCulture),

                rebirthLevel = 0,

                lastPlayedTimeUtc = timeService.UtcNow.ToString("O",CultureInfo.InvariantCulture)
            };


        List<int> floorIds = new List<int>(floorRuntimeModels.Keys);

        floorIds.Sort();


        for (int i = 0; i < floorIds.Count; i++)
        {
            FloorRuntimeModel runtimeModel = floorRuntimeModels[floorIds[i]];

            FloorSaveData floorSaveData = new FloorSaveData
                {
                    floorId = runtimeModel.FloorId,

                    isUnlocked = runtimeModel.IsUnlocked,

                    upgradeLevel = runtimeModel.UpgradeLevel,

                    progress = runtimeModel.Progress,

                    automaticTimeRemaining = runtimeModel.AutomaticTimeRemaining
                };

            saveData.floors.Add(floorSaveData);
        }


        return saveData;
    }


    private void SubscribeToEvents()
    {
        eventBus.Subscribe<UpgradePurchasedEvent>(OnUpgradePurchased);

        eventBus.Subscribe<FloorUnlockedEvent>(OnFloorUnlocked);
    }


    private void OnUpgradePurchased(UpgradePurchasedEvent upgradeEvent)
    {
        SaveGame();
    }


    private void OnFloorUnlocked(FloorUnlockedEvent floorEvent)
    {
        SaveGame();
    }


    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        eventBus.Unsubscribe<UpgradePurchasedEvent>(OnUpgradePurchased);

        eventBus.Unsubscribe<FloorUnlockedEvent>(OnFloorUnlocked);

        isDisposed = true;
    }
}