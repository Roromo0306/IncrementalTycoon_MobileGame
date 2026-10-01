public interface IUpgradeService
{
    bool CanPurchaseUpgrade(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel);

    bool TryPurchaseUpgrade(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel
    );

    UpgradeDefinition GetNextUpgrade(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel);
}