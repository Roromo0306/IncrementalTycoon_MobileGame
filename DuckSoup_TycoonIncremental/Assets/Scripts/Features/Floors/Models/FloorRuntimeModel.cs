using System;

public class FloorRuntimeModel
{
    public int FloorId { get; }

    public bool IsUnlocked { get; private set; }

    public float Progress { get; private set; }

    public int UpgradeLevel { get; private set; }

    public float AutomaticTimeRemaining { get; private set; }


    public FloorRuntimeModel(int floorId, bool isUnlocked = false)
    {
        if (floorId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(floorId),"Floor ID must be greater than zero.");
        }

        FloorId = floorId;
        IsUnlocked = isUnlocked;

        Progress = 0f;
        UpgradeLevel = 0;
        AutomaticTimeRemaining = 0f;
    }


    public void SetUnlocked(bool isUnlocked)
    {
        IsUnlocked = isUnlocked;
    }


    public void SetProgress(float progress)
    {
        if (progress < 0f)
        {
            Progress = 0f;
            return;
        }

        if (progress > 1f)
        {
            Progress = 1f;
            return;
        }

        Progress = progress;
    }


    public void SetUpgradeLevel(int upgradeLevel)
    {
        if (upgradeLevel < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(upgradeLevel),"Upgrade level cannot be negative.");
        }

        UpgradeLevel = upgradeLevel;
    }


    public void SetAutomaticTimeRemaining(float time)
    {
        AutomaticTimeRemaining =
            time < 0f ? 0f : time;
    }


    public void ResetProgress()
    {
        Progress = 0f;
    }
}