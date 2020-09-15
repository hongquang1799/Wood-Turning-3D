using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIEventDispatcher
{
    private static GUIEventDispatcher instance;

    public static GUIEventDispatcher Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GUIEventDispatcher();
            }

            return instance;
        }
    }

    public void RegisterEvent(GUIEventID eventId, Action<object> callback)
    {
        if (!allCallbacks.ContainsKey(eventId))
        {
            allCallbacks.Add(eventId, null);
        }

        allCallbacks[eventId] += callback;
    }

    public void RemoveEvent(GUIEventID eventId, Action<object> callback)
    {
        if (allCallbacks.ContainsKey(eventId))
        {
            allCallbacks[eventId] -= callback;

            if (allCallbacks[eventId] == null)
            {
                allCallbacks.Remove(eventId);
            }
        }
    }

    public void NotifyEvent(GUIEventID eventId, object param = null)
    {
        if (allCallbacks.ContainsKey(eventId))
        {
            var callback = allCallbacks[eventId];
            if (callback != null)
                callback(param);
        }
    }

    public void ClearEvent()
    {
        allCallbacks.Clear();
    }

    private Dictionary<GUIEventID, Action<object>> allCallbacks = new Dictionary<GUIEventID, Action<object>>();
}
