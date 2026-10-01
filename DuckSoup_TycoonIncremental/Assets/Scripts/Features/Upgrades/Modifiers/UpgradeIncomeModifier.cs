public class UpgradeIncomeModifier: IIncomeModifier
{
    public Money Apply(Money currentValue, IncomeContext context)
    {
        int upgradeLevel = context.RuntimeModel.UpgradeLevel;

        if (upgradeLevel <= 0)
        {
            return currentValue;
        }

        UpgradeDefinition[] upgrades = context.Definition.Upgrades;

        if (upgrades == null || upgrades.Length == 0)
        {
            return currentValue;
        }

        int upgradeIndex = upgradeLevel - 1;

        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Length)
        {
            return currentValue;
        }

        UpgradeDefinition upgrade = upgrades[upgradeIndex];

        if (upgrade == null)
        {
            return currentValue;
        }

        return currentValue.Multiply(upgrade.IncomeMultiplier);
    }
}