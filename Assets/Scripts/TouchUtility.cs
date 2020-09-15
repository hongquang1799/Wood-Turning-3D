using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class TouchUtility 
{
    public static bool Enabled { get { return enabled; } set { enabled = value; } }

    public static bool TouchedUI(int fingerId)
    {
        return EventSystem.current.IsPointerOverGameObject(fingerId);
    }

    private static bool enabled = true;

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
               
#else
    private static Vector2 previousPosition;        
#endif
    public static int TouchCount
    {
        get 
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return Input.touchCount;
#else
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0)/* || Input.GetMouseButtonUp(0)*/ )
                return 1;
            else
                return 0;
#endif
        }
    }

    public static Touch GetTouch(int index)
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return Input.GetTouch(index);
#else
        Touch touch = new Touch();
        
        if (index == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 position = Input.mousePosition;
                touch.position = position;
                touch.phase = TouchPhase.Began;
                touch.fingerId = -1;

                touch.deltaPosition = Vector2.zero;

                previousPosition = position;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 position = Input.mousePosition;
                touch.position = position;
                touch.phase = TouchPhase.Moved;
                touch.fingerId = -1;
                touch.deltaPosition = position - previousPosition;

                previousPosition = position;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector2 position = Input.mousePosition;
                touch.position = position;
                touch.phase = TouchPhase.Ended;
                touch.fingerId = -1;
                touch.deltaPosition = position - previousPosition;

                previousPosition = position;
            }
        }
        
        return touch;
#endif
    }
}
