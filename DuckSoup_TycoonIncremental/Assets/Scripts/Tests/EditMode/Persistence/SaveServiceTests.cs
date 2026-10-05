using System;
using System.Collections.Generic;
using NUnit.Framework;

public class SaveServiceTests
{
    [Test]
    public void SaveGame_SavesEconomyAndFloorState()
    {
        EventBus eventBus = new EventBus();

        EconomyModel economyModel = new EconomyModel(new Money(100));

        EconomyService economyService = new EconomyService(economyModel,eventBus);

        economyService.AddMoney(new Money(50));


        FloorRuntimeModel floor1 =new FloorRuntimeModel(1,true);

        floor1.SetUpgradeLevel(2);
        floor1.SetProgress(0.4f);
        floor1.SetAutomaticTimeRemaining(3.5f);


        FloorRuntimeModel floor2 = new FloorRuntimeModel(2,false);


        Dictionary<int, FloorRuntimeModel>
            floors =
                new Dictionary<
                    int,
                    FloorRuntimeModel
                >
                {
                    { 1, floor1 },
                    { 2, floor2 }
                };


        FakeTimeService timeService =
            new FakeTimeService(
                new DateTime(
                    2026,
                    10,
                    2,
                    12,
                    0,
                    0,
                    DateTimeKind.Utc
                )
            );

        FakeSaveRepository repository =
            new FakeSaveRepository();


        SaveService saveService =
            new SaveService(
                economyService,
                timeService,
                repository,
                eventBus,
                floors
            );


        saveService.SaveGame();


        GameSaveData savedData =
            repository.LastSavedData;


        Assert.IsNotNull(
            savedData
        );

        Assert.AreEqual(
            1,
            savedData.saveVersion
        );

        Assert.AreEqual(
            "150",
            savedData.money
        );

        Assert.AreEqual(
            2,
            savedData.floors.Count
        );


        FloorSaveData savedFloor1 =
            savedData.floors[0];

        Assert.AreEqual(
            1,
            savedFloor1.floorId
        );

        Assert.IsTrue(
            savedFloor1.isUnlocked
        );

        Assert.AreEqual(
            2,
            savedFloor1.upgradeLevel
        );

        Assert.AreEqual(
            0.4f,
            savedFloor1.progress,
            0.0001f
        );

        Assert.AreEqual(
            3.5f,
            savedFloor1
                .automaticTimeRemaining,
            0.0001f
        );


        saveService.Dispose();
    }


    [Test]
    public void Tick_AfterThirtySeconds_PerformsAutosave()
    {
        EventBus eventBus =
            new EventBus();

        EconomyModel economyModel =
            new EconomyModel(
                new Money(100)
            );

        EconomyService economyService =
            new EconomyService(
                economyModel,
                eventBus
            );


        Dictionary<int, FloorRuntimeModel>
            floors =
                new Dictionary<
                    int,
                    FloorRuntimeModel
                >
                {
                    {
                        1,
                        new FloorRuntimeModel(
                            1,
                            true
                        )
                    }
                };


        FakeTimeService timeService =
            new FakeTimeService(
                DateTime.UtcNow
            );

        FakeSaveRepository repository =
            new FakeSaveRepository();


        SaveService saveService =
            new SaveService(
                economyService,
                timeService,
                repository,
                eventBus,
                floors
            );


        timeService.SetDeltaTime(
            30f
        );

        saveService.Tick();


        Assert.AreEqual(
            1,
            repository.SaveCount
        );


        saveService.Dispose();
    }
}