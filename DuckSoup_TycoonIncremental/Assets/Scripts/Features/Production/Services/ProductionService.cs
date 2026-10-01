using System;

public class ProductionService
    : IProductionService
{
    private readonly IEconomyService economyService;
    private readonly IIncomeCalculator incomeCalculator;


    public ProductionService(
        IEconomyService economyService,
        IIncomeCalculator incomeCalculator)
    {
        this.economyService = economyService
            ?? throw new ArgumentNullException(
                nameof(economyService)
            );

        this.incomeCalculator = incomeCalculator
            ?? throw new ArgumentNullException(
                nameof(incomeCalculator)
            );
    }


    public bool GenerateManual(
        FloorDefinitionSO definition,
        FloorRuntimeModel runtimeModel)
    {
        ValidateParameters(
            definition,
            runtimeModel
        );

        float newProgress =
            runtimeModel.Progress
            + definition.ProgressPerTap;

        runtimeModel.SetProgress(
            newProgress
        );

        if (runtimeModel.Progress < 1f)
        {
            return false;
        }

        AddIncome(
            definition,
            runtimeModel
        );

        return true;
    }


    public void GenerateAutomaticIncome(
        FloorDefinitionSO definition,
        FloorRuntimeModel runtimeModel)
    {
        ValidateParameters(
            definition,
            runtimeModel
        );

        AddIncome(
            definition,
            runtimeModel
        );
    }


    private void AddIncome(
        FloorDefinitionSO definition,
        FloorRuntimeModel runtimeModel)
    {
        IncomeContext context =
            new IncomeContext(
                definition,
                runtimeModel
            );

        Money finalIncome =
            incomeCalculator.Calculate(
                definition.BaseIncome,
                context
            );

        economyService.AddMoney(
            finalIncome
        );
    }


    private void ValidateParameters(
        FloorDefinitionSO definition,
        FloorRuntimeModel runtimeModel)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(
                nameof(definition)
            );
        }

        if (runtimeModel == null)
        {
            throw new ArgumentNullException(
                nameof(runtimeModel)
            );
        }
    }
}