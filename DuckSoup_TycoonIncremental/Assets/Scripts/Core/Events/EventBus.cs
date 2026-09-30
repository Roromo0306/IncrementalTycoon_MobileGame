using System;
using System.Collections.Generic;

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, Delegate> listeners = new();

    public void Subscribe<T>(Action<T> listener)
    {
        Type eventType = typeof(T);

        if (listeners.ContainsKey(eventType))
        {
            listeners[eventType] = Delegate.Combine(
                listeners[eventType],
                listener
            );
        }
        else
        {
            listeners[eventType] = listener;
        }
    }

    public void Unsubscribe<T>(Action<T> listener)
    {
        Type eventType = typeof(T);

        if (!listeners.ContainsKey(eventType))
        {
            return;
        }

        Delegate currentListeners = Delegate.Remove(listeners[eventType],listener);

        if (currentListeners == null)
        {
            listeners.Remove(eventType);
        }
        else
        {
            listeners[eventType] = currentListeners;
        }
    }

    public void Publish<T>(T eventData)
    {
        Type eventType = typeof(T);

        if (!listeners.TryGetValue(eventType, out Delegate eventListeners))
        {
            return;
        }

        ((Action<T>)eventListeners)?.Invoke(eventData);
    }
}