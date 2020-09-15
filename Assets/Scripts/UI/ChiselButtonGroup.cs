using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class ChiselButtonGroup : MonoBehaviour
{
    public ChiselTable chiselTabel;

    private struct ChiselButtonData
    {
        public Image layoutImage;
        public GameObject chiselPrefab;
    }

    private List<ChiselButtonData> chiselButtonDatas = new List<ChiselButtonData>();

    private void Awake()
    {
        int count = Mathf.Min(transform.childCount, chiselTabel.Count);

        for (int i = 0; i < count; i++)
        {
            var child = transform.GetChild(i);

            Image layoutImage = child.GetComponent<Image>();
            Image image = child.GetChild(0).GetComponent<Image>();

            ChiselInfo chiselInfo = chiselTabel.GetChiselInfoAtIndex(i);
            GameObject chiselPrefab = chiselInfo.prefab;
            Texture2D buttonTexture = chiselInfo.buttonTexture; 

            image.sprite = Sprite.Create(buttonTexture, new Rect(0, 0, buttonTexture.width, buttonTexture.height), new Vector2(0.5f, 0.5f));

            int idx = i;
            child.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectButton(idx);
            });

            chiselButtonDatas.Add(new ChiselButtonData { layoutImage = layoutImage, chiselPrefab = chiselPrefab });
        }
    }

    private void OnEnable()
    {
        SelectButton(0);
    }

    void SelectButton(int idx)
    {
        for (int i = 0; i < chiselButtonDatas.Count; i++)
        {
            chiselButtonDatas[i].layoutImage.color = new Color(0.35f, 0.35f, 0.35f, 0.6f);
        }

        var chiselButtonData = chiselButtonDatas[idx];
        chiselButtonData.layoutImage.color = Color.white;

        SetChisel(chiselButtonData.chiselPrefab);
    }

    private void SetChisel(GameObject chiselPrefab)
    {
        var state = PlaySystem.Instance.GetState() as TurningState;
        if (state != null)
        {
            state.ReplaceChisel(chiselPrefab);
        }
    }
}
