using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using AppDto;
using AssetPipeline;

public static class UIHelper
{
    //品质对应颜色
    public static readonly Dictionary<int, string> QualityToColor = new Dictionary<int, string>
    { 
        {1, ColorConstantV3.Color_White_Str },
        {2, ColorConstantV3.Color_Green_Str },
        {3, ColorConstantV3.Color_Blue_Str },
        {4, ColorConstantV3.Color_Purple_Str },
        {5, ColorConstantV3.Color_Orange_Str },
        {6, ColorConstantV3.Color_Red_Str },
    };

    /**
	 *  UIPanel AddChild UIPanel and UIPanel AddChild UIWidget
	 **/
    public static void AdjustDepth(GameObject go, int adjustment = 1)
    {
        if (go != null)
        {
            if (go.GetComponent<UIPanel>() == null)
            {
                AdjustDepthWithoutPanel(go, adjustment);
            }
            else
            {
                if (go.transform.parent != null)
                {
                    UIPanel parentPanel = go.transform.parent.GetComponentInParent<UIPanel>();

                    // 注意:这里重置 panel.depth 是因为缓存的 panel.depth 已经设置过了,继续设置会导致 panel.depth 无限增加
                    UIPanel panel = go.GetComponent<UIPanel>();
                    if (panel != null && panel.depth >= 10)
                    {
                        panel.depth = 1;
                    }
                    // end

                    if (parentPanel != null)
                    {
                        NGUITools.AdjustDepth(go, parentPanel.depth + adjustment);
                    }
                }
                else
                    NGUITools.AdjustDepth(go, adjustment);
            }
        }
    }

    /**
	 *  UIPanel AddChild UIWidget
	 **/
    public static int AdjustDepthWithoutPanel(GameObject go, int adjustment)
    {
        if (go != null)
        {
            UIWidget[] widgets = go.GetComponentsInChildren<UIWidget>(true);
            for (int i = 0, imax = widgets.Length; i < imax; ++i)
            {
                UIWidget w = widgets[i];
                w.depth = w.depth + adjustment;
            }
            return 2;
        }
        return 0;
    }

    public static int GetMaxDepthWithPanelAndWidget(this GameObject go)
    {
        if (go == null)
        {
            return 0;
        }

        var rootDepth = 0;
        var panel = go.GetComponent<UIPanel>();
        if (panel != null)
        {
            rootDepth = panel.depth;
        }

        List<int> depthList = new List<int>();
        UIPanel[] panels = go.GetComponentsInChildren<UIPanel>(true);
        panels.ForEach(s => depthList.Add(s.depth));

        depthList.Sort();

        var max = depthList[depthList.Count - 1];
        return rootDepth <= max ? max : rootDepth + max;
    }

    public static UIButton CreateBigBaseBtn(GameObject parent, 
        string label, 
        EventDelegate.Callback callback = null,
        string buttonName = "BaseButton",
        string goName = "Button")
    {
        GameObject go = NGUITools.AddChild(parent, (GameObject)AssetPipeline.ResourcePoolManager.Instance.LoadUI(buttonName));
        go.name = goName;
        go.GetComponentInChildren<UILabel>().text = label;
        UIButton btn = go.GetComponent<UIButton>();
        if (btn != null && callback != null)
        {
            EventDelegate.Set(btn.onClick, callback);
        }
        return btn;
    }

    public static void Destroy(UnityEngine.Object pObject)
    {
        AssetPipeline.ResourcePoolManager.Destroy(pObject);
        pObject = null;
    }

    public static UIButton CreateBaseBtn(GameObject parent, string label, EventDelegate.Callback callback = null,
                                         string goName = "Button")
    {
        GameObject go = NGUITools.AddChild(parent,
                            (GameObject)AssetPipeline.ResourcePoolManager.Instance.LoadUI("BaseSmallButton"));
        go.name = goName;
        go.GetComponentInChildren<UILabel>().text = label;
        UIButton btn = go.GetComponent<UIButton>();
        if (btn != null && callback != null)
        {
            EventDelegate.Set(btn.onClick, callback);
        }
        return btn;
    }

    public static CostButton CreateCostBtn(GameObject parent, string label, string icon,
                                           EventDelegate.Callback callback = null, string goName = "CostButton")
    {
        GameObject go = NGUITools.AddChild(parent, (GameObject)AssetPipeline.ResourcePoolManager.Instance.LoadUI("CostButton"));
        go.name = goName;
        CostButton costBtn = go.GetComponent<CostButton>();
        costBtn.NameStr = label;
        costBtn.CostIconSprite = icon;
        UIButton btn = go.GetComponent<UIButton>();
        if (btn != null && callback != null)
        {
            EventDelegate.Set(btn.onClick, callback);
        }
        return costBtn;
    }


    public static SliderToggle CreateSliderToggle(GameObject parent, bool state, Action<bool> OnSliderToggleClick)
    {
        GameObject go = NGUITools.AddChild(parent,
                            (GameObject)AssetPipeline.ResourcePoolManager.Instance.LoadUI("SliderToggle"));
        SliderToggle btn = go.GetComponent<SliderToggle>();
        if (btn != null)
        {
            btn.SetState(state);
            btn.OnSliderToggleClick = OnSliderToggleClick;
        }
        return btn;
    }

    public static UILabel AddDescLbl(GameObject table, UIFont bitmapFont, string text = "")
    {
        UILabel descLbl = NGUITools.AddWidget<UILabel>(table, 10);
        descLbl.cachedGameObject.name = "descLbl";
        descLbl.bitmapFont = bitmapFont;
        descLbl.fontSize = 20;
        descLbl.overflowMethod = UILabel.Overflow.ResizeHeight;
        descLbl.width = 345;
        descLbl.pivot = UIWidget.Pivot.TopLeft;
        descLbl.spacingY = 6;

        descLbl.text = text;
        return descLbl;
    }

    public static UILabel AddDescLblWithTitle(GameObject table, UIFont bitmapFont, string title, string text = "", string titleColor = ColorConstantV3.Color_Yellow_Str)
    {
        UILabel titleLbl = NGUITools.AddWidget<UILabel>(table, 10);
        titleLbl.cachedGameObject.name = "titleLbl";
        titleLbl.bitmapFont = bitmapFont;
        titleLbl.fontSize = 22;
        titleLbl.overflowMethod = UILabel.Overflow.ResizeHeight;
        titleLbl.width = 318;
        titleLbl.pivot = UIWidget.Pivot.TopLeft;
        titleLbl.text = title.WrapColor(titleColor);
        //		AddSpace (titleLbl.cachedGameObject, 2);
        UILabel contentLbl = AddDescLbl(titleLbl.cachedGameObject, bitmapFont, text);
        contentLbl.cachedTransform.localPosition = new Vector3(0f, -28f, 0f);
        return contentLbl;
    }

    public static CdSpriteTween AddCdTweener(GameObject parent, JSTimer.CdTask task)
    {
        GameObject modulePrefab = AssetPipeline.ResourcePoolManager.Instance.LoadUI("CoolingEffect") as GameObject;
        GameObject module = NGUITools.AddChild(parent, modulePrefab);
        CdSpriteTween com = module.GetMissingComponent<CdSpriteTween>();
        com.Setup(task);
        return com;
    }

    public static void AddButtonClickSound(GameObject buttonGo, string soundName)
    {
        ResourcePoolManager.Instance.LoadAudioClip(soundName, asset =>
            {
                if (asset != null && null != buttonGo)
                {
                    AudioClip audioClip = asset as AudioClip;

                    UIPlaySound playSound = buttonGo.GetComponent<UIPlaySound>();
                    if (playSound == null)
                    {
                        playSound = buttonGo.AddComponent<UIPlaySound>();
                    }

                    playSound.audioClip = audioClip;
                    playSound.trigger = UIPlaySound.Trigger.OnClick;
                }
                else
                {
                    GameDebuger.Log("Can not find the sound of " + soundName);
                }
            });
    }

    public static void DisposeUITexture(UITexture uiTexture)
    {
        if (uiTexture != null)
        {
            Texture tex = uiTexture.mainTexture;
            if (tex != null)
            {
                uiTexture.mainTexture = null;
                Resources.UnloadAsset(tex);
            }
        }
    }

    public static void RemoveNGUIEvent(GameObject go)
    {
        UIButton[] buttons = go.GetComponentsInChildren<UIButton>(true);
        buttons.ForEach(item => item.onClick.Clear());

        UIToggle[] toggles = go.GetComponentsInChildren<UIToggle>(true);
        toggles.ForEach(item => item.onChange.Clear());

        UIEventListener[] eventListeners = go.GetComponentsInChildren<UIEventListener>(true);
        eventListeners.ForEach(item => item.ClearAllListener());

        UISlider[] sliders = go.GetComponentsInChildren<UISlider>(true);
        sliders.ForEach(item => item.onChange.Clear());

        UIPopupList[] tUIPopupLists = go.GetComponentsInChildren<UIPopupList>(true);
        tUIPopupLists.ForEach(item => item.onChange.Clear());
    }

    #region Tween Helper

    public static void PlayAlphaTween(UIRect rect, float fromAlpha = 1f, float toAlpha = 0f, float duration = 0.3f,
                                      float delay = 0f, TweenCallback onFinish = null)
    {
        rect.alpha = fromAlpha;
        if (fromAlpha == toAlpha)
            return;

        var tweener = DOTween.To(() => rect.alpha, x => rect.alpha = x, toAlpha, duration);
        tweener.SetDelay(delay);
        tweener.OnComplete(onFinish);
    }

    #endregion

    #region iconAtlas helper

    private static Dictionary<string, UIAtlas> _uiAtlasDic;
    public static void SetSkillQualityIcon(UISprite uiSprite, int quality, bool makePixelPerfect = false)
    {
        SetIcon(uiSprite, "skill_ib_" + quality, "SkillIconAtlas", makePixelPerfect);
    }

    public static void SetItemQualityIcon(UISprite uiSprite, int quality, bool makePixelPerfect = false)
    {
        SetCommonIcon(uiSprite, quality <= 0 ? "item_ib_bg" : "item_ib_" + quality, makePixelPerfect);
    }

    //icon图集弱引用Map
    public static void SetFactionIcon(UISprite uiSprite, int factionId, bool makePixelPerfect = false)
    {
        SetCommonIcon(uiSprite, "faction_" + factionId, makePixelPerfect);
    }

    public static void SetOtherIcon(UISprite uiSprite, string icon, bool makePixelPerfect = false)
    {
        SetIcon(uiSprite, icon, "OtherIconAtlas", makePixelPerfect);
    }
    
    public static void SetCommonIcon(UISprite uiSprite, string icon, bool makePixelPerfect = false)
    {
        SetIcon(uiSprite, icon, "CommonUIAltas", makePixelPerfect);
    }
    public static void SetItemIcon(UISprite uiSprite, string icon, bool makePixelPerfect = false)
    {
        SetIcon(uiSprite, icon, "ItemIconAtlas", makePixelPerfect);
    }

    public static void SetSkillIcon(UISprite uiSprite, string icon, bool makePixelPerfect = false)
    {
        SetIcon(uiSprite, icon, "SkillIconAtlas", makePixelPerfect);
    }

    public static void SetPetIcon(UISprite uiSprite, string icon, bool makePixelPerfect = false)
    {
        SetIcon(uiSprite, icon, "PetIconAtlas", makePixelPerfect);
    }

    public static void SetMountIcon(UISprite uiSprite, int pTexture, bool makePixelPerfect = false)
    {
        SetPetIcon(uiSprite, "large_" + pTexture, makePixelPerfect);
    }

    public static void SetQuartyIcon(UISprite uisprite, string icon, bool makePixelPerfect = true)
    {
        SetIcon(uisprite, icon, "QuartzAtlas", makePixelPerfect);
    }

    //虚拟货币的Icon
    public static void SetAppVirtualItemIcon(UISprite uiSprite,AppVirtualItem.VirtualItemEnum id, bool makePixelPerfect = false)
    {
        SetCommonIcon(uiSprite, "virualItem_" + (int)id, makePixelPerfect);
    }

    public static UIAtlas GetIconAtlas(string icon, string atlasPrefix)
    {
        if (string.IsNullOrEmpty(icon))
            return null;

        string atlasName = IconConfigManager.GetAltasName(icon, atlasPrefix);
        if (string.IsNullOrEmpty(atlasName))
            return null;

        if (_uiAtlasDic == null)
            _uiAtlasDic = new Dictionary<string, UIAtlas>(8);

        UIAtlas result = null;
        if (_uiAtlasDic.TryGetValue(atlasName, out result))
        {
            if (result == null)
            {
                GameObject atlasPrefab = AssetManager.Instance.LoadAsset(atlasName, ResGroup.UIAtlas) as GameObject;
                if (atlasPrefab != null)
                {
                    result = atlasPrefab.GetComponent<UIAtlas>();
                    _uiAtlasDic[atlasName] = result;
                }
            }
        }
        else
        {
            GameObject atlasPrefab = AssetManager.Instance.LoadAsset(atlasName, ResGroup.UIAtlas) as GameObject;
            if (atlasPrefab != null)
            {
                result = atlasPrefab.GetComponent<UIAtlas>();
                _uiAtlasDic.Add(atlasName, result);
            }
        }

        return result;
    }

    private static void SetIcon(UISprite uiSprite, string icon, string atlasName, bool makePixelPerfect)
    {
        if (uiSprite == null)
            return;

        if (string.IsNullOrEmpty(icon))
        {
            uiSprite.spriteName = "0";
            return;
        }

        atlasName = IconConfigManager.GetAltasName(icon, atlasName);

        if (uiSprite.atlas == null || uiSprite.atlas.name != atlasName)
        {
            if (_uiAtlasDic == null)
                _uiAtlasDic = new Dictionary<string, UIAtlas>(8);

            UIAtlas result = null;
            if (_uiAtlasDic.TryGetValue(atlasName, out result))
            {
                if (result == null)
                {
                    GameObject atlasPrefab =
                        AssetManager.Instance.LoadAsset(atlasName, ResGroup.UIAtlas) as GameObject;
                    if (atlasPrefab != null)
                    {
                        result = atlasPrefab.GetComponent<UIAtlas>();
                        _uiAtlasDic[atlasName] = result;
                    }
                }
            }
            else
            {
                GameObject atlasPrefab = AssetManager.Instance.LoadAsset(atlasName, ResGroup.UIAtlas) as GameObject;
                if (atlasPrefab != null)
                {
                    result = atlasPrefab.GetComponent<UIAtlas>();
                    _uiAtlasDic.Add(atlasName, result);
                }
            }

            uiSprite.atlas = result;
        }

        if (uiSprite.atlas != null && uiSprite.atlas.GetSprite(icon) == null)
        {
            icon = "0";
        }

        uiSprite.spriteName = icon;
        UIButton uibutton = uiSprite.GetComponent<UIButton>();
        if (uibutton != null)
        {
            uibutton.normalSprite = icon;
        }

        if (makePixelPerfect)
            uiSprite.MakePixelPerfect();
    }

    #endregion


    #region view 相关

    public static bool IsViewUpdateEnable(BaseView pBaseView)
    {
        return null != pBaseView && null != pBaseView.gameObject && pBaseView.gameObject.activeInHierarchy;
    }

    #endregion

    public enum ItemTipType
    {
        None = 0,
        NormalTip = 1,
        GainWay = 2
    }

    #region 更改按钮选中状态

    public enum BtnType
    {
        Little,
        Middle,
        Big,
    }

    public static void SetBtnState(UILabel btnLbl, UISprite sprite, bool select, BtnType btnType = BtnType.Little)
    {
        /*
        btnLbl.effectStyle = UILabel.Effect.Shadow;
        if (select)
        {
            btnLbl.effectDistance = btnType == BtnType.Big ? Vector2.one : -Vector2.one;
            btnLbl.color = btnType == BtnType.Big ? ColorConstantV3.Color_BigButtonFontColor : ColorConstantV3.Color_ButtonFontSelectColor;
            btnLbl.effectColor = btnType == BtnType.Big ? ColorConstantV3.Color_BigButtonFontSelectEffColor : ColorConstantV3.Color_ButtonFontSelectEffColor;
        }
        else
        {
            btnLbl.effectDistance = Vector2.one;
            btnLbl.color = ColorConstantV3.Color_ButtonFontColor;
            btnLbl.effectColor = ColorConstantV3.Color_ButtonFontEffColor;
        }*/

        if (btnType == BtnType.Little)
        {
            sprite.spriteName = select ? "button-001-selected" : "button-001";
        }
        else if (btnType == BtnType.Middle)
        {
            sprite.spriteName = select ? "button-001-selected" : "button-001";
        }
        else if (btnType == BtnType.Big)
        {
            sprite.spriteName = select ? "green-big-button" : "button-001";
        }
    }

    public static void SetBtnState(UIButton button, bool select, BtnType btnType = BtnType.Little)
    {
        /*
        UILabel btnLbl = button.GetComponentInChildren<UILabel>();
        btnLbl.effectStyle = UILabel.Effect.Shadow;
        if (select)
        {
            btnLbl.effectDistance = btnType == BtnType.Big ? Vector2.one : -Vector2.one;
            btnLbl.color = btnType == BtnType.Big ? ColorConstantV3.Color_BigButtonFontColor : ColorConstantV3.Color_ButtonFontSelectColor;
            btnLbl.effectColor = btnType == BtnType.Big ? ColorConstantV3.Color_BigButtonFontSelectEffColor : ColorConstantV3.Color_ButtonFontSelectEffColor;
        }
        else
        {
            btnLbl.effectDistance = Vector2.one;
            btnLbl.color = ColorConstantV3.Color_ButtonFontColor;
            btnLbl.effectColor = ColorConstantV3.Color_ButtonFontEffColor;
        }*/

        if (btnType == BtnType.Little)
        {
            button.normalSprite = select ? "button-001-selected" : "button-001";
        }
        else if (btnType == BtnType.Middle)
        {
            button.normalSprite = select ? "button-001-selected" : "button-001";
        }
        else if (btnType == BtnType.Big)
        {
            button.normalSprite = select ? "green-big-button" : "button-001";
        }
    }

    #endregion

    public static void RepositionDelay(this UIGrid pUIGrid, Action pFinishHandler = null)
    {
        var tCdTaskName = "RepositionDelay" + pUIGrid.GetHashCode().ToString();
        JSTimer.Instance.CancelCd(tCdTaskName);
        JSTimer.Instance.SetupCoolDown(tCdTaskName, 0.1f, null, () =>
            {
                if (null == pUIGrid)
                    return;
                pUIGrid.Reposition();

                if (null != pFinishHandler)
                    pFinishHandler();
            });
    }

    public static void RepositionDelay(this UITable pUITable, Action pFinishHandler = null)
    {
        JSTimer.Instance.SetNextFrame(() => {
            pUITable.Reposition();
            if (null != pFinishHandler)
                pFinishHandler();
        });
    }

    /// <summary>
    /// 加载技能texture
    /// </summary>
    /// <param name="uITexture"></param>
    /// <param name="texName"></param>
    public static void SetUITexture(UITexture uITexture,
        string texName,
        bool makePerfect = true, 
        Action callback = null)
    {
        if (uITexture.mainTexture != null && string.Equals(uITexture.mainTexture.name, texName))
            return;

        ResourcePoolManager.Instance.LoadImage(texName,asset =>
        {
            var texture = asset as Texture2D;

            if (texture == null)
            {
                GameDebuger.LogError("cann't load texture, skill id is " + texName);
                GameUtil.SafeRun(callback);
                return;
            }
#if UNITY_EDITOR

#else
            //DisposeUITexture(uITexture);
#endif
            uITexture.mainTexture = texture;
            if (makePerfect)
                uITexture.MakePixelPerfect();
            GameUtil.SafeRun(callback);
        });
    }

    public static void SetTextQualityColor(UILabel label, int quality, string str = "")
    {
        if (QualityToColor.ContainsKey(quality))
            label.text = string.IsNullOrEmpty(str)?label.text.WrapColor(QualityToColor[quality]):str.WrapColor(QualityToColor[quality]);
    }

    public static string GetStepGradeName(int level, bool needStar, string defaultStepGradeName = "")
    {
        ExpGrade expGrade = DataCache.getDtoByCls<ExpGrade>(level);
        if (expGrade == null)
        {
            GameDebuger.Log("没有此等级对应的ExpGrade数据" + level);
            return defaultStepGradeName;
        }

        return level + "级";
    }

    public static void SetDynamicScrollView(UIScrollView scrollView, int count)
    {

    }
}

