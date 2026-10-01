using System;

public interface ITimeService
{
    float DeltaTime { get; }

    DateTime UtcNow { get; }
}