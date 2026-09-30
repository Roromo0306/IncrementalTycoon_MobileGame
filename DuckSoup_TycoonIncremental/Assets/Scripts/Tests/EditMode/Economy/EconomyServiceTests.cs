using NUnit.Framework;

public class EconomyServiceTests
{
    private EventBus eventBus;
    private EconomyModel economyModel;
    private EconomyService economyService;

    [SetUp]
    public void SetUp()
    {
        eventBus = new EventBus();

        economyModel = new EconomyModel(
            new Money(100)
        );

        economyService = new EconomyService(
            economyModel,
            eventBus
        );
    }

    [Test]
    public void AddMoney_IncreasesCurrentMoney()
    {
        economyService.AddMoney(
            new Money(50)
        );

        Assert.AreEqual(
            150m,
            economyService.CurrentMoney.Value
        );
    }

    [Test]
    public void TrySpend_WithEnoughMoney_ReturnsTrue()
    {
        bool result = economyService.TrySpend(
            new Money(50)
        );

        Assert.IsTrue(result);

        Assert.AreEqual(
            50m,
            economyService.CurrentMoney.Value
        );
    }

    [Test]
    public void TrySpend_WithoutEnoughMoney_ReturnsFalse()
    {
        bool result = economyService.TrySpend(
            new Money(200)
        );

        Assert.IsFalse(result);

        Assert.AreEqual(
            100m,
            economyService.CurrentMoney.Value
        );
    }

    [Test]
    public void CanAfford_WithExactAmount_ReturnsTrue()
    {
        bool result = economyService.CanAfford(
            new Money(100)
        );

        Assert.IsTrue(result);
    }

    [Test]
    public void AddMoney_PublishesMoneyChangedEvent()
    {
        Money receivedMoney = new Money(0);

        eventBus.Subscribe<MoneyChangedEvent>(
            eventData =>
            {
                receivedMoney =
                    eventData.CurrentMoney;
            }
        );

        economyService.AddMoney(
            new Money(50)
        );

        Assert.AreEqual(
            150m,
            receivedMoney.Value
        );
    }

    [Test]
    public void TrySpend_WithoutEnoughMoney_DoesNotPublishMoneyChangedEvent()
    {
        bool eventReceived = false;

        eventBus.Subscribe<MoneyChangedEvent>(
            eventData =>
            {
                eventReceived = true;
            }
        );

        economyService.TrySpend(
            new Money(200)
        );

        Assert.IsFalse(eventReceived);
    }
}