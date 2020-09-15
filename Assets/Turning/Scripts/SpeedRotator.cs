using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpeedRotator : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin " + eventData.pressPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Move " + eventData.pressPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End " + eventData.pressPosition);
    }
}
