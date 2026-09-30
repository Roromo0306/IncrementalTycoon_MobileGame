using UnityEngine;

public class GameCompositionRoot
{
    private IEventBus eventBus;

    private EconomyModel economyModel;
    private IEconomyService economyService;

    public void Build()
    {
        CreateCoreServices();
        CreateGameServices();

        Debug.Log("GameCompositionRoot: Dependencies created.");
    }

    private void CreateCoreServices()
    {
        eventBus = new EventBus();
    }

    private void CreateGameServices()
    {
        economyModel = new EconomyModel(new Money(100));

        economyService =new EconomyService(economyModel,eventBus);
    }

}