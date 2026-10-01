using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloorView : MonoBehaviour
{
    [Header("UI References")]

    [SerializeField] private TMP_Text incomeText;

    [SerializeField]private Slider progressBar;

    [SerializeField]private Button generateButton;

    [SerializeField]private Button upgradeButton;


    public event Action GeneratePressed;
    public event Action UpgradePressed;


    private void Awake()
    {
        generateButton.onClick.AddListener(OnGenerateButtonPressed);

        upgradeButton.onClick.AddListener(OnUpgradeButtonPressed);
    }


    private void OnDestroy()
    {
        generateButton.onClick.RemoveListener(OnGenerateButtonPressed);

        upgradeButton.onClick.RemoveListener(OnUpgradeButtonPressed);
    }


    private void OnGenerateButtonPressed()
    {
        GeneratePressed?.Invoke();
    }


    private void OnUpgradeButtonPressed()
    {
        UpgradePressed?.Invoke();
    }


    public void SetIncome(string value)
    {
        incomeText.text = value;
    }


    public void SetProgress(float progress)
    {
        progressBar.value = progress;
    }


    public void SetGenerateButtonInteractable(bool interactable)
    {
        generateButton.interactable = interactable;
    }


    public void SetUpgradeButtonInteractable(bool interactable)
    {
        upgradeButton.interactable = interactable;
    }
}