// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MiniWorldMapViewController.cs
// Author   : fish
// Created  : 2015/4/17
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using AssetPipeline;
using UniRx;
using UnityEngine;

public partial interface IMiniWorldMapViewController
{
    UniRx.IObservable<int> SceneBtnClickEvt { get; }
}

public partial class MiniWorldMapViewController    {

    private OneShotUIEffect _oneShotUIEffect;
    private Dictionary<int, GameObject> sceneBtnDic = null;
    public const string WorldMap = "WorldMap";

    private Subject<int> sceneBtnClick;

    protected override void InitData()
    {
        sceneBtnClick = new Subject<int>();
    }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _view.CurrMapBtn.gameObject.SetActive(false);
        var textureName = ChangeSkinHelper.GetWorldMapTexResName(WorldMap);
        var asset = AssetManager.Instance.LoadAsset(textureName, AssetPipeline.ResGroup.Image);
        ChangeSkinHelper.ChangeTexture(_view.Map_UITexture, asset as Texture);
        AssetManager.Instance.UnloadBundle(textureName, AssetPipeline.ResGroup.Image);
        
        var maps = DataCache.getArrayByCls<SceneMap>();
        InitSceneBtn(maps);
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IWorldMapData data){
        UIHelper.SetPetIcon(_view.PlayerHeadSprite, data.HeadTex.ToString());
        UpdateHeroSpritePos(data.CurSceneId);
    }

    private void UpdateHeroSpritePos(int curSceneId)
    {
        // 设置我当前的位置
        GameObject sceneBtn = null;
        sceneBtnDic.TryGetValue(curSceneId, out sceneBtn);
        if (sceneBtn != null)
        {
            _view.HeroSprite.SetActive(true);
            var heroPosition = sceneBtn.transform.localPosition
                               + new Vector3(0f, sceneBtn.gameObject.FindScript<BoxCollider>("").size.y + 10, 0f);
            _view.HeroSprite.transform.localPosition = heroPosition;

            if (_oneShotUIEffect == null)
            {
//                _oneShotUIEffect = OneShotUIEffect.BeginFollowEffect("ui_eff_MiniWorldMap_Effect", _view.EffPos,
//                    Vector2.zero, 1, true);
            }
        }
        else
        {
            _view.HeroSprite.SetActive(false);
        }
    }
    
    public void InitSceneBtn(IEnumerable<SceneMap> maps)
    {
        sceneBtnDic = new Dictionary<int, GameObject>();
        maps.ForEach(map =>
        {
            var sceneBtn = _view.PlaceGroup.FindGameObject("Place_" + map.id);
            if (sceneBtn != null)
            {
                sceneBtnDic.Add(map.id, sceneBtn);
                sceneBtn.OnClickAsObservable().Subscribe(_ => sceneBtnClick.OnNext(map.id));
            }
        });
    } 

    protected override void OnDispose()
    {
        if (_oneShotUIEffect != null)
        {
            _oneShotUIEffect.Dispose();
        }
        sceneBtnClick = sceneBtnClick.CloseOnceNull();
        base.OnDispose();
    }

    public UniRx.IObservable<int> SceneBtnClickEvt {
        get { return sceneBtnClick; }
    }
}
