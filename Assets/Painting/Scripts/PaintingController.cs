using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingController : MonoBehaviour
{
    public RenderTexture mainTexture;

    public int textureSize = 1024;

    private Mesh mesh;

    public RenderTexture targetTexture1;
    public RenderTexture targetTexture2;

    private void Awake()
    {
        mainTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);
        mainTexture.wrapMode = TextureWrapMode.Repeat;
        mainTexture.Create();

        targetTexture1 = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);
        targetTexture1.wrapMode = TextureWrapMode.Repeat;
        targetTexture1.Create();

        targetTexture2 = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);
        targetTexture2.wrapMode = TextureWrapMode.Repeat;
        targetTexture2.Create();

        ResetState();
    }

    private void OnDestroy()
    {
        if (mainTexture.IsCreated())
            mainTexture.Release();

        if (targetTexture1.IsCreated())
            targetTexture1.Release();

        if (targetTexture2.IsCreated())
            targetTexture2.Release();
    }

    void Start()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;

        GetComponent<MeshRenderer>().material.SetTexture("_PaintTex", mainTexture);
    }

    public void SetTexture(RenderTexture texture)
    {
        if (texture.IsCreated())
        {
            if (mainTexture.IsCreated())
                mainTexture.Release();

            mainTexture = texture;
            GetComponent<MeshRenderer>().material.SetTexture("_PaintTex", mainTexture);
        }     
    }

    public RenderTexture GetCopyTexture()
    {
        var copyTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGB32);
        copyTexture.wrapMode = TextureWrapMode.Repeat;
        copyTexture.Create();

        Graphics.CopyTexture(mainTexture, copyTexture);

        return copyTexture;
    }

    public void ResetState()
    {
        var currentRenderTexture = RenderTexture.active;
        RenderTexture.active = targetTexture1;

        GL.Clear(true, true, new Color(1f, 1f, 1f, 0f));

        RenderTexture.active = currentRenderTexture;

        Graphics.CopyTexture(targetTexture1, mainTexture);
    }

    public void RenderUVSpace(Material uvSpaceMaterial)
    {
        var currentRenderTexture = RenderTexture.active;
        RenderTexture.active = targetTexture1;

        //GL.Clear(true, true, Color.blue);           
        Graphics.DrawMeshNow(mesh, Matrix4x4.identity);

        RenderTexture.active = currentRenderTexture;

        Graphics.CopyTexture(targetTexture1, mainTexture);

        //var currentRenderTexture = RenderTexture.active;
        //RenderTexture.active = targetTexture1;

        ////GL.Clear(true, true, Color.blue);           
        //Graphics.DrawMeshNow(mesh, Matrix4x4.identity);

        //RenderTexture.active = currentRenderTexture;

        //Graphics.CopyTexture(targetTexture1, targetTexture2);

        //Graphics.Blit(targetTexture2, mainTexture, fixUVIslandMaterial);
    }
}
