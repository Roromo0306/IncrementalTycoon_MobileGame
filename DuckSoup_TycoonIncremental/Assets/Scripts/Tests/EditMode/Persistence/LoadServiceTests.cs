using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class LoadServiceTests
{
    [Test]
    public void GetMoneyOrDefault_WithSave_RestoresMoney()
    {
        FakeSaveRepository repository =
            new FakeSaveRepository();

        GameSaveData saveData =
            new GameSaveData
            {
                saveVersion = 1,
                money = "425.5"
            };

        repository.Save(
            saveData
        );


        LoadService loadService =
            new LoadService(
                repository
            );


        GameSaveData loadedData =
            loadService.LoadGame();

        Money loadedMoney =
            loadService.GetMoneyOrDefault(
                loadedData,
                new Money(100)
            );


        Assert.AreEqual(
            425.5m,
            loadedMoney.Value
        );
    }


    [Test]
    public void GetMoneyOrDefault_WithoutSave_ReturnsDefaultMoney()
    {
        FakeSaveRepository repository =
            new FakeSaveRepository();

        LoadService loadService =
            new LoadService(
                repository
            );


        Money loadedMoney =
            loadService.GetMoneyOrDefault(
                null,
                new Money(100)
            );


        Assert.AreEqual(
            100m,
            loadedMoney.Value
        );
    }


    [Test]
    public void CreateFloorRuntimeModel_WithSave_RestoresFloorState()
    {
        FakeSaveRepository repository =
            new FakeSaveRepository();

        LoadService loadService =
            new LoadService(
                repository
            );


        FloorDefinitionSO definition =
            CreateDefinition(
                id: 2,
                upgradeCount: 4
            );


        GameSaveData saveData =
            new GameSaveData();

        saveData.floors.Add(
            new FloorSaveData
            {
                floorId = 2,
                isUnlocked = true,
                upgradeLevel = 3,
                progress = 0.6f,
                automaticTimeRemaining = 4.5f
            }
        );


        FloorRuntimeModel runtimeModel =
            loadService
                .CreateFloorRuntimeModel(
                    definition,
                    saveData
                );


        Assert.AreEqual(
            2,
            runtimeModel.FloorId
        );

        Assert.IsTrue(
            runtimeModel.IsUnlocked
        );

        Assert.AreEqual(
            3,
            runtimeModel.UpgradeLevel
        );

        Assert.AreEqual(
            0.6f,
            runtimeModel.Progress,
            0.0001f
        );

        Assert.AreEqual(
            4.5f,
            runtimeModel
                .AutomaticTimeRemaining,
            0.0001f
        );
    }


    [Test]
    public void CreateFloorRuntimeModel_WithoutSave_CreatesDefaultState()
    {
        FakeSaveRepository repository =
            new FakeSaveRepository();

        LoadService loadService =
            new LoadService(
                repository
            );


        FloorDefinitionSO floor1Definition =
            CreateDefinition(
                id: 1,
                upgradeCount: 4
            );

        FloorDefinitionSO floor2Definition =
            CreateDefinition(
                id: 2,
                upgradeCount: 4
            );


        FloorRuntimeModel floor1 =
            loadService
                .CreateFloorRuntimeModel(
                    floor1Definition,
                    null
                );

        FloorRuntimeModel floor2 =
            loadService
                .CreateFloorRuntimeModel(
                    floor2Definition,
                    null
                );


        Assert.IsTrue(
            floor1.IsUnlocked
        );

        Assert.IsFalse(
            floor2.IsUnlocked
        );
    }


    private FloorDefinitionSO CreateDefinition(
        int id,
        int upgradeCount)
    {
        FloorDefinitionSO definition =
            ScriptableObject.CreateInstance<
                FloorDefinitionSO
            >();

        SetPrivateField(
            definition,
            "id",
            id
        );

        UpgradeDefinition[] upgrades =
            new UpgradeDefinition[
                upgradeCount
            ];

        for (int i = 0;
             i < upgradeCount;
             i++)
        {
            upgrades[i] =
                new UpgradeDefinition();
        }

        SetPrivateField(
            definition,
            "upgrades",
            upgrades
        );

        return definition;
    }


    private void SetPrivateField<T>(
        object target,
        string fieldName,
        T value)
    {
        FieldInfo field =
            target.GetType().GetField(
                fieldName,
                BindingFlags.Instance
                | BindingFlags.NonPublic
            );

        Assert.IsNotNull(
            field
        );

        field.SetValue(
            target,
            value
        );
    }
}