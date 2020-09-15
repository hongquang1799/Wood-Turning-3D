using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFitCamera : MonoBehaviour
{
    public float distanceToCamera = 40f;

    void Awake()
    {
        Camera camera = Camera.main;

        Vector3 position = transform.position;
        var cameraTransform = camera.transform;

        float frustumHeight = 2.0f * distanceToCamera * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frustumHeight * camera.aspect;

        transform.position = cameraTransform.localPosition + camera.transform.forward * distanceToCamera;
        transform.localScale = new Vector3(frustumWidth, frustumHeight, 1f);
    }
}
