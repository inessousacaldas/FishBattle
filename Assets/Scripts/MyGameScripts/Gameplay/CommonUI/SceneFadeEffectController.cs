using System;
using System.Collections;
using AppDto;
using UnityEngine;
using Random = UnityEngine.Random;

public class SceneFadeEffectController : MonoViewController<SceneFadeEffectPrefab>
{
    public const string NAME = "SceneFadeEffectPrefab";
    const float DefaultLoadTime = 2f;
    const float bufferSlider = 0.05f;
    private static SceneFadeEffectController _instance;
    static Vector3 mainPlayerPos;

    private int _callbackVal;
    private bool _finishLoadMap;
    private bool _isBattleScene;

//    private ModelDisplayController _modelController;
    private Action _onFinishFadeOut;
    private Action<int> _onFinishLoadMap;
    private int _sceneResId;
    
    private float curLoadTime;
    private float startLoadTime;
    public static void Show(SceneDto sceneDto, Action<int> onFinishMap, Action onFinishFadeOut)
    {
        foreach (SceneObjectDto objDto in sceneDto.objects)
        {
            if (ModelManager.Player.GetPlayerId() == objDto.id)
            {
                mainPlayerPos = new Vector3(objDto.x, 0, objDto.z);
            }
        }
        Show(sceneDto.sceneMap.resId, false, sceneDto.id, false, onFinishMap, onFinishFadeOut);
    }

    public static void Show(Video battleVideo, Action<int> onFinishMap, Action onFinishFadeOut)
    {
        Show(battleVideo.mapId, false, battleVideo.mapId, true, onFinishMap, onFinishFadeOut);
    }

    public static void Show(int sceneResId,bool is2dMap, int callbackVal, bool isBattleScene, Action<int> onFinishMap,
        Action onFinishFadeOut)
    {
        var module = UIModuleManager.Instance.OpenFunModule(NAME, UILayerType.SceneChange, false);
        if (_instance == null)
        {
            _instance = module.GetMissingComponent<SceneFadeEffectController>();
        }

        _instance.ReSetViewStyle(is2dMap);
        _instance.FadeIn(sceneResId, is2dMap, callbackVal, isBattleScene, onFinishMap, onFinishFadeOut);
    }

    protected override void AfterInitView ()
    {

        GameDebuger.TODO(@"
        if (_modelController == null)
        {
            _modelController = ModelDisplayController.GenerateUICom(View.loadingSliderThumb);
            _modelController.Init(200, 200, new Vector3(-16.5f, -33.9f, 8.5f), 1f, ModelHelper.Anim_run, false);
            _modelController.SetOrthographic(1.2f);
            _modelController.SetupModel(2000);
            _modelController.transform.localPosition = new Vector3(-20f, 56f, 0f);
        }
");
    }

    private void ReSetViewStyle(bool is2dMap) {
        View.bgTexture.gameObject.SetActive (!is2dMap);
        GameDebuger.TODO(@"View.tipBg_Transform.gameObject.SetActive (!is2dMap);");
        View.loadingSliderThumb.gameObject.SetActive (!is2dMap);
    }

    private void FadeIn(int sceneResId, bool is2dMap, int callbakVal, bool isBattleScene, Action<int> onFinishMap,
        Action onFinishFadeOut)
    {
        _sceneResId = sceneResId;
        
        _callbackVal = callbakVal;
        _isBattleScene = isBattleScene;
        _onFinishLoadMap = onFinishMap;
        _onFinishFadeOut = onFinishFadeOut;

        View.tipLbl.text = LoadingTipManager.GetLoadingTip();
        if (!isBattleScene)
            View.bgTexture.mainTexture = Resources.Load<Texture>("Textures/LoadingBG/loadingBG");
        if (is2dMap)
        {
            View.alphaTween.gameObject.SetActive(false);
            View.loadingSlider.gameObject.SetActive(false);
            if (isBattleScene)
            {
                WorldMapLoader.Instance.LoadBattleMap(sceneResId, OnLoad2DMapFinish);
            }
            else
            {
                WorldMapLoader.Instance.LoadWorldMap(sceneResId, OnLoad2DMapFinish);
            }
        }
        else
        {
            View.alphaTween.gameObject.SetActive(true);
            View.loadingSlider.gameObject.SetActive(!isBattleScene);

            _finishLoadMap = false;

            _view.LoadSceneEffectGO.SetActive(false);

            _view.LoadSceneEffect.enabled = false;
            _view.LoadSceneEffect.enabled = true;
            LoadSceneMap();
        }
    }

    private void LoadSceneMap()
    {
        curLoadTime = PlayerPrefsExt.GetPlayerFloat(SceneLoadTimeKey, DefaultLoadTime);
        Debug.Log(curLoadTime);
        startLoadTime = Time.unscaledTime;

        if (_isBattleScene)
        {
            JSTimer.Instance.StartCoroutine(LoadBattleMapNextFrame());
        }

        else
        {
            WorldMapLoader.Instance.LoadWorldMap(_sceneResId, mainPlayerPos, OnLoadMapFinish);
            JSTimer.Instance.StartCoroutine(UpdateSliderProc());
        }
    }

    private IEnumerator LoadBattleMapNextFrame()
    {
        yield return null;
        WorldMapLoader.Instance.LoadBattleMap(_sceneResId, delegate
        {
            OnLoadMapFinish();
        } );
        JSTimer.Instance.StartCoroutine(UpdateSliderProc());
    }

    private IEnumerator UpdateSliderProc()
    {
        while (!_finishLoadMap)
        {
            float nextValue = GetRealtimeSlider();
            UpdateSliderInfo(nextValue);
            yield return null;
        }
        float newLoadTime = Time.unscaledTime - startLoadTime;
        PlayerPrefsExt.SetPlayerFloat(SceneLoadTimeKey, (newLoadTime + (curLoadTime == DefaultLoadTime ? newLoadTime : curLoadTime)) / 2f);
        var sliderValue = GetRealtimeSlider();
        var speed = 2f;   //加载完成后，剩余进度条速度  --by chaojian
        while (true)
        {
            sliderValue += speed * Time.unscaledDeltaTime;
            if (sliderValue >= 1f)
                break;
            UpdateSliderInfo(sliderValue);
            yield return null;
        }
        FadeOut();
    }

    private float GetRealtimeSlider()
    {
        return Mathf.Clamp01((Time.unscaledTime - startLoadTime) / curLoadTime * (1 - bufferSlider * 2) + bufferSlider);
    }

    private void OnLoadMapFinish()
    {
        if (_onFinishLoadMap != null)
            _onFinishLoadMap(_callbackVal);
        _onFinishLoadMap = null;

        _finishLoadMap = true;
    }

    private void OnLoad2DMapFinish()
    {
        OnLoadMapFinish();
        Close();
    }

    private void UpdateSliderInfo(float percent)
    {
        if (_isBattleScene)
            return;
        View.loadingSlider.value = percent;
        View.loadingLbl.text = View.loadingSlider.value.ToString("P0");
    }

    private void FadeOut()
    {
        //View.alphaTween.PlayReverse();

        //EventDelegate.Set(View.alphaTween.onFinished, Close);
        if (_isBattleScene)
        {
            _view.LoadSceneEffectGO.SetActive(true);
            JSTimer.Instance.SetupCoolDown(
                "callback" + GetInstanceID()
                , 0.2f
                , null
                , delegate
                {
                    if (_onFinishFadeOut != null)
                        _onFinishFadeOut();
                    _onFinishFadeOut = null;
                });
            JSTimer.Instance.SetupCoolDown(
                "OnLoadMapFinish" + GetInstanceID()
                , 1f
                , null
                , delegate
                {
                    Close();
                });
        }
        else
        {
            if (_onFinishFadeOut != null)
                _onFinishFadeOut();
            _onFinishFadeOut = null;
            Close();
        }
    }

    private void Close()
    {
        UIModuleManager.Instance.CloseModule(NAME);
    }

    private string SceneLoadTimeKey
    {
        get { return "SceneLoadTime" + _sceneResId; }
    }
}