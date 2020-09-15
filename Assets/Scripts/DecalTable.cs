using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecalTable", menuName = "ScriptableObjects/DecalTable", order = 4)]
public class DecalTable : ScriptableObject
{
    public DecalInfo FindDecalWithName(string name)
    {
        for (int i = 0; i < decalInfoList.Count; i++)
        {
            if (name == decalInfoList[i].name)
                return decalInfoList[i];
        }

        return null;
    }

    public DecalInfo GetDecalInfo(int idx)
    {
        return decalInfoList[idx];
    }

    public int Count { get { return decalInfoList.Count; } }

    [SerializeField] private List<DecalInfo> decalInfoList;
}
