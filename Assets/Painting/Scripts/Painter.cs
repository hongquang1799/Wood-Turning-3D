using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour
{
    public Material uvSpaceMaterial;

    public float brushSize = 50;

    public Color color;

    public PaintingController subject;

    public Texture2D brushTexture;

    private Camera mainCamera;

    private void Awake()
    { 
        mainCamera = Camera.main;
    }

    public void Paint()
    {
        Vector4 uvRect = new Vector4();
        uvRect.x = (Input.mousePosition.x - brushSize / 2f) / Screen.width;
        uvRect.y = (Input.mousePosition.y - brushSize / 2f) / Screen.height;
        uvRect.z = brushSize / Screen.width;
        uvRect.w = brushSize / Screen.height;

        Vector4 uvClamp = new Vector4();
        uvClamp.x = Mathf.Max(0f, uvRect.x);
        uvClamp.y = Mathf.Max(0f, uvRect.y);
        uvClamp.z = Mathf.Min(1f, uvRect.x + uvRect.z);
        uvClamp.w = Mathf.Min(1f, uvRect.y + uvRect.w);

        uvSpaceMaterial.SetTexture("_BrushTex", brushTexture);
  
        uvSpaceMaterial.SetMatrix("_PMatrix", mainCamera.projectionMatrix);
        uvSpaceMaterial.SetVector("_BrushUVRect", uvRect);
        uvSpaceMaterial.SetVector("_BrushUVClamp", uvClamp);
        uvSpaceMaterial.SetColor("_BrushColor", color);

        uvSpaceMaterial.SetTexture("_MainTex", subject.mainTexture);
        uvSpaceMaterial.SetMatrix("_MVMatrix", mainCamera.worldToCameraMatrix * subject.transform.localToWorldMatrix);
        uvSpaceMaterial.SetPass(0);
        subject.RenderUVSpace(uvSpaceMaterial);
    }
}
