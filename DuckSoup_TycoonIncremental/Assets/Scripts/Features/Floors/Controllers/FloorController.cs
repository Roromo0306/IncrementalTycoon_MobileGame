using System;

public class FloorController : IDisposable
{
    private readonly FloorDefinitionSO definition;
    private readonly FloorRuntimeModel runtimeModel;
    private readonly FloorView view;
    private readonly FloorStateMachine stateMachine;

    private bool isDisposed;


    public FloorController(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel, FloorView view, FloorStateMachine stateMachine)
    {
        this.definition = definition ?? throw new ArgumentNullException(nameof(definition));

        this.runtimeModel = runtimeModel ?? throw new ArgumentNullException(nameof(runtimeModel));

        this.view = view ?? throw new ArgumentNullException(nameof(view));

        this.stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));

        SubscribeToView();

        Render();
    }


    private void SubscribeToView()
    {
        view.GeneratePressed += OnGeneratePressed;
    }


    private void OnGeneratePressed()
    {
        stateMachine.HandleGenerate();

        Render();
    }


    public void Render()
    {
        view.SetIncome(definition.BaseIncome.ToFormattedString());

        view.SetProgress(runtimeModel.Progress);

        view.SetGenerateButtonInteractable(stateMachine.CanGenerate);

        view.SetUpgradeButtonInteractable(false);

        UnityEngine.Debug.Log($"Floor State: {stateMachine.CurrentStateType}");
    }


    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        view.GeneratePressed -= OnGeneratePressed;

        isDisposed = true;
    }
}