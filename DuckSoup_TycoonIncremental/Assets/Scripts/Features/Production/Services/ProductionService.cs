using System;

public class ProductionService : IProductionService
{
    private readonly IEconomyService economyService;


    public ProductionService(IEconomyService economyService)
    {
        this.economyService = economyService ?? throw new ArgumentNullException( nameof(economyService));
    }


    public void GenerateManual(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        if (runtimeModel == null)
        {
            throw new ArgumentNullException(nameof(runtimeModel));
        }

        if (!runtimeModel.IsUnlocked)
        {
            return;
        }

        if (runtimeModel.AutomaticTimeRemaining > 0f)
        {
            return;
        }

        AddManualProgress(definition, runtimeModel);

        if (runtimeModel.Progress < 1f)
        {
            return;
        }

        CompleteManualProduction(definition, runtimeModel);
    }


    private void AddManualProgress(FloorDefinitionSO definition,FloorRuntimeModel runtimeModel)
    {
        float newProgress =runtimeModel.Progress + definition.ProgressPerTap;

        runtimeModel.SetProgress(newProgress);
    }


    private void CompleteManualProduction(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel)
    {
        economyService.AddMoney(definition.BaseIncome);

        runtimeModel.SetAutomaticTimeRemaining(definition.AutomaticDuration);
    }
}