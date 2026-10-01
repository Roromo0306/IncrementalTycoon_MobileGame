using System;

public class FakeTimeService : ITimeService
{
    public float DeltaTime { get; private set; }

    public DateTime UtcNow { get; private set; }


    public FakeTimeService(DateTime initialUtcNow)
    {
        UtcNow = initialUtcNow;
        DeltaTime = 0f;
    }


    public void Advance(float seconds)
    {
        if (seconds < 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds), "Time cannot advance by a negative amount.");
        }

        DeltaTime = seconds;

        UtcNow = UtcNow.AddSeconds(seconds);
    }


    public void SetDeltaTime(float deltaTime)
    {
        if (deltaTime < 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTime), "Delta time cannot be negative.");
        }

        DeltaTime = deltaTime;
    }
}