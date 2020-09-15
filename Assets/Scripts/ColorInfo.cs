using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorInfo", menuName = "ScriptableObjects/ColorInfo", order = 3)]
public class ColorInfo : ScriptableObject
{
    public string name;
    public Color color = Color.white;
}