using UnityEngine;

public class GameCompositionRoot
{
    private IEventBus eventBus;

    public void Build()
    {
        CreateCoreServices();

        Debug.Log("GameCompositionRoot: Dependencies created.");
    }

    private void CreateCoreServices()
    {
        eventBus = new EventBus();
    }
}