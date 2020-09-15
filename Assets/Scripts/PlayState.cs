using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayState 
{
    protected PlaySystem system;

    public PlayState(PlaySystem system)
    {
        this.system = system;
    }

    public abstract void Enter();

    public abstract void Exit();

    public abstract void Update();

    public abstract void Reset();
}

public class TurningState : PlayState
{
    public GameObject targetGo;

    private GameObject chiselGo;

    public GameObject targetProjGo;

    public GameObject patternProjGo_1;

    public GameObject patternProjGo_2;

    public Transform targetTransform;

    public Transform chiselSpawnTransform;

    public TurningState(PlaySystem system) : base(system)
    {        
    }

    public void ReplaceChisel(GameObject chiselPrefab)
    {
        if (chiselGo && chiselGo.name == chiselPrefab.name) return;

        Vector3 chiselOldPosition = chiselSpawnTransform.position;
        if (chiselGo)
        {
            chiselOldPosition = chiselGo.transform.position;
            Object.Destroy(chiselGo);
        }

        chiselGo = Object.Instantiate(chiselPrefab, chiselOldPosition, Quaternion.identity);
        chiselGo.GetComponent<TurningChisel>().Subject = targetGo.GetComponent<TurningController>();
    }

    public override void Enter()
    {
        targetProjGo.SetActive(true);
        patternProjGo_1.SetActive(true);
        patternProjGo_2.SetActive(true);

        patternProjGo_2.transform.position = targetProjGo.transform.position;
        patternProjGo_2.transform.localScale = targetProjGo.transform.localScale;

        Mesh patternMesh = PatternShape.BuildMeshFromShape(PatternShape.Current);
        patternProjGo_1.GetComponent<MeshFilter>().mesh = patternMesh;
        patternProjGo_2.GetComponent<MeshFilter>().mesh = patternMesh;

        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.EnterTurning);      
    }

    public override void Exit()
    {
        Object.Destroy(chiselGo);
        targetProjGo.SetActive(false);
        patternProjGo_1.SetActive(false);
        patternProjGo_2.SetActive(false);

        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.ExitTurning);        
    }

    public override void Update()
    {
        targetTransform.Rotate(new Vector3(1200f * Time.deltaTime, 0f, 0f));
    }

    public override void Reset()
    {
        chiselGo.GetComponent<TurningChisel>().ChangePosition(chiselSpawnTransform.position);
        targetGo.GetComponent<TurningController>().ResetState();
    }   
}

public class ColorPaintingState : PlayState
{
    public PaintingController target;

    public Painter painter;

    public Transform targetTransform;

    public ColorPaintingState(PlaySystem system) : base(system)
    {
    }

    public void ChangePainterColor(Color color)
    {
        painter.color = color;
    }

    public override void Enter()
    {
        painter.enabled = false;
        system.StartCoroutine(Schedule());        
    }

    public override void Exit()
    {
        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.ExitColorPainting);
    }

    public override void Update()
    {
        if (TouchUtility.Enabled && TouchUtility.TouchCount > 0)
        {
            Touch touch = TouchUtility.GetTouch(0);
            if (!TouchUtility.TouchedUI(touch.fingerId))
            {
                painter.Paint();
            }
        }

        targetTransform.Rotate(new Vector3(80f * Time.deltaTime, 0f, 0f));
    }

    public override void Reset()
    {
        target.ResetState();
    }

    public IEnumerator Schedule()
    {
        float animationDuration;
        var animatorProxy = target.GetComponent<AnimationControllerProxy>();

        animatorProxy.SetEnabled(true);
        animatorProxy.Play("TurningToPainting", out animationDuration);

        yield return new WaitForSeconds(animationDuration + 0.25f);

        animatorProxy.SetEnabled(false);

        painter.enabled = true;
        painter.subject = target;
        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.EnterColorPainting);        
    }
}

public class DecalPaintingState : PlayState
{
    public PaintingController target;

    public Painter painter;

    public Transform targetTransform;

    public RenderTexture colorTexture;

    public DecalPaintingState(PlaySystem system) : base(system)
    {
    }

    public void ChangeDecalTexture(Texture2D texture)
    {
        painter.brushTexture = texture;
    }

    public override void Enter()
    {
        painter.enabled = true;
        painter.color = Color.white;
        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.EnterDecalPainting);

        colorTexture = target.GetCopyTexture();
    }

    public override void Exit()
    {
        if (colorTexture.IsCreated())
            colorTexture.Release();

        painter.gameObject.SetActive(false);

        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.ExitDecalPainting);
    }

    public override void Update()
    {
        if (TouchUtility.Enabled && TouchUtility.TouchCount > 0)
        {
            Touch touch = TouchUtility.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !TouchUtility.TouchedUI(touch.fingerId))
            {
                painter.Paint();
            }
        }

        targetTransform.Rotate(new Vector3(80f * Time.deltaTime, 0f, 0f));
    }

    public override void Reset()
    {
        target.SetTexture(colorTexture);
        colorTexture = target.GetCopyTexture();
    }
}

public class EvaluatingState : PlayState
{
    public GameObject target;

    public GameObject patternProjectionGo;

    public bool PlayerPassed { get; set; }

    bool rotateTarget = false;

    public EvaluatingState(PlaySystem system) : base(system)
    {

    }
    public override void Enter()
    {
        system.StartCoroutine(Schedule());
    }

    public override void Exit()
    {
        
    }

    public override void Reset()
    {
        
    }

    public override void Update()
    {
        if (rotateTarget)
        {
            target.transform.Rotate(new Vector3(40f * Time.deltaTime, 0f, 0f));
        }
    }

    public IEnumerator Schedule()
    {
        float animationDuration;
        var animatorProxy = target.GetComponent<AnimationControllerProxy>();

        animatorProxy.SetEnabled(true);
        animatorProxy.Play("PaintingToEvaluating", out animationDuration);

        yield return new WaitForSeconds(animationDuration);

        animatorProxy.SetEnabled(false);

        float accuracy = AccuracyEvaluator.CalculateAccuracy();
        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.StartEvaluating, accuracy);

        patternProjectionGo.SetActive(true);

        Vector3 position = target.transform.localPosition;
        Quaternion rotation = target.transform.localRotation;
        Vector3 scale = target.transform.localScale;

        var xf = patternProjectionGo.transform;
        xf.localPosition = new Vector3(-position.x, position.y, position.z);
        xf.localRotation = rotation;
        xf.localScale = scale;

        yield return new WaitForSeconds(1.5f);

        rotateTarget = true;

        if (accuracy > 0.2f)
        {
            PlayerPassed = true;
            var playerData = LevelManager.Instance.PlayerData;
            playerData.levelIndex += 1;
            playerData.moneyScore += Mathf.RoundToInt(accuracy * 500f);
            playerData.Save();

            GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.UpdateMoney, playerData.moneyScore);
            
            GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.CanRestartOrGoNext);
        }            
        else
        {
            PlayerPassed = false;
            GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.LetTryAgain);
        }
            
    }
}
