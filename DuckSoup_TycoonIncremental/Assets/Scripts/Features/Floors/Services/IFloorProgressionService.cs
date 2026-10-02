using System.Collections.Generic;

public interface IFloorProgressionService
{
    bool CanUnlockFloor(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel, IReadOnlyDictionary<int, FloorRuntimeModel> floors);

    bool TryUnlockFloor(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel, IReadOnlyDictionary<int, FloorRuntimeModel> floors);
}