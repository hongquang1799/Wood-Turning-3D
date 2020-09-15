using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public bool NeedToReload { get { return needToReload; } set { needToReload = value; } }

    public int maxLevelIndex = 10;

    private static LevelManager instance;

    private PlayerData playerData;

    private bool needToReload = true;
   
    public PlayerData PlayerData
    {
        get { return playerData; }
    }

    public static LevelManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType<LevelManager>();

                if (!instance)
                {
                    var go = new GameObject("LevelManager");
                    go.AddComponent<LevelManager>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance.GetInstanceID() != GetInstanceID())
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
    }

    public void Load(bool forceLoading = false)
    {
        if (needToReload == false && forceLoading == false) return;

        needToReload = false;
#if UNITY_EDITOR
        //PlayerData.FilePath = "F:/player_data.txt";
        PlayerData.FilePath = Application.persistentDataPath + "player_data.txt";
#else
        PlayerData.FilePath = Application.persistentDataPath + "player_data.txt";
#endif

        playerData = new PlayerData();
        playerData.Load();

        if (playerData.levelIndex > maxLevelIndex) playerData.levelIndex = 1;

        var patternShape = Resources.Load<PatternShape>("Patterns/" + playerData.levelIndex.ToString());

        PatternShape.Current = patternShape;       
    }
}
