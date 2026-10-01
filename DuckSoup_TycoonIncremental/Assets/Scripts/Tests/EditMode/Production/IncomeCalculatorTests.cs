using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class IncomeCalculatorTests
{
    [Test]
    public void Calculate_WithoutUpgrade_ReturnsBaseIncome()
    {
        FloorDefinitionSO definition =
            CreateDefinition();

        FloorRuntimeModel runtimeModel =
            new FloorRuntimeModel(
                1,
                true
            );

        IIncomeModifier[] modifiers =
        {
            new UpgradeIncomeModifier()
        };

        IncomeCalculator calculator =
            new IncomeCalculator(
                modifiers
            );

        Money result =
            calculator.Calculate(
                new Money(5),
                new IncomeContext(
                    definition,
                    runtimeModel
                )
            );

        Assert.AreEqual(
            5m,
            result.Value
        );
    }


    [Test]
    public void Calculate_WithUpgrade_AppliesUpgradeMultiplier()
    {
        FloorDefinitionSO definition =
            CreateDefinition();

        FloorRuntimeModel runtimeModel =
            new FloorRuntimeModel(
                1,
                true
            );

        runtimeModel.SetUpgradeLevel(1);

        IIncomeModifier[] modifiers =
        {
            new UpgradeIncomeModifier()
        };

        IncomeCalculator calculator =
            new IncomeCalculator(
                modifiers
            );

        Money result =
            calculator.Calculate(
                new Money(5),
                new IncomeContext(
                    definition,
                    runtimeModel
                )
            );

        Assert.AreEqual(
            7.5m,
            result.Value
        );
    }


    private FloorDefinitionSO CreateDefinition()
    {
        FloorDefinitionSO definition =
            ScriptableObject.CreateInstance<
                FloorDefinitionSO
            >();

        UpgradeDefinition upgrade =
            new UpgradeDefinition();

        SetPrivateField(
            upgrade,
            "level",
            1
        );

        SetPrivateField(
            upgrade,
            "incomeMultiplier",
            1.5d
        );

        UpgradeDefinition[] upgrades =
        {
            upgrade
        };

        SetPrivateField(
            definition,
            "upgrades",
            upgrades
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
