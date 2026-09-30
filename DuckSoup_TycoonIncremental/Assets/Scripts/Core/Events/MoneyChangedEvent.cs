public readonly struct MoneyChangedEvent
{
    public readonly Money CurrentMoney;

    public MoneyChangedEvent(Money currentMoney)
    {
        CurrentMoney = currentMoney;
    }

}