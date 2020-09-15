using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecalPaintButtonGroup : MonoBehaviour
{
    private struct DecalButtonData
    {
        public Image layoutImage;
        public Texture2D texture;
    }

    public DecalTable decalTable;

    private List<DecalButtonData> decalButtonDatas = new List<DecalButtonData>();

    private void Awake()
    {
        int count = Mathf.Min(transform.childCount, decalTable.Count);

        for (int i = 0; i < count; i++)
        {
            var child = transform.GetChild(i);

            Image layoutImage = child.GetComponent<Image>();
            Image colorImage = child.GetChild(0).GetComponent<Image>();
            Texture2D texture = decalTable.GetDecalInfo(i).texture;

            colorImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            int idx = i;
            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectButton(idx);
            });

            decalButtonDatas.Add(new DecalButtonData { layoutImage = layoutImage, texture = texture });
        }
    }

    private void OnEnable()
    {
        SelectButton(0);
    }

    void SelectButton(int idx)
    {
        for (int i = 0; i < decalButtonDatas.Count; i++)
        {
            decalButtonDatas[i].layoutImage.color = new Color(0.35f, 0.35f, 0.35f, 0.6f);
        }

        var decalButtonData = decalButtonDatas[idx];
        decalButtonData.layoutImage.color = Color.white;

        SetDecal(decalButtonData.texture);
    }

    private void SetDecal(Texture2D texture)
    {
        var state = PlaySystem.Instance.GetState() as DecalPaintingState;
        if (state != null)
        {
            state.ChangeDecalTexture(texture);
        }
    }
}
