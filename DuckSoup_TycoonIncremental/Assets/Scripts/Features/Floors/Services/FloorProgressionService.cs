using System;
using System.Collections.Generic;

public class FloorProgressionService : IFloorProgressionService
{
    private readonly IEconomyService economyService;
    private readonly IEventBus eventBus;


    public FloorProgressionService(IEconomyService economyService, IEventBus eventBus)
    {
        this.economyService = economyService ?? throw new ArgumentNullException(nameof(economyService));

        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }


    public bool CanUnlockFloor(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel, IReadOnlyDictionary<int, FloorRuntimeModel> floors)
    {
        ValidateParameters(definition, runtimeModel, floors);

        if (runtimeModel.IsUnlocked)
        {
            return false;
        }

        if (!MeetsUnlockRequirement(definition, floors))
        {
            return false;
        }

        return economyService.CanAfford(definition.UnlockCost);
    }


    public bool TryUnlockFloor(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel, IReadOnlyDictionary<int, FloorRuntimeModel> floors)
    {
        if (!CanUnlockFloor(definition, runtimeModel, floors))
        {
            return false;
        }

        if (!economyService.TrySpend(definition.UnlockCost))
        {
            return false;
        }

        runtimeModel.SetUnlocked(true);

        eventBus.Publish(new FloorUnlockedEvent(runtimeModel.FloorId));

        return true;
    }


    private bool MeetsUnlockRequirement(FloorDefinitionSO definition, IReadOnlyDictionary<int, FloorRuntimeModel> floors)
    {
        int requiredFloorId = definition.RequiredFloorId;

        if (requiredFloorId <= 0)
        {
            return true;
        }

        if (!floors.TryGetValue(requiredFloorId, out FloorRuntimeModel requiredFloor))
        {
            return false;
        }

        return requiredFloor.IsUnlocked;
    }


    private void ValidateParameters(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel, IReadOnlyDictionary<int, FloorRuntimeModel> floors)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        if (runtimeModel == null)
        {
            throw new ArgumentNullException(nameof(runtimeModel));
        }

        if (floors == null)
        {
            throw new ArgumentNullException(nameof(floors));
        }

        if (definition.Id != runtimeModel.FloorId)
        {
            throw new ArgumentException("Floor definition and runtime model must use the same Floor ID.");
        }
    }
}