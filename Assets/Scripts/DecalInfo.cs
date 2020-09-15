using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecalInfo", menuName = "ScriptableObjects/DecalInfo", order = 3)]
public class DecalInfo : ScriptableObject
{
    public string name;
    public Texture2D texture;
}
