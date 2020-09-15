using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPaintButtonGroup : MonoBehaviour
{
    private struct ColorButtonData
    {
        public Image layoutImage;
        public Color color;
    }

    public ColorTable colorTable;

    private List<ColorButtonData> colorButtonDatas = new List<ColorButtonData>();

    private void Awake()
    {
        int count = Mathf.Min(transform.childCount, colorTable.Count);

        for (int i = 0; i < count; i++)
        {
            var child = transform.GetChild(i);

            Image layoutImage = child.GetComponent<Image>();
            Image colorImage = child.GetChild(0).GetComponent<Image>();
            Color color = colorTable.GetColorInfo(i).color;

            colorImage.color = color;

            int idx = i;
            child.GetComponent<Button>().onClick.AddListener(() =>
            {               
                SelectButton(idx);
            });

            colorButtonDatas.Add(new ColorButtonData { layoutImage = layoutImage, color = color});
        }     
    }

    private void OnEnable()
    {
        SelectButton(0);
    }

    void SelectButton(int idx)
    {
        for (int i = 0; i < colorButtonDatas.Count; i++)
        {
            colorButtonDatas[i].layoutImage.color = new Color(0.35f, 0.35f, 0.35f, 0.6f);
        }

        var colorButtonData = colorButtonDatas[idx];
        colorButtonData.layoutImage.color = Color.white;

        SetColor(colorButtonData.color);
    }

    private void SetColor(Color color)
    {
        var state = PlaySystem.Instance.GetState() as ColorPaintingState;
        if (state != null)
        {
            state.ChangePainterColor(color);
        }
    }
}
