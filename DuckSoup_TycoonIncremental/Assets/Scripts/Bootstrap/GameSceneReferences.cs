using UnityEngine;

public class GameSceneReferences : MonoBehaviour
{
    [Header("Floor 01")]
    [SerializeField] private FloorDefinitionSO floorDefinition;
    [SerializeField]private FloorView floorView;

    [Header("Economy")]
    [SerializeField]private EconomyView economyView;


    public FloorDefinitionSO FloorDefinition => floorDefinition;

    public FloorView FloorView => floorView;

    public EconomyView EconomyView => economyView;
}