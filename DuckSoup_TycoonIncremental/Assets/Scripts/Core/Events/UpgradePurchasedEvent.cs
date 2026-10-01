public readonly struct UpgradePurchasedEvent
{
    public readonly int FloorId;
    public readonly int UpgradeLevel;
    public readonly int VisualStage;

    public UpgradePurchasedEvent(int floorId, int upgradeLevel, int visualStage)
    {
        FloorId = floorId;
        UpgradeLevel = upgradeLevel;
        VisualStage = visualStage;
    }
}