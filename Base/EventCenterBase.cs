using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EventCenterBase<T> : SingleMonoBase<EventCenterBase<T>>
{
    private Dictionary<string, UnityAction<T>> events = new Dictionary<string, UnityAction<T>>();

    public void AddEvent(string eventName, UnityAction<T> action)
    {
        if (!events.ContainsKey(eventName))
        {
            events.Add(eventName, action);
        }
        else
        {
            events[eventName] += action;
        }
    }

    public void RemoveEvent(string eventName, UnityAction<T> action)
    {
        if (!events.ContainsKey(eventName) || events[eventName] == null)
        {
            return;
        }
        else
        {
            events[eventName] -= action;
        }
    }

    public void TriggerEvent(string eventName, T args)
    {
        if (!events.ContainsKey(eventName) || events[eventName] == null)
        {
            return;
        }
        events[eventName].Invoke(args);
    }

}
