using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerData 
{
    public static string FilePath
    {
        get { return filePath; }
        set { filePath = value; }
    }

    private static string filePath;

    public int levelIndex = 1;

    public int moneyScore = 0;

    public void Save()
    {
        string json = JsonUtility.ToJson(this);

        FileStream fileStream = File.Open(FilePath, FileMode.OpenOrCreate);
        StreamWriter sw = new StreamWriter(fileStream);
        sw.Write(json);
        sw.Close();
        fileStream.Close();
    }

    public void Load()
    {
        FileStream fileStream = File.Open(FilePath, FileMode.OpenOrCreate);
        StreamReader sr = new StreamReader(fileStream);
        string json = sr.ReadToEnd();
        sr.Close();
        fileStream.Close();

        PlayerData copy = JsonUtility.FromJson<PlayerData>(json);

        if (copy != null)
        {
            levelIndex = copy.levelIndex;
            moneyScore = copy.moneyScore;
        }      
    }
}
