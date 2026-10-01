using System;

public class EconomyController : IDisposable
{
    private readonly IEconomyService economyService;
    private readonly IEventBus eventBus;
    private readonly EconomyView view;

    private bool isDisposed;

    public EconomyController(IEconomyService economyService,IEventBus eventBus, EconomyView view)
    {
        this.economyService = economyService ?? throw new ArgumentNullException(nameof(economyService));

        this.eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

        this.view = view ?? throw new ArgumentNullException(nameof(view));

        eventBus.Subscribe<MoneyChangedEvent>(OnMoneyChanged);

        Render();
    }

    private void OnMoneyChanged(MoneyChangedEvent moneyChangedEvent)
    {
        view.SetMoney(moneyChangedEvent.CurrentMoney.ToFormattedString());
    }

    private void Render()
    {
        view.SetMoney(economyService.CurrentMoney.ToFormattedString());
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        eventBus.Unsubscribe<MoneyChangedEvent>(OnMoneyChanged);

        isDisposed = true;
    }
}