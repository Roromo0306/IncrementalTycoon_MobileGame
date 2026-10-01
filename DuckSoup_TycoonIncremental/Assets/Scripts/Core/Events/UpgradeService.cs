using System;

public class UpgradeService : IUpgradeService
{
    private readonly IEconomyService economyService;
    private readonly IEventBus eventBus;


    public UpgradeService(IEconomyService economyService, IEventBus eventBus)
    {
        this.economyService = economyService ?? throw new ArgumentNullException(nameof(economyService));

        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }


    public bool CanPurchaseUpgrade(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel)
    {
        UpgradeDefinition nextUpgrade = GetNextUpgrade(definition, runtimeModel);

        if (nextUpgrade == null)
        {
            return false;
        }

        return economyService.CanAfford(nextUpgrade.Cost);
    }


    public bool TryPurchaseUpgrade(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel)
    {
        UpgradeDefinition nextUpgrade = GetNextUpgrade(definition, runtimeModel);

        if (nextUpgrade == null)
        {
            return false;
        }

        if (!economyService.TrySpend(nextUpgrade.Cost))
        {
            return false;
        }

        runtimeModel.SetUpgradeLevel(nextUpgrade.Level);

        eventBus.Publish(new UpgradePurchasedEvent(runtimeModel.FloorId, nextUpgrade.Level, nextUpgrade.VisualStage));

        return true;
    }


    public UpgradeDefinition GetNextUpgrade(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        if (runtimeModel == null)
        {
            throw new ArgumentNullException(nameof(runtimeModel));
        }

        int nextUpgradeIndex = runtimeModel.UpgradeLevel;

        if (definition.Upgrades == null)
        {
            return null;
        }

        if (nextUpgradeIndex < 0 || nextUpgradeIndex >= definition.Upgrades.Length)
        {
            return null;
        }

        return definition.Upgrades[nextUpgradeIndex];
    }
}