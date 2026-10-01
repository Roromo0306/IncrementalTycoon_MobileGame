using System;
using NUnit.Framework;

public class TimeServiceTests
{
    [Test]
    public void Advance_UpdatesDeltaTime()
    {
        FakeTimeService timeService =
            new FakeTimeService(
                new DateTime(
                    2026,
                    1,
                    1,
                    12,
                    0,
                    0,
                    DateTimeKind.Utc
                )
            );

        timeService.Advance(5f);

        Assert.AreEqual(5f, timeService.DeltaTime);
    }


    [Test]
    public void Advance_UpdatesUtcNow()
    {
        DateTime initialTime =
            new DateTime(
                2026,
                1,
                1,
                12,
                0,
                0,
                DateTimeKind.Utc
            );

        FakeTimeService timeService = new FakeTimeService(initialTime);

        timeService.Advance(10f);

        Assert.AreEqual(initialTime.AddSeconds(10),timeService.UtcNow);
    }


    [Test]
    public void Advance_WithNegativeTime_ThrowsException()
    {
        FakeTimeService timeService = new FakeTimeService(DateTime.UtcNow);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                timeService.Advance(-1f);
            }
        );
    }
}