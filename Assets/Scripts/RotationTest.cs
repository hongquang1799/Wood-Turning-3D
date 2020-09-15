using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest : MonoBehaviour
{
    public float magnitude = 0.2f;

    public Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        

        Vector3 right = camera.transform.right;
        Vector3 forward = camera.transform.forward;
        Vector3 up = camera.transform.up;

        //Vector3 position = transform.position;
        //Debug.DrawLine(position - right * 10f, position + right * 10f);
        //Debug.DrawLine(position - forward * 10f, position + forward * 10f);
        //Debug.DrawLine(position - up * 10f, position + up * 10f);

        if (TouchUtility.Enabled && TouchUtility.TouchCount > 0)
        {
            Touch touch = TouchUtility.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                Vector2 deltaNormalized = delta.normalized;
                float deltaLength = delta.magnitude;

                Vector3 direction = right * deltaNormalized.x + up * deltaNormalized.y;


                Vector3 axis = Vector3.Cross(direction, forward);

                transform.Rotate(axis, deltaLength * magnitude, Space.World);
            }
        }

        
    }
}
