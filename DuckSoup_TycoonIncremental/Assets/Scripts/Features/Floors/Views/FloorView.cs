using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloorView : MonoBehaviour
{
    [Header("Identity")]

    [SerializeField]
    private TMP_Text floorNameText;


    [Header("UI References")]

    [SerializeField]
    private TMP_Text incomeText;

    [SerializeField]
    private TMP_Text automaticTimeText;

    [SerializeField]
    private Slider progressBar;

    [SerializeField]
    private Image progressFill;

    [SerializeField]
    private Button generateButton;

    [SerializeField]
    private Button upgradeButton;


    [Header("Unlock")]

    [SerializeField]
    private GameObject lockedPanel;

    [SerializeField]
    private TMP_Text unlockCostText;

    [SerializeField]
    private Button unlockButton;


    [Header("Upgrade Visuals")]

    [SerializeField]
    private GameObject[] upgradeVisuals;


    [Header("Progress Animation")]

    [Min(0.1f)]
    [SerializeField] private float progressAnimationSpeed = 1f;


    [Header("Automatic Visual")]

    [Min(0.1f)]
    [SerializeField] private float rainbowSpeed = 0.35f;


    public event Action GeneratePressed;
    public event Action UpgradePressed;
    public event Action UnlockPressed;


    private float targetProgress;

    private bool automaticVisualActive;

    private Color normalProgressColor;

    private float rainbowHue;


    private void Awake()
    {
        targetProgress = progressBar.value;

        if (progressFill != null)
        {
            normalProgressColor = progressFill.color;
        }

        if (automaticTimeText != null)
        {
            automaticTimeText.gameObject.SetActive(false);
        }

        generateButton.onClick.AddListener(OnGenerateButtonPressed);

        upgradeButton.onClick.AddListener(OnUpgradeButtonPressed);

        unlockButton.onClick.AddListener(OnUnlockButtonPressed);
    }


    private void Update()
    {
        AnimateProgressBar();
        AnimateAutomaticVisual();
    }


    private void OnDestroy()
    {
        generateButton.onClick.RemoveListener(OnGenerateButtonPressed);

        upgradeButton.onClick.RemoveListener(OnUpgradeButtonPressed);

        unlockButton.onClick.RemoveListener(OnUnlockButtonPressed);
    }


    private void AnimateProgressBar()
    {
        progressBar.value = Mathf.MoveTowards(progressBar.value,targetProgress,progressAnimationSpeed * Time.deltaTime);
    }


    private void AnimateAutomaticVisual()
    {
        if (!automaticVisualActive || progressFill == null)
        {
            return;
        }

        rainbowHue =Mathf.Repeat(rainbowHue + rainbowSpeed * Time.deltaTime,1f);

        progressFill.color = Color.HSVToRGB(rainbowHue,0.85f,1f);
    }


    private void OnGenerateButtonPressed()
    {
        GeneratePressed?.Invoke();
    }


    private void OnUpgradeButtonPressed()
    {
        UpgradePressed?.Invoke();
    }


    private void OnUnlockButtonPressed()
    {
        UnlockPressed?.Invoke();
    }


    public void SetFloorName(string value)
    {
        floorNameText.text = value;
    }


    public void SetIncome(string value)
    {
        incomeText.text = value;
    }


    public void SetProgress(float progress)
    {
        targetProgress = Mathf.Clamp01(progress);
    }


    public void SetGenerateButtonInteractable(bool interactable)
    {
        generateButton.interactable =interactable;
    }


    public void SetUpgradeButtonInteractable(bool interactable)
    {
        upgradeButton.interactable = interactable;
    }


    public void SetLockedState(bool isLocked, string unlockCost,bool canUnlock)
    {
        lockedPanel.SetActive(isLocked);

        unlockCostText.text = $"Unlock: {unlockCost}";

        unlockButton.interactable = canUnlock;
    }


    public void SetAutomaticVisual(bool active)
    {
        if (automaticVisualActive == active)
        {
            return;
        }

        automaticVisualActive = active;

        if (!active && progressFill != null)
        {
            progressFill.color = normalProgressColor;
        }
    }


    public void SetAutomaticTime(float remainingTime,bool visible)
    {
        if (automaticTimeText == null)
        {
            return;
        }

        automaticTimeText.gameObject.SetActive(visible);

        if (!visible)
        {
            return;
        }

        automaticTimeText.text = $"AUTO {remainingTime:0.0}s";
    }


    public void SetVisualStage(int visualStage)
    {
        if (upgradeVisuals == null)
        {
            return;
        }

        for (int i = 0; i < upgradeVisuals.Length;i++)
        {
            if (upgradeVisuals[i] == null)
            {
                continue;
            }

            bool shouldBeActive =i < visualStage;

            upgradeVisuals[i].SetActive(shouldBeActive);
        }
    }
}