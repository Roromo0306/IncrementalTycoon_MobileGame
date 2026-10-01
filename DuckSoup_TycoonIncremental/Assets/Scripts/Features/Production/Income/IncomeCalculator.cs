using System;
using System.Collections.Generic;

public class IncomeCalculator : IIncomeCalculator
{
    private readonly IReadOnlyList<IIncomeModifier>modifiers;


    public IncomeCalculator(IReadOnlyList<IIncomeModifier> modifiers)
    {
        this.modifiers = modifiers ?? throw new ArgumentNullException(nameof(modifiers));
    }


    public Money Calculate(Money baseIncome,IncomeContext context)
    {
        Money finalIncome = baseIncome;

        for (int i = 0; i < modifiers.Count; i++)
        {
            finalIncome = modifiers[i].Apply(finalIncome, context);
        }

        return finalIncome;
    }
}