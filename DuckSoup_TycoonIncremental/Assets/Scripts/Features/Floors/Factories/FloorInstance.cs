using System;

public class FloorInstance : IDisposable
{
    public FloorDefinitionSO Definition { get; }

    public FloorRuntimeModel RuntimeModel { get; }

    private readonly FloorController floorController;
    private readonly FloorVisualController floorVisualController;

    private bool isDisposed;


    public FloorInstance(FloorDefinitionSO definition,FloorRuntimeModel runtimeModel,FloorController floorController,FloorVisualController floorVisualController)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));

        RuntimeModel = runtimeModel ?? throw new ArgumentNullException(nameof(runtimeModel));

        this.floorController = floorController ?? throw new ArgumentNullException(nameof(floorController));

        this.floorVisualController = floorVisualController ?? throw new ArgumentNullException(nameof(floorVisualController));
    }


    public void Tick()
    {
        floorController.Tick();
    }


    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        floorController.Dispose();
        floorVisualController.Dispose();

        isDisposed = true;
    }
}