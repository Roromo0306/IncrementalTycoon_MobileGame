public interface IEconomyService
{
    Money CurrentMoney { get; }

    void AddMoney(Money amount);

    bool CanAfford(Money cost);

    bool TrySpend(Money cost);
}