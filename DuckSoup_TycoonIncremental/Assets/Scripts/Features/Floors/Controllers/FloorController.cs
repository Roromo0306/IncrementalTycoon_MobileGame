using System;

public class FloorController : IDisposable
{
    private readonly FloorDefinitionSO definition;
    private readonly FloorRuntimeModel runtimeModel;
    private readonly FloorView view;
    private readonly FloorStateMachine stateMachine;
    private readonly IUpgradeService upgradeService;
    private readonly IIncomeCalculator incomeCalculator;

    private bool isDisposed;


    public FloorController(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel, FloorView view, FloorStateMachine stateMachine, IUpgradeService upgradeService, IIncomeCalculator incomeCalculator)
    {
        this.definition = definition ?? throw new ArgumentNullException(nameof(definition));

        this.runtimeModel = runtimeModel
            ?? throw new ArgumentNullException(
                nameof(runtimeModel)
            );

        this.view = view
            ?? throw new ArgumentNullException(
                nameof(view)
            );

        this.stateMachine = stateMachine
            ?? throw new ArgumentNullException(
                nameof(stateMachine)
            );

        this.upgradeService = upgradeService ?? throw new ArgumentNullException(nameof(upgradeService));

        this.incomeCalculator = incomeCalculator ?? throw new ArgumentNullException(nameof(incomeCalculator));

        SubscribeToView();

        RenderStaticData();
        RenderDynamicData();
        this.upgradeService = upgradeService;
        this.incomeCalculator = incomeCalculator;
    }


    private void SubscribeToView()
    {
        view.GeneratePressed += OnGeneratePressed;

        view.UpgradePressed += OnUpgradePressed;
    }


    private void OnGeneratePressed()
    {
        stateMachine.HandleGenerate();

        RenderDynamicData();
    }


    public void Tick()
    {
        stateMachine.Tick();

        RenderDynamicData();
    }


    private void RenderStaticData()
    {
        view.SetIncome(definition.BaseIncome.ToFormattedString());

        view.SetUpgradeButtonInteractable(upgradeService.CanPurchaseUpgrade(definition, runtimeModel));
    }


    private void RenderDynamicData()
    {
        IncomeContext incomeContext = new IncomeContext(definition, runtimeModel);

        Money currentIncome = incomeCalculator.Calculate(definition.BaseIncome, incomeContext);

        view.SetIncome(
            currentIncome.ToFormattedString()
        );

        view.SetProgress(
            runtimeModel.Progress
        );

        view.SetGenerateButtonInteractable(
            stateMachine.CanGenerate
        );

        bool isAutomatic =
            stateMachine.CurrentStateType
            == FloorStateType.Automatic;

        view.SetAutomaticVisual(
            isAutomatic
        );

        view.SetAutomaticTime(
            runtimeModel.AutomaticTimeRemaining,
            isAutomatic
        );
    }


    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        view.GeneratePressed -=
            OnGeneratePressed;

        view.UpgradePressed -= OnUpgradePressed;

        isDisposed = true;
    }

    private void OnUpgradePressed()
    {
        upgradeService.TryPurchaseUpgrade(
            definition,
            runtimeModel
        );

        RenderDynamicData();
    }
}