using System;

public readonly struct IncomeContext
{
    public FloorDefinitionSO Definition { get; }

    public FloorRuntimeModel RuntimeModel { get; }


    public IncomeContext(FloorDefinitionSO definition,FloorRuntimeModel runtimeModel)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));

        RuntimeModel = runtimeModel ?? throw new ArgumentNullException(nameof(runtimeModel));
    }
}