using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public GameObject turningToolsGroup;

    public GameObject colorPaintingToolsGroup;

    public GameObject decalPaintingToolsGroup;

    public GameObject nextStateButton;

    public GameObject resetStateButton;

    public GameObject resultText;

    public EvaluatingPanel evaluatingPanel;

    public GameObject tryAgain;

    public Text moneyText;

    private void Awake()
    {
        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.StartLevel, OnStartLevel);

        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.EnterTurning, new Action<object>((object param) => SetActiveTarget(turningToolsGroup, true)));
        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.EnterColorPainting, new Action<object>((object param) => SetActiveTarget(colorPaintingToolsGroup, true)));
        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.EnterDecalPainting, new Action<object>((object param) => SetActiveTarget(decalPaintingToolsGroup, true)));

        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.ExitTurning, new Action<object>((object param) => SetActiveTarget(turningToolsGroup, false)));
        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.ExitColorPainting, new Action<object>((object param) => SetActiveTarget(colorPaintingToolsGroup, false)));
        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.ExitDecalPainting, new Action<object>((object param) => SetActiveTarget(decalPaintingToolsGroup, false)));

        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.EnterTurning, new Action<object>((object param) => EnableStateButtons(true)));
        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.EnterColorPainting, new Action<object>((object param) => EnableStateButtons(true)));
        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.EnterDecalPainting, new Action<object>((object param) => EnableStateButtons(true)));

        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.ExitTurning, new Action<object>((object param) => EnableStateButtons(false)));
        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.ExitColorPainting, new Action<object>((object param) => EnableStateButtons(false)));
        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.ExitDecalPainting, new Action<object>((object param) => EnableStateButtons(false)));

        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.StartEvaluating, new Action<object>((object param) => SetActiveTarget(resultText, true)));
        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.StartEvaluating, new Action<object>((object param) =>
        {
            SetActiveTarget(evaluatingPanel.gameObject, true);
            evaluatingPanel.StartEvaluating((float)param);
        }));
        

        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.UpdateMoney, new Action<object>((object param) => SetText(moneyText, "$" + param.ToString())));

        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.CanRestartOrGoNext, new Action<object>((object param) => EnableStateButtons(true)));
        GUIEventDispatcher.Instance.RegisterEvent(GUIEventID.LetTryAgain, new Action<object>((object param) => SetActiveTarget(tryAgain, true)));
    }

    private void OnDestroy()
    {
        GUIEventDispatcher.Instance.ClearEvent();
    }

    private void OnStartLevel(object param)
    {
        turningToolsGroup.SetActive(false);
        colorPaintingToolsGroup.SetActive(false);
        decalPaintingToolsGroup.SetActive(false);
        nextStateButton.SetActive(true);
        resetStateButton.SetActive(true);
        resultText.SetActive(false);
        evaluatingPanel.gameObject.SetActive(false);
        tryAgain.SetActive(false);
    }

    private void EnableStateButtons(bool flag)
    {
        nextStateButton.GetComponent<Button>().interactable = flag;
        resetStateButton.GetComponent<Button>().interactable = flag;
    }

    private void SetActiveTarget(GameObject target, bool flag)
    {
        target.SetActive(flag);
    }

    private void SetText(Text text, string str)
    {
        text.text = str;
    }
}
