using System;

public interface IEventBus
{
    void Subscribe<T>(Action<T> listener);
    void Unsubscribe<T>(Action<T> listener);
    void Publish<T>(T eventData);
}