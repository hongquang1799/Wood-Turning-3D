using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextShowGraphicPlatform : MonoBehaviour
{
    void Start()
    {
        GetComponent<Text>().text = SystemInfo.graphicsDeviceType.ToString();
    }  
}
