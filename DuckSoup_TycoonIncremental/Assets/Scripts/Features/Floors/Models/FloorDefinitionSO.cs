using UnityEngine;

[CreateAssetMenu(fileName = "FloorDefinition", menuName = "Duck Soup/Floors/Floor Definition")]
public class FloorDefinitionSO : ScriptableObject
{
    [Header("Identity")]

    [SerializeField]
    private int id;

    [SerializeField]
    private string floorName;


    [Header("Economy")]

    [Min(0)]
    [SerializeField]
    private double unlockCost;

    [Min(0)]
    [SerializeField]
    private double baseIncome;


    [Header("Production")]

    [Range(0.01f, 1f)]
    [SerializeField]
    private float progressPerTap = 0.1f;

    [Min(0.1f)]
    [SerializeField]
    private float automaticDuration = 10f;


    public int Id => id;

    public string FloorName => floorName;

    public Money UnlockCost => new Money((decimal)unlockCost);

    public Money BaseIncome => new Money((decimal)baseIncome);

    public float ProgressPerTap => progressPerTap;

    public float AutomaticDuration => automaticDuration;
}