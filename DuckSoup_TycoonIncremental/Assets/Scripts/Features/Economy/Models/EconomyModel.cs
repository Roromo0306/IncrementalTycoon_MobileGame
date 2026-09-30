public class EconomyModel
{
    public Money CurrentMoney { get; private set; }

    public EconomyModel(Money startingMoney)
    {
        CurrentMoney = startingMoney;
    }

    public void SetMoney(Money money)
    {
        CurrentMoney = money;
    }
}