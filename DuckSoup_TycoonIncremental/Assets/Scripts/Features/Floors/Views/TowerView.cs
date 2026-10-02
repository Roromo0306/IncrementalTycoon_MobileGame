using System;
using UnityEngine;
using UnityEngine.UI;

public class TowerView : MonoBehaviour
{
    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    private RectTransform content;

    [SerializeField]
    private FloorView floorPrefab;


    public FloorView CreateFloorView(
        FloorDefinitionSO definition)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(
                nameof(definition)
            );
        }

        if (floorPrefab == null)
        {
            throw new InvalidOperationException(
                "Floor Prefab is not assigned in TowerView."
            );
        }

        if (content == null)
        {
            throw new InvalidOperationException(
                "Content is not assigned in TowerView."
            );
        }

        FloorView view =
            Instantiate(
                floorPrefab,
                content
            );

        view.name =
            $"Floor_{definition.Id:00}";

        
        view.transform.SetAsFirstSibling();

        return view;
    }


    public void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();

        LayoutRebuilder
            .ForceRebuildLayoutImmediate(
                content
            );

        scrollRect.verticalNormalizedPosition =
            0f;
    }
}