using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    private static EventManager instance;

    private Dictionary<string, Delegate> eventDictionary;

    public static EventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EventManager();
            }
            return instance;
        }
    }

    private EventManager()
    {
        eventDictionary = new Dictionary<string, Delegate>();
    }

    public void RegisterEvent(string eventName, Action listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Delegate thisEvent))
        {
            thisEvent = Delegate.Combine(thisEvent, listener);
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            eventDictionary[eventName] = listener;
        }
        Debug.Log($"Event '{eventName}' registered");

    }


    public void RegisterEvent<T>(string eventName, Action<T> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Delegate thisEvent))
        {
            thisEvent = Delegate.Combine(thisEvent, listener);
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            eventDictionary[eventName] = listener;
        }

        Debug.Log($"Event '{eventName}' registered");

    }


    public void UnregisterEvent(string eventName, Action listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Delegate thisEvent))
        {
            thisEvent = Delegate.Remove(thisEvent, listener);
            if (thisEvent == null)
            {
                eventDictionary.Remove(eventName);
            }
            else
            {
                eventDictionary[eventName] = thisEvent;
            }
        }
    }


    public void UnregisterEvent<T>(string eventName, Action<T> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out Delegate thisEvent))
        {
            thisEvent = Delegate.Remove(thisEvent, listener);
            if (thisEvent == null)
            {
                eventDictionary.Remove(eventName);
            }
            else
            {
                eventDictionary[eventName] = thisEvent;
            }
        }
    }


    public void TriggerEvent(string eventName)
    {
        if (eventDictionary.TryGetValue(eventName, out Delegate thisEvent))
        {
            (thisEvent as Action)?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Event '{eventName}' not found!");
        }
    }


    public void TriggerEvent<T>(string eventName, T parameter)
    {
        if (eventDictionary.TryGetValue(eventName, out Delegate thisEvent))
        {
            (thisEvent as Action<T>)?.Invoke(parameter);
        }
        else
        {
            Debug.LogWarning($"Event '{eventName}' not found!");
        }
    }

    public void UnregisterAllEvents()
    {
        eventDictionary.Clear();
    }
}
