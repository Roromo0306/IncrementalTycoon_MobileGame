public class EconomyService : IEconomyService
{
    private readonly EconomyModel economyModel;
    private readonly IEventBus eventBus;

    public Money CurrentMoney => economyModel.CurrentMoney;

    public EconomyService(EconomyModel economyModel,IEventBus eventBus)
    {
        this.economyModel = economyModel;
        this.eventBus = eventBus;
    }

    public void AddMoney(Money amount)
    {
        Money newBalance =economyModel.CurrentMoney.Add(amount);

        economyModel.SetMoney(newBalance);

        PublishMoneyChanged();
    }

    public bool CanAfford(Money cost)
    {
        return economyModel.CurrentMoney.IsGreaterThanOrEqual(cost);
    }

    public bool TrySpend(Money cost)
    {
        if (!CanAfford(cost))
        {
            return false;
        }

        Money newBalance = economyModel.CurrentMoney.Subtract(cost);

        economyModel.SetMoney(newBalance);

        PublishMoneyChanged();

        return true;
    }

    private void PublishMoneyChanged()
    {
        eventBus.Publish(new MoneyChangedEvent(economyModel.CurrentMoney)
        );
    }
}