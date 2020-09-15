using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorTable", menuName = "ScriptableObjects/ColorTable", order = 4)]
public class ColorTable : ScriptableObject
{
    public ColorInfo FindColorWithName(string name)
    {
        for (int i = 0; i < colorInfoList.Count; i++)
        {
            if (name == colorInfoList[i].name)
                return colorInfoList[i];
        }

        return null;
    }

    public ColorInfo GetColorInfo(int idx)
    {
        return colorInfoList[idx];
    }

    public int Count { get { return colorInfoList.Count; } }

    [SerializeField] private List<ColorInfo> colorInfoList;
}
