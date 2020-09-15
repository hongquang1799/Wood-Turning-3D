using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDragChisel : MonoBehaviour
{
    public float dragRadius = 0.1f;

    public float maxDragSpeedNormal = 0.25f;

    public float maxDragSpeedContact = 0.08f;

    [HideInInspector] public float clampPositionY;

    protected bool touched = false;

    protected Camera mainCamera;

    protected float mainCameraZPos;

    protected Vector2 previousTouchPoint;

    protected float maxDragSpeed;

    public void OnChiselContactBegin()
    {
        maxDragSpeed = maxDragSpeedContact; 
    }

    public void OnChiselContactEnd()
    {
        maxDragSpeed = maxDragSpeedNormal;
    }

    void Start()
    {
        mainCamera = Camera.main;
        mainCameraZPos = mainCamera.transform.position.z;
    }

    void Update()
    {
        if (TouchUtility.Enabled && TouchUtility.TouchCount > 0)
        {
            Touch touch = TouchUtility.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (TouchUtility.TouchedUI(touch.fingerId))
                {
                    touched = false;
                    return;
                }    
                    
                Vector2 currentTouchPoint = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, -mainCameraZPos));
                if ((currentTouchPoint.ToVector3() - transform.position).sqrMagnitude < dragRadius * dragRadius)
                {
                    touched = true;
                    previousTouchPoint = currentTouchPoint;
                }
                else
                {
                    touched = false;
                }
            }

            if (touched == true && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended))
            {
                Vector2 currentTouchPoint = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, -mainCameraZPos));
                Vector2 delta = (currentTouchPoint - previousTouchPoint).LimitLength(maxDragSpeed);

                if (delta.sqrMagnitude > 0f)
                {
                    Vector3 chiselPosition = transform.localPosition + delta.ToVector3();

                    if (chiselPosition.y > clampPositionY)
                        chiselPosition.y = clampPositionY;

                    transform.localPosition = chiselPosition;

                    previousTouchPoint = currentTouchPoint;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    touched = false;
                }
            }
        }
    }
}
