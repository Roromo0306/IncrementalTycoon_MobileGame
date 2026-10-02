using System;

public class FloorVisualController : IDisposable
{
    private readonly FloorDefinitionSO definition;
    private readonly FloorRuntimeModel runtimeModel;
    private readonly FloorView view;
    private readonly IEventBus eventBus;

    private bool isDisposed;


    public FloorVisualController(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel, FloorView view, IEventBus eventBus)
    {
        this.definition = definition ?? throw new ArgumentNullException(nameof(definition));

        this.runtimeModel = runtimeModel ?? throw new ArgumentNullException(nameof(runtimeModel));

        this.view = view ?? throw new ArgumentNullException(nameof(view));

        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

        SubscribeToEvents();

        RenderCurrentVisualStage();
    }


    private void SubscribeToEvents()
    {
        eventBus.Subscribe<UpgradePurchasedEvent>(OnUpgradePurchased);
    }


    private void OnUpgradePurchased(UpgradePurchasedEvent upgradeEvent)
    {
        if (upgradeEvent.FloorId!= runtimeModel.FloorId)
        {
            return;
        }

        view.SetVisualStage(upgradeEvent.VisualStage);
    }


    private void RenderCurrentVisualStage()
    {
        int visualStage = GetCurrentVisualStage();

        view.SetVisualStage(visualStage);
    }


    private int GetCurrentVisualStage()
    {
        int upgradeLevel = runtimeModel.UpgradeLevel;

        if (upgradeLevel <= 0)
        {
            return 0;
        }

        UpgradeDefinition[] upgrades = definition.Upgrades;

        if (upgrades == null || upgrades.Length == 0)
        {
            return 0;
        }

        int upgradeIndex = upgradeLevel - 1;

        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Length)
        {
            return 0;
        }

        UpgradeDefinition upgrade = upgrades[upgradeIndex];

        if (upgrade == null)
        {
            return 0;
        }

        return upgrade.VisualStage;
    }


    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        eventBus.Unsubscribe<UpgradePurchasedEvent>(OnUpgradePurchased);

        isDisposed = true;
    }
}