using System;
using System.Globalization;

public class LoadService
{
    private readonly ISaveRepository saveRepository;


    public LoadService(
        ISaveRepository saveRepository)
    {
        this.saveRepository =
            saveRepository
            ?? throw new ArgumentNullException(
                nameof(saveRepository)
            );
    }


    public GameSaveData LoadGame()
    {
        return saveRepository.Load();
    }


    public Money GetMoneyOrDefault(
        GameSaveData saveData,
        Money defaultMoney)
    {
        if (saveData == null)
        {
            return defaultMoney;
        }

        if (string.IsNullOrWhiteSpace(
            saveData.money))
        {
            return defaultMoney;
        }

        bool parsed =
            decimal.TryParse(
                saveData.money,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out decimal moneyValue
            );

        if (!parsed)
        {
            return defaultMoney;
        }

        return new Money(
            moneyValue
        );
    }


    public FloorRuntimeModel CreateFloorRuntimeModel(
        FloorDefinitionSO definition,
        GameSaveData saveData)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(
                nameof(definition)
            );
        }

        FloorSaveData floorSaveData =
            FindFloorSaveData(
                saveData,
                definition.Id
            );

        if (floorSaveData == null)
        {
            return CreateDefaultRuntimeModel(
                definition
            );
        }

        FloorRuntimeModel runtimeModel =
            new FloorRuntimeModel(
                definition.Id,
                floorSaveData.isUnlocked
            );

        int maxUpgradeLevel =
            definition.Upgrades != null
                ? definition.Upgrades.Length
                : 0;

        int restoredUpgradeLevel =
            floorSaveData.upgradeLevel;

        if (restoredUpgradeLevel < 0)
        {
            restoredUpgradeLevel = 0;
        }

        if (restoredUpgradeLevel
            > maxUpgradeLevel)
        {
            restoredUpgradeLevel =
                maxUpgradeLevel;
        }

        runtimeModel.SetUpgradeLevel(
            restoredUpgradeLevel
        );

        runtimeModel.SetProgress(
            floorSaveData.progress
        );

        runtimeModel.SetAutomaticTimeRemaining(
            floorSaveData
                .automaticTimeRemaining
        );

        return runtimeModel;
    }


    private FloorRuntimeModel CreateDefaultRuntimeModel(
        FloorDefinitionSO definition)
    {
        bool startsUnlocked =
            definition.Id == 1;

        return new FloorRuntimeModel(
            definition.Id,
            startsUnlocked
        );
    }


    private FloorSaveData FindFloorSaveData(
        GameSaveData saveData,
        int floorId)
    {
        if (saveData?.floors == null)
        {
            return null;
        }

        for (int i = 0;
             i < saveData.floors.Count;
             i++)
        {
            FloorSaveData floor =
                saveData.floors[i];

            if (floor != null
                && floor.floorId == floorId)
            {
                return floor;
            }
        }

        return null;
    }
}