using System;
using System.Collections.Generic;

public class FloorController : IDisposable
{
    private readonly FloorDefinitionSO definition;
    private readonly FloorRuntimeModel runtimeModel;
    private readonly FloorView view;
    private readonly FloorStateMachine stateMachine;
    private readonly IUpgradeService upgradeService;
    private readonly IIncomeCalculator incomeCalculator;
    private readonly IFloorProgressionService floorProgressionService;
    private readonly IReadOnlyDictionary<int, FloorRuntimeModel>allRuntimeModels;

    private bool isDisposed;


    public FloorController(FloorDefinitionSO definition,FloorRuntimeModel runtimeModel,FloorView view,FloorStateMachine stateMachine,IUpgradeService upgradeService,IIncomeCalculator incomeCalculator,IFloorProgressionService floorProgressionService,IReadOnlyDictionary<int, FloorRuntimeModel> allRuntimeModels)
    {
        this.definition = definition ?? throw new ArgumentNullException(nameof(definition));

        this.runtimeModel = runtimeModel ?? throw new ArgumentNullException(nameof(runtimeModel));

        this.view = view ?? throw new ArgumentNullException(nameof(view));

        this.stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));

        this.upgradeService = upgradeService ?? throw new ArgumentNullException(nameof(upgradeService));

        this.incomeCalculator = incomeCalculator?? throw new ArgumentNullException(nameof(incomeCalculator));

        this.floorProgressionService = floorProgressionService?? throw new ArgumentNullException(nameof(floorProgressionService));

        this.allRuntimeModels = allRuntimeModels ?? throw new ArgumentNullException(nameof(allRuntimeModels));

        SubscribeToView();

        RenderStaticData();
        RenderDynamicData();
    }


    private void SubscribeToView()
    {
        view.GeneratePressed += OnGeneratePressed;

        view.UpgradePressed += OnUpgradePressed;

        view.UnlockPressed += OnUnlockPressed;
    }


    private void OnGeneratePressed()
    {
        stateMachine.HandleGenerate();

        RenderDynamicData();
    }


    private void OnUpgradePressed()
    {
        upgradeService.TryPurchaseUpgrade(definition,runtimeModel);

        RenderDynamicData();
    }


    private void OnUnlockPressed()
    {
        bool unlocked = floorProgressionService.TryUnlockFloor(definition,runtimeModel,allRuntimeModels);

        if (unlocked)
        {
            stateMachine.ChangeToManual();
        }

        RenderDynamicData();
    }


    public void Tick()
    {
        stateMachine.Tick();

        RenderDynamicData();
    }


    private void RenderStaticData()
    {
        view.SetFloorName(definition.FloorName);
    }


    private void RenderDynamicData()
    {
        IncomeContext incomeContext = new IncomeContext(definition,runtimeModel);

        Money currentIncome = incomeCalculator.Calculate(definition.BaseIncome,incomeContext);

        view.SetIncome(currentIncome.ToFormattedString());

        view.SetProgress(runtimeModel.Progress);

        view.SetGenerateButtonInteractable(stateMachine.CanGenerate);

        view.SetUpgradeButtonInteractable(upgradeService.CanPurchaseUpgrade(definition,runtimeModel));

        bool isAutomatic = stateMachine.CurrentStateType == FloorStateType.Automatic;

        view.SetAutomaticVisual(isAutomatic);

        view.SetAutomaticTime(runtimeModel.AutomaticTimeRemaining,isAutomatic);

        bool isLocked = !runtimeModel.IsUnlocked;

        bool canUnlock =isLocked&& floorProgressionService.CanUnlockFloor(definition,runtimeModel,allRuntimeModels);

        view.SetLockedState(isLocked,definition.UnlockCost.ToFormattedString(),canUnlock);
    }


    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        view.GeneratePressed -= OnGeneratePressed;

        view.UpgradePressed -= OnUpgradePressed;

        view.UnlockPressed -= OnUnlockPressed;

        isDisposed = true;
    }
}