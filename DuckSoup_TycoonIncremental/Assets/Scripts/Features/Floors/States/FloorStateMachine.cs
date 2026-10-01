using System;

public class FloorStateMachine
{
    private readonly FloorLockedState lockedState;
    private readonly FloorManualState manualState;
    private readonly FloorAutomaticState automaticState;

    private IFloorState currentState;


    public FloorStateType CurrentStateType => currentState.StateType;

    public bool CanGenerate => currentState.CanGenerate;


    public FloorStateMachine(FloorDefinitionSO definition, FloorRuntimeModel runtimeModel, IProductionService productionService)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        if (runtimeModel == null)
        {
            throw new ArgumentNullException(nameof(runtimeModel));
        }

        if (productionService == null)
        {
            throw new ArgumentNullException(nameof(productionService));
        }

        lockedState = new FloorLockedState();

        manualState = new FloorManualState(this, definition, runtimeModel, productionService);

        automaticState = new FloorAutomaticState(definition, runtimeModel);

        if (runtimeModel.IsUnlocked)
        {
            ChangeState(manualState);
        }
        else
        {
            ChangeState(lockedState);
        }
    }


    public void HandleGenerate()
    {
        currentState.HandleGenerate();
    }


    public void ChangeToLocked()
    {
        ChangeState(lockedState);
    }


    public void ChangeToManual()
    {
        ChangeState(manualState);
    }


    public void ChangeToAutomatic()
    {
        ChangeState(automaticState);
    }


    private void ChangeState(IFloorState newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState.Enter();
    }
}