using System;
using UnityEngine;

[Serializable]
public class UpgradeDefinition
{
    [Min(1)][SerializeField] private int level;

    [Min(0)][SerializeField] private double cost;

    [Min(1)][SerializeField] private double incomeMultiplier = 1;

    [Min(0)][SerializeField] private int visualStage;


    public int Level => level;

    public Money Cost => new Money((decimal)cost);

    public decimal IncomeMultiplier => (decimal)incomeMultiplier;

    public int VisualStage => visualStage;
}