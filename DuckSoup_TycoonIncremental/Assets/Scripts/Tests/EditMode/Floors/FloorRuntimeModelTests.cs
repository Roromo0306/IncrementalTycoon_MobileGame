using NUnit.Framework;

public class FloorRuntimeModelTests
{
    [Test]
    public void Constructor_CreatesExpectedInitialState()
    {
        FloorRuntimeModel floor =new FloorRuntimeModel(floorId: 1,isUnlocked: true);

        Assert.AreEqual(1, floor.FloorId);

        Assert.IsTrue(floor.IsUnlocked);

        Assert.AreEqual(0f, floor.Progress);

        Assert.AreEqual(0, floor.UpgradeLevel);

        Assert.AreEqual(0f,floor.AutomaticTimeRemaining);
    }


    [Test]
    public void SetUnlocked_ChangesUnlockedState()
    {
        FloorRuntimeModel floor =new FloorRuntimeModel(1);

        floor.SetUnlocked(true);

        Assert.IsTrue(floor.IsUnlocked);
    }


    [Test]
    public void SetProgress_StoresValidProgress()
    {
        FloorRuntimeModel floor =new FloorRuntimeModel(1);

        floor.SetProgress(0.5f);

        Assert.AreEqual(0.5f,floor.Progress);
    }


    [Test]
    public void SetProgress_AboveOne_ClampsToOne()
    {
        FloorRuntimeModel floor =new FloorRuntimeModel(1);

        floor.SetProgress(1.5f);

        Assert.AreEqual(1f,floor.Progress);
    }


    [Test]
    public void SetProgress_BelowZero_ClampsToZero()
    {
        FloorRuntimeModel floor =new FloorRuntimeModel(1);

        floor.SetProgress(-1f);

        Assert.AreEqual(0f,floor.Progress);
    }


    [Test]
    public void SetAutomaticTimeRemaining_BelowZero_ClampsToZero()
    {
        FloorRuntimeModel floor =new FloorRuntimeModel(1);

        floor.SetAutomaticTimeRemaining(-5f);

        Assert.AreEqual(0f,floor.AutomaticTimeRemaining);
    }
}