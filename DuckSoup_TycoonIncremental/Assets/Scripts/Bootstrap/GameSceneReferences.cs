using UnityEngine;

public class GameSceneReferences : MonoBehaviour
{
    [Header("Floor 01")]

    [SerializeField]private FloorDefinitionSO floorDefinition;

    [SerializeField]private FloorView floorView;


    public FloorDefinitionSO FloorDefinition => floorDefinition;

    public FloorView FloorView => floorView;
}