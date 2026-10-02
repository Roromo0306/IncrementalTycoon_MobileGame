using UnityEngine;

public class GameSceneReferences : MonoBehaviour
{
    [Header("Tower")]

    [SerializeField]
    private TowerView towerView;

    [SerializeField]
    private FloorDefinitionSO[] floorDefinitions;


    [Header("Economy")]

    [SerializeField]
    private EconomyView economyView;


    public TowerView TowerView => towerView;

    public FloorDefinitionSO[] FloorDefinitions => floorDefinitions;

    public EconomyView EconomyView => economyView;
}