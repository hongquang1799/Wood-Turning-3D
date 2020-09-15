using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColor : MonoBehaviour
{
    public Color[] colorCache;

    private void Start()
    {
        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.StartLevel, ChangeColor);
    }

    public void ChangeColor(object param)
    {
        int levelIndex = LevelManager.Instance.PlayerData.levelIndex;
        GetComponent<MeshRenderer>().material.SetColor("_Color", colorCache[(levelIndex - 1) % colorCache.Length]);
    }
}
