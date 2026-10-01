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

        float newProgress = runtimeModel.Progress + definition.ProgressPerTap;

        runtimeModel.SetProgress(newProgress);

        if (runtimeModel.Progress < 1f)
        {
            return false;
        }

        economyService.AddMoney(definition.BaseIncome);

        return true;
    }


    public void GenerateAutomaticIncome(FloorDefinitionSO definition)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        economyService.AddMoney(definition.BaseIncome);
    }
}