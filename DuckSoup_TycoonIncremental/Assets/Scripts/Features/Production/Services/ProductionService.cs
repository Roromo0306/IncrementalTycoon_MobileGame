using System;

public class ProductionService : IProductionService
{
    private readonly IEconomyService economyService;


    public ProductionService(IEconomyService economyService)
    {
        this.economyService = economyService ?? throw new ArgumentNullException(nameof(economyService));
    }


    public bool GenerateManual(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        if (runtimeModel == null)
        {
            throw new ArgumentNullException(nameof(runtimeModel));
        }

        AddManualProgress(definition, runtimeModel);

        if (runtimeModel.Progress < 1f)
        {
            return false;
        }

        CompleteManualProduction(
            definition
        );

        return true;
    }


    private void AddManualProgress(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel)
    {
        float newProgress = runtimeModel.Progress + definition.ProgressPerTap;

        runtimeModel.SetProgress(newProgress);
    }


    private void CompleteManualProduction(FloorDefinitionSO definition)
    {
        economyService.AddMoney(definition.BaseIncome);
    }
}