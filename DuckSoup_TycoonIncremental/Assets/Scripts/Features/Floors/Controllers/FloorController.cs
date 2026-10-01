using System;

public class FloorController : IDisposable
{
    private readonly FloorDefinitionSO definition;
    private readonly FloorRuntimeModel runtimeModel;
    private readonly FloorView view;
    private readonly IProductionService productionService;

    private bool isDisposed;


    public FloorController(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel, FloorView view, IProductionService productionService)
    {
        this.definition = definition ?? throw new ArgumentNullException(nameof(definition));

        this.runtimeModel = runtimeModel ?? throw new ArgumentNullException(nameof(runtimeModel));

        this.view = view ?? throw new ArgumentNullException(nameof(view));

        this.productionService = productionService ?? throw new ArgumentNullException(nameof(productionService));

        SubscribeToView();

        Render();
    }


    private void SubscribeToView()
    {
        view.GeneratePressed += OnGeneratePressed;
    }


    private void OnGeneratePressed()
    {
        if (!runtimeModel.IsUnlocked)
        {
            return;
        }

        productionService.GenerateManual(definition, runtimeModel);

        Render();
    }


    public void Render()
    {
        view.SetIncome(definition.BaseIncome.ToFormattedString());

        view.SetProgress(runtimeModel.Progress);

        bool canGenerate =runtimeModel.IsUnlocked && runtimeModel.AutomaticTimeRemaining <= 0f;

        view.SetGenerateButtonInteractable(canGenerate);

        view.SetUpgradeButtonInteractable(false);
    }


    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        view.GeneratePressed -= OnGeneratePressed;

        isDisposed = true;
    }
}