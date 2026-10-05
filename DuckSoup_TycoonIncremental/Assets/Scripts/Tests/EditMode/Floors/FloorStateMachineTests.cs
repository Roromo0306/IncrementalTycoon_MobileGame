using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class FloorStateMachineTests
{
    [Test]
    public void ManualProgress_DecaysAfterIdleDelay()
    {
        FloorDefinitionSO definition =
            CreateDefinition(
                progressPerTap: 0.1f,
                decayDelay: 1f,
                decayPerSecond: 0.2f,
                automaticDuration: 10f,
                automaticIncomeInterval: 1f
            );

        FloorRuntimeModel runtimeModel =
            new FloorRuntimeModel(
                1,
                true
            );

        runtimeModel.SetProgress(
            0.5f
        );

        FakeTimeService timeService =
            new FakeTimeService(
                new DateTime(
                    2026,
                    1,
                    1,
                    12,
                    0,
                    0,
                    DateTimeKind.Utc
                )
            );

        EconomyService economyService =
            CreateEconomyService();

        ProductionService productionService =
            new ProductionService(
                economyService,
                CreateIncomeCalculator()
            );

        FloorStateMachine stateMachine =
            new FloorStateMachine(
                definition,
                runtimeModel,
                productionService,
                timeService
            );

        // Pasa exactamente el tiempo de espera.
        // Todavía no debería empezar el decay.
        timeService.SetDeltaTime(
            1f
        );

        stateMachine.Tick();

        Assert.AreEqual(
            0.5f,
            runtimeModel.Progress,
            0.0001f
        );


        // Pasan 0.5 segundos adicionales.
        // El decay debería empezar.
        timeService.SetDeltaTime(
            0.5f
        );

        stateMachine.Tick();

        Assert.AreEqual(
            0.4f,
            runtimeModel.Progress,
            0.0001f
        );
    }


    [Test]
    public void AutomaticProduction_EndsAndReturnsToManual()
    {
        FloorDefinitionSO definition =
            CreateDefinition(
                progressPerTap: 1f,
                decayDelay: 1f,
                decayPerSecond: 0.1f,
                automaticDuration: 2f,
                automaticIncomeInterval: 1f
            );

        FloorRuntimeModel runtimeModel =
            new FloorRuntimeModel(
                1,
                true
            );

        FakeTimeService timeService =
            new FakeTimeService(
                new DateTime(
                    2026,
                    1,
                    1,
                    12,
                    0,
                    0,
                    DateTimeKind.Utc
                )
            );

        EconomyService economyService =
            CreateEconomyService();

        ProductionService productionService =
            new ProductionService(
                economyService,
                CreateIncomeCalculator()
            );

        FloorStateMachine stateMachine =
            new FloorStateMachine(
                definition,
                runtimeModel,
                productionService,
                timeService
            );


        // ProgressPerTap = 1, así que un solo Generate
        // completa la barra y entra en Automatic.
        stateMachine.HandleGenerate();


        Assert.AreEqual(
            FloorStateType.Automatic,
            stateMachine.CurrentStateType
        );

        // Empezamos con 100 y completar manual da +5.
        Assert.AreEqual(
            105m,
            economyService.CurrentMoney.Value
        );


        // Primer segundo automático.
        timeService.SetDeltaTime(
            1f
        );

        stateMachine.Tick();

        Assert.AreEqual(
            110m,
            economyService.CurrentMoney.Value
        );


        // Segundo segundo automático.
        timeService.SetDeltaTime(
            1f
        );

        stateMachine.Tick();

        Assert.AreEqual(
            115m,
            economyService.CurrentMoney.Value
        );


        // Debe haber vuelto a Manual.
        Assert.AreEqual(
            FloorStateType.Manual,
            stateMachine.CurrentStateType
        );

        // Al terminar Automatic se limpia el progreso.
        Assert.AreEqual(
            0f,
            runtimeModel.Progress,
            0.0001f
        );

        // Y también el timer.
        Assert.AreEqual(
            0f,
            runtimeModel.AutomaticTimeRemaining,
            0.0001f
        );
    }


    [Test]
    public void Constructor_WithAutomaticTimeRemaining_RestoresAutomaticState()
    {
        FloorDefinitionSO definition =
            CreateDefinition(
                progressPerTap: 0.1f,
                decayDelay: 1f,
                decayPerSecond: 0.1f,
                automaticDuration: 10f,
                automaticIncomeInterval: 1f
            );

        FloorRuntimeModel runtimeModel =
            new FloorRuntimeModel(
                1,
                true
            );

        // Simulamos un piso cargado desde un save
        // que estaba en producción automática.
        runtimeModel.SetProgress(
            1f
        );

        runtimeModel.SetAutomaticTimeRemaining(
            4.5f
        );

        FakeTimeService timeService =
            new FakeTimeService(
                DateTime.UtcNow
            );

        EconomyService economyService =
            CreateEconomyService();

        ProductionService productionService =
            new ProductionService(
                economyService,
                CreateIncomeCalculator()
            );

        FloorStateMachine stateMachine =
            new FloorStateMachine(
                definition,
                runtimeModel,
                productionService,
                timeService
            );


        Assert.AreEqual(
            FloorStateType.Automatic,
            stateMachine.CurrentStateType
        );

        // Muy importante:
        // no debe volver a 10 segundos.
        Assert.AreEqual(
            4.5f,
            runtimeModel.AutomaticTimeRemaining,
            0.0001f
        );
    }


    private EconomyService CreateEconomyService()
    {
        EventBus eventBus =
            new EventBus();

        EconomyModel economyModel =
            new EconomyModel(
                new Money(100)
            );

        return new EconomyService(
            economyModel,
            eventBus
        );
    }


    private IIncomeCalculator CreateIncomeCalculator()
    {
        IIncomeModifier[] modifiers =
        {
            new UpgradeIncomeModifier()
        };

        return new IncomeCalculator(
            modifiers
        );
    }


    private FloorDefinitionSO CreateDefinition(
        float progressPerTap,
        float decayDelay,
        float decayPerSecond,
        float automaticDuration,
        float automaticIncomeInterval)
    {
        FloorDefinitionSO definition =
            ScriptableObject.CreateInstance<
                FloorDefinitionSO
            >();


        SetPrivateField(
            definition,
            "id",
            1
        );

        SetPrivateField(
            definition,
            "baseIncome",
            5d
        );

        SetPrivateField(
            definition,
            "progressPerTap",
            progressPerTap
        );

        SetPrivateField(
            definition,
            "manualDecayDelay",
            decayDelay
        );

        SetPrivateField(
            definition,
            "manualDecayPerSecond",
            decayPerSecond
        );

        SetPrivateField(
            definition,
            "automaticDuration",
            automaticDuration
        );

        SetPrivateField(
            definition,
            "automaticIncomeInterval",
            automaticIncomeInterval
        );


        return definition;
    }


    private void SetPrivateField<T>(
        object target,
        string fieldName,
        T value)
    {
        FieldInfo field =
            target.GetType().GetField(
                fieldName,
                BindingFlags.Instance
                | BindingFlags.NonPublic
            );

        Assert.IsNotNull(
            field,
            $"Field '{fieldName}' was not found."
        );

        field.SetValue(
            target,
            value
        );
    }
}