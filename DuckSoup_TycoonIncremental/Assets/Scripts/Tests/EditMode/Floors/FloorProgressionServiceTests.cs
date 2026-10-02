using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class FloorProgressionServiceTests
{
    private EventBus eventBus;
    private EconomyService economyService;
    private FloorProgressionService progressionService;


    [SetUp]
    public void SetUp()
    {
        eventBus = new EventBus();

        EconomyModel economyModel = new EconomyModel(new Money(100));

        economyService = new EconomyService(economyModel,eventBus);

        progressionService = new FloorProgressionService(economyService,eventBus);
    }


    [Test]
    public void TryUnlockFloor_WithRequirementAndMoney_UnlocksFloor()
    {
        FloorRuntimeModel floor1 = new FloorRuntimeModel(1, true);

        FloorRuntimeModel floor2 = new FloorRuntimeModel(2, false);

        FloorDefinitionSO definition = CreateDefinition(id: 2, unlockCost: 50,requiredFloorId: 1);

        Dictionary<int, FloorRuntimeModel> floors = new Dictionary<int, FloorRuntimeModel>{{ 1, floor1 },{ 2, floor2 }};

        int eventCount = 0;

        eventBus.Subscribe<FloorUnlockedEvent>(eventData =>{eventCount++;});

        bool result = progressionService.TryUnlockFloor(definition,floor2,floors);

        Assert.IsTrue(result);

        Assert.IsTrue(floor2.IsUnlocked);

        Assert.AreEqual(50m,economyService.CurrentMoney.Value);

        Assert.AreEqual(1,eventCount);
    }


    [Test]
    public void TryUnlockFloor_WhenRequirementIsLocked_DoesNotUnlock()
    {
        FloorRuntimeModel floor1 = new FloorRuntimeModel(1,false);

        FloorRuntimeModel floor2 = new FloorRuntimeModel(2,false);

        FloorDefinitionSO definition = CreateDefinition(id: 2,unlockCost: 50,requiredFloorId: 1);

        Dictionary<int, FloorRuntimeModel> floors = new Dictionary<int, FloorRuntimeModel>{{ 1, floor1 },{ 2, floor2 }};

        bool result = progressionService.TryUnlockFloor(definition,floor2,floors);

        Assert.IsFalse(result);

        Assert.IsFalse(floor2.IsUnlocked);

        Assert.AreEqual(100m,economyService.CurrentMoney.Value);
    }


    [Test]
    public void TryUnlockFloor_WithoutEnoughMoney_DoesNotUnlock()
    {
        FloorRuntimeModel floor1 = new FloorRuntimeModel(1,true);

        FloorRuntimeModel floor2 = new FloorRuntimeModel(2,false);

        FloorDefinitionSO definition = CreateDefinition(id: 2,unlockCost: 200,requiredFloorId: 1);

        Dictionary<int, FloorRuntimeModel> floors = new Dictionary<int, FloorRuntimeModel>{{ 1, floor1 },{ 2, floor2 }};

        bool result = progressionService.TryUnlockFloor(definition,floor2,floors);

        Assert.IsFalse(result);

        Assert.IsFalse(floor2.IsUnlocked);

        Assert.AreEqual(100m,economyService.CurrentMoney.Value);
    }


    [Test]
    public void TryUnlockFloor_WhenAlreadyUnlocked_DoesNotSpendAgain()
    {
        FloorRuntimeModel floor1 = new FloorRuntimeModel(1,true);

        FloorRuntimeModel floor2 = new FloorRuntimeModel(2,false);

        FloorDefinitionSO definition = CreateDefinition(id: 2,unlockCost: 50,requiredFloorId: 1);

        Dictionary<int, FloorRuntimeModel> floors = new Dictionary<int, FloorRuntimeModel>{{ 1, floor1 },{ 2, floor2 }};

        bool firstResult = progressionService.TryUnlockFloor(definition,floor2,floors);

        bool secondResult = progressionService.TryUnlockFloor(definition,floor2,floors);

        Assert.IsTrue(firstResult);

        Assert.IsFalse(secondResult);

        Assert.AreEqual(50m,economyService.CurrentMoney.Value);
    }


    private FloorDefinitionSO CreateDefinition(int id,double unlockCost,int requiredFloorId)
    {
        FloorDefinitionSO definition =ScriptableObject.CreateInstance<FloorDefinitionSO>();

        SetPrivateField(definition,"id",id);

        SetPrivateField(definition,"unlockCost",unlockCost);

        SetPrivateField(definition,"requiredFloorId",requiredFloorId);

        return definition;
    }


    private void SetPrivateField<T>(object target,string fieldName,T value)
    {
        FieldInfo field = target.GetType().GetField(fieldName,BindingFlags.Instance| BindingFlags.NonPublic);

        Assert.IsNotNull(field,$"Field '{fieldName}' was not found.");

        field.SetValue(target,value);
    }
}