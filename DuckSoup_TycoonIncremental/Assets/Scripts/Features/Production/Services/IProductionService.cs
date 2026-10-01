public interface IProductionService
{
    bool GenerateManual(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel);

    void GenerateAutomaticIncome(FloorDefinitionSO definition);
}