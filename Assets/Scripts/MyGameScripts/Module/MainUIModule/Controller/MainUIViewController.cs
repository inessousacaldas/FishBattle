// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MainUIViewController.cs
// Author   : fish
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using UnityEngine;

public class pool<T>
{
    private Subject<T> stream;
    private Queue<T> queue;
    private float time;
    private IDisposable _disposable;
    public void dispose()
    {
        if (_disposable != null)
        {
            _disposable.Dispose();
            _disposable = null;
        }
    }

    public void addData(T t)
    {
        queue.Enqueue(t);
    }

    public void aaa()
    {
        _disposable = Observable.Interval(TimeSpan.FromMilliseconds(2000)).Subscribe(_ =>
        {
            if (queue == null || queue.Count <= 0)
            {
                ;
            }
            else
            {
                stream.OnNext(queue.Dequeue());
            }

        });
    }

    public void StopAndClean()
    {
    }
    
    public void Pause(){}

    public void Resume()
    {
    }
}

public partial interface IMainUIViewController
{
    ICharacterPropertyController PlayerInfo { get; }
    IRightExpandBtnGroupController ExpandBtnGroupCtrl{ get; }
    IMainUIExpandContentViewController ExpandCtrl { get; }

    ChatBoxController ChatBoxCtrl { get; }

    GameObject WeatherBtn_GO { get; }

    void UpdateByLayerChange(UIMode mode, MainUIDataMgr.ShowState showState);
    void UpdateBuffShow(IEnumerable<int> buffList);
    void UpdateAllBtnGroup(bool b, MainUIDataMgr.ShowState showState);

    void OnPlayerInfoBtnClick(IMainUIData data);
    void OnClickPopupUseMissionPropsBtn(AppMissionItem missionItem = null,System.Action callback = null,bool callbackState = false);
    void OnUsePropsBtnClick();
    void OnClickPopupUseMissionPropsClose();
    void Show();
    void Hide();
    void ChangeMainCrew(long id);
    void SetCrewInfo(MainCrewInfoNotify notify);
    void SetActivityPollGo(bool b);
    void UpdateTowerGuildUI(ITowerData data);//四轮之塔指引UI
    void UpdateKungFuInfo(IMartialData data);   //武术大会
}

public partial class MainUIViewController
{
    private const string DeviceStatusTimer = "DeviceStatusTimer";
        
    private UISprite[] wifi;
    private List<BuffShowItemController> _buffList = new List<BuffShowItemController>(); 

    private CharacterPropertyController _playerInfo;
    private RightExpandBtnGroupController _expandBtnGroupController;
    private IMainUIExpandContentViewController expandCtrl;
    private ChatBoxController _chatBoxCtrl;
//    RedPointController _assistSkillRedPointCtrl;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        _view.Button_PlayerInfo.SetActive(false);
        _view.TempBtn_UIButton.gameObject.SetActive(false);
        wifi = new[]{_view.wifi_1_UISprite, _view.wifi_2_UISprite, _view.wifi_3_UISprite};
            
        InitCharactorInfo();
        InitExpandContentInfo();
        InitChatBox();
        InitDeviceStatusTimer();
        InitRightBtnGroup();
        SetCrewInfo(ModelManager.Player.GetCrewInfoDto());

        var pos = WorldManager.OnHeroPosStreamChange == null ? Vector3.zero : WorldManager.OnHeroPosStreamChange.LastValue;
        var s = DataCache.getDtoByCls<SceneMap>(ModelManager.IPlayer.SceneID);
        var sceneName = s == null ? string.Empty : s.name;
        _view.SceneName_UILabel.text = string.Format("{0} {1}, {2}",sceneName, (int) pos.x * 10, (int) pos.z * 10);

        SetFuncOpen();

        //红点测试xjd
//        _assistSkillRedPointCtrl = AddCachedChild<RedPointController, RedPointView>(View.Button_lifeskill_UIButton.gameObject, RedPointView.NAME);
//        _assistSkillRedPointCtrl.InitView(1, 9);
    }

    private void SetFuncOpen()
    {
        View.GuideBtn_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_30));
        View.ScheduleBtn_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_38));
        View.Button_Daily_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_27));
        View.Button_Equipment_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_41));
        View.Button_quartz_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_43));
        View.Button_Pack_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_2));
        View.Button_Skill_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_40));
        View.Button_lifeskill_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_42));
        View.Button_Crew_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_10));
        View.Button_Recruit_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_44));
        View.Button_Friend_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_8));
        View.Button_Ranking_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_32));
        View.Button_Trade_UIButton.gameObject.SetActive(
            FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_17)
            ||FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_18));
        View.Button_guild_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_34));

        int cout = 0;
        for (int i = 0; i < View.ButtonGrid_UIGrid.transform.childCount; i++)
        {
            var go = View.ButtonGrid_UIGrid.GetChild(i);
            if (go != null && go.gameObject.activeSelf)
                cout += 1;
        }
        View.BottomRightAnchorSprite_UISprite.width = 74 + cout * 70;

        _expandBtnGroupController.SetFuncOpen();

        View.TopLeftBtnGrid_UIGrid.Reposition();
        View.ButtonGrid_UIGrid.Reposition();
        View.TopBtnGrid_UIGrid.Reposition();

        Bounds b = NGUIMath.CalculateRelativeWidgetBounds(View.TopBtnGrid_UIGrid.transform);
        View.LeftShrinkBtn_UISprite.transform.localPosition = new Vector3(-119 - (int)b.size.x, View.LeftShrinkBtn_UISprite.transform.localPosition.y);
        View.LeftShrinkBtn_TweenPosition.from = View.LeftShrinkBtn_UISprite.transform.localPosition;
    }

    private void InitChatBox()
    {
        _chatBoxCtrl = AddController<ChatBoxController, ChatBox>(_view.ChatBox);
    }

    protected override void OnDispose()
    {
        JSTimer.Instance.CancelTimer(DeviceStatusTimer);
        UICamera.onClick -= OnCameraClick;
        base.OnDispose();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {
        
    }


    protected override void RegistCustomEvent()
    {
        //人物升级数据流 用于更新开放接口功能
        _disposable.Add(PlayerModel.LvUpStream.SubscribeAndFire(_=> { SetFuncOpen(); }));
        // todo fish:临时包裹订阅物品更新红点
        _disposable.Add(BackpackDataMgr.Stream.SubscribeAndFire(data =>
        {
            var _data = data.MainUIViewData;
            ShowTempBagBtn(_data.IsTempPackNotNull);
        }));
        //断线重连不会将前面的监听给释放掉，所以需要在重新监听之前释放掉之前的监听。
        UICamera.onClick -= OnCameraClick;
        SystemTimeManager.Instance.OnSystemWeatherChange -= OnSystemWeatherChange;
        SystemTimeManager.Instance.OnSystemTimeChange -= OnSystemTimeChange;

        SystemTimeManager.Instance.OnSystemWeatherChange += OnSystemWeatherChange;
        SystemTimeManager.Instance.OnSystemTimeChange += OnSystemTimeChange;

        UICamera.onClick += OnCameraClick;

        //主角信息变化
        _disposable.Add(PlayerModel.Stream.Subscribe(x =>
        {
            UpdatePlayerInfo();
        }));

        //红点测试xjd
        _disposable.Add(RedPointDataMgr.Stream.Subscribe(e =>
        {
            //生产按钮红点
//            if (e.PushSingleData.redPointType == 1)
//            {
//                _assistSkillRedPointCtrl.SetShow(e.PushSingleData.isShow, e.PushSingleData.num);
//            }
        }));
        
        EventDelegate.Set(_view.Button_ChargeTest_UIButton.onClick, () =>
        {
            var id = 50113;
            PayManager.Instance.Charge(id);
        });
    }
    
    public void Show()
    {
        _view.Show();
    }

    public void Hide()
    {
        _view.Hide();
    }
    
    protected override void RemoveCustomEvent()
    {
        SystemTimeManager.Instance.OnSystemWeatherChange -= OnSystemWeatherChange;
        SystemTimeManager.Instance.OnSystemTimeChange -= OnSystemTimeChange;
    }

    private void InitCharactorInfo()
    {
        _playerInfo = AddController<CharacterPropertyController, CharacterPropertyView>(_view.CharacterPropertyView);
    }

    // 客户端自定义代码
    private void InitExpandContentInfo()
    {
        expandCtrl = AddChild<MainUIExpandContentViewController, MainUIExpandContentView>(_view.ExpandPanelAnchor,
            MainUIExpandContentView.NAME);

        _disposable.Add(TeamDataMgr.Stream.SubscribeAndFire(data => expandCtrl.UpdateView(data)));

        _disposable.Add(LayerManager.Stream.SubscribeAndFire(data => expandCtrl.UpdateView(data)));

    }

    protected override void UpdateDataAndView(IMainUIData data)
    {
        if (data == null)
            return;

        // UpdatePlayerInfoBtn
        var playerDto = data.SelectedPlayer;
        if (playerDto == null)
            _view.Button_PlayerInfo.SetActive(false);
        else
        {
            _view.Button_PlayerInfo.SetActive(true);
            _view.lvLbl_UILabel.text = string.Format("[b]{0}[-]", playerDto.grade);
            _view.nameLbl_UILabel.text = string.Format("[b]{0}[-]", playerDto.name);

            var dto = playerDto.charactor as MainCharactor;
            //_view.icon_UISprite.spriteName = dto == null ? "" : dto.texture.ToString();
            //todo 头像男女区别
            UIHelper.SetPetIcon(_view.icon_UISprite, (dto.gender == 1 ? 101 : 103).ToString());
        }

        UpdatePanelsShowState(data.panelShowState);
        _playerInfo.UpdateView(data);
        _playerInfo.UpdateBuffList();
        UpdatePlayerInfo();
        _view.SceneName_UILabel.text = data.GetSceneMapName;
    }

    public void SetCrewInfo(MainCrewInfoNotify mainCrew)
    {
        _view.CrewLvLb_UILabel.gameObject.SetActive(mainCrew != null);
        _view.CrewIcon_UISprite.gameObject.SetActive(mainCrew != null);
        if (mainCrew == null || mainCrew.id < 0)
        {
            UIHelper.SetPetIcon(_view.CrewIcon_UISprite, "");
            _view.CrewLvLb_UILabel.text = "";
            _view.AddCrew.gameObject.SetActive(true);
            _view.CrewLvLb_UILabel.gameObject.SetActive(false);
        }
        else
        {
            _view.AddCrew.gameObject.SetActive(false);
            _view.CrewLvLb_UILabel.gameObject.SetActive(true);
            UIHelper.SetPetIcon(_view.CrewIcon_UISprite, mainCrew.crew.icon);
            _view.CrewLvLb_UILabel.text = mainCrew.grade.ToString();
        }
    }

    private void UpdatePanelsShowState(MainUIDataMgr.ShowState _showState)
    {
        if (_showState.allBtnPanelHide)
        {
            UpdateAllBtnGroup(true, _showState);
            return;
        }

        //右上方按钮栏
        _view.LeftShrinkBtn_UIButton.gameObject.SetActive(true);
        _expandBtnGroupController.ShowOrHideBtn(false);
        _view.SceneBtnGroup_TweenPosition.Play(false);
        _view.TL_UpRoll_TweenPosition.Play(!_showState.topBtnPanelShow);
        _view.LeftShrinkBtn_TweenPosition.Play(!_showState.topBtnPanelShow);
        _view.LeftShrinkBtn_UIButton.sprite.flip = _showState.topBtnPanelShow ?
            UIBasicSprite.Flip.Horizontally : UIBasicSprite.Flip.Vertically;

        //右下方按钮栏
        _view.BR_DownRoll_TweenPosition.Play(false);
        _view.BottomGroup_TweenPosition.Play(!_showState.rightBottomBtnPanelShow);
        _view.BottomRightGroup_TweenPosition.Play(!_showState.rightBottomBtnPanelShow);

        //任务栏
        expandCtrl.UpdateExpandContentState(_showState.expandPanelShow);
    }

    public void UpdateAllBtnGroup(bool b, MainUIDataMgr.ShowState showState)
    {
        //_view.LeftShrinkBtn_UIButton.gameObject.SetActive(!b);
        _view.TL_UpRoll_TweenPosition.Play(b || !showState.topBtnPanelShow);
        _view.BR_DownRoll_TweenPosition.Play(b);
        _expandBtnGroupController.RefreshTweenPos(!b);
        _expandBtnGroupController.ShowOrHideBtn(b);
        _view.LeftShrinkBtn_TweenPosition.Play(b || !showState.topBtnPanelShow);
        _view.LeftShrinkBtn_UIButton.sprite.flip = b || !showState.topBtnPanelShow ?
            UIBasicSprite.Flip.Vertically : UIBasicSprite.Flip.Horizontally;

        _view.SceneBtnGroup_TweenPosition.Play(b);
        expandCtrl.HideGameObject(!b);
        expandCtrl.UpdateExpandContentState(showState.expandPanelShow, !b);
    }

    public void UpdateBuffShow(IEnumerable<int> buffList)
    {
        _view.BuffGroup.gameObject.SetActive(true);
        _buffList.ForEach(item => item.gameObject.SetActive(false));

        buffList.ForEachI((item, idx) =>
        {
            if (idx < _buffList.Count)
            {
                _buffList[idx].UpdateView();
                _buffList[idx].gameObject.SetActive(true);
            }
            else
            {
                var controller = AddChild<BuffShowItemController, BuffShowItem>(_view.BuffShowGrid_UIGrid.gameObject,
                    BuffShowItem.NAME);
                controller.UpdateView();
            }
        });
        _view.BuffShowGrid_UIGrid.Reposition();
    }

    public void UpdatePlayerInfo()
    {
        var playerDto = ModelManager.Player.GetPlayer().charactor as MainCharactor;
        _view.PlayerIcon_UISprite.spriteName = playerDto == null ? "" : playerDto.headicon;
        _view.PlayerNameLb_UILabel.text = ModelManager.Player.GetPlayerName();
        _view.PlayerLvLb_UILabel.text = ModelManager.Player.GetPlayerLevel().ToString();
        //_view.PowerLb_UILabel.text = ModelManager.Player.
    }

    public void ChangeMainCrew(long id)
    {
        if (id <= 0)
        {
            UpdateCrewInfo();
            return;
        }
        var mainCrew = CrewViewDataMgr.DataMgr.GetCrewListInfo().Find(d => d.id == id);
        if (mainCrew == null)
            mainCrew = TeamFormationDataMgr.DataMgr.GetMainCrew;
        if(mainCrew == null)
            mainCrew = CrewReCruitDataMgr.DataMgr.GetMianCrew();
        UpdateCrewInfo(mainCrew);
    }

    public void UpdateCrewInfo(CrewInfoDto dto = null)
    {
        if (_view == null) return;

        var crewDto = dto;
        _view.CrewLvLb_UILabel.gameObject.SetActive(crewDto != null);
        _view.CrewIcon_UISprite.gameObject.SetActive(crewDto != null);
        _view.AddCrew.SetActive(crewDto == null);

        if (crewDto != null)
        {
            var crew = DataCache.getDtoByCls<GeneralCharactor>(crewDto.crewId) as Crew;
            UIHelper.SetPetIcon(_view.CrewIcon_UISprite, crew.icon);
            _view.CrewLvLb_UILabel.text = crewDto.grade.ToString();
        }
    }

    public void UpdateByLayerChange(UIMode mode, MainUIDataMgr.ShowState showState)
    {
        if (mode == UIMode.STORY || mode == UIMode.LOGIN || mode == UIMode.NULL)
        {
            _view.Hide();
            return;
        }

        _view.Show();
        ChangeGMBtnPos(mode);
        _expandBtnGroupController.ShowOrHideBtn(mode != UIMode.BATTLE);
        expandCtrl.HideGameObject(mode != UIMode.BATTLE);
        UpdatePanelsShowState(showState);
        ShowWeather(mode);

        if (mode != UIMode.GAME)
            View.Button_PlayerInfo_UIButton.gameObject.SetActive(false);

        switch (mode)
        {
            case UIMode.GAME:
                _view.SceneBtnGroup.SetActive(true);
                _view.BottomLeftAnchor.SetActive(true);
                _view.BottomRightAnchor.SetActive(true);
                _view.TL_UpRoll_V2_UIWidget.gameObject.SetActive(true);
                break;
            case UIMode.BATTLE:
                _view.BottomRightAnchor.SetActive(false);
                _view.TL_UpRoll_V2_UIWidget.gameObject.SetActive(false);
                break;
        }

        //_view.Left_Up_BtnTable_UITable.gameObject.SetActive(UIMode.BATTLE != mode);

        if (_playerInfo != null)
            _playerInfo.ChangeMode(mode);
    }

    public void OnPlayerInfoBtnClick(IMainUIData data)
    {
        if (data.SelectedPlayer == null) return;
        var pos = _view.PlayerInfoViewAnchor.TransToWorldPos();
        ProxyMainUI.OpenPlayerInfoView(pos, data.SelectedPlayer);
    }

    //在战斗中隐藏右上角除天气时间之外的东西
    private void ShowWeather(UIMode mode)
    {
        _view.WorldMapGroup.SetActive(mode != UIMode.BATTLE);
        _view.MiniMapBtn_UIButton.gameObject.SetActive(mode != UIMode.BATTLE);
        _view.TL_UpRoll_TweenPosition.gameObject.SetActive(mode != UIMode.BATTLE);
        _view.LeftShrinkBtn_UIButton.gameObject.SetActive(mode != UIMode.BATTLE);
        _view.TL_UpScrollView.SetActive(mode != UIMode.BATTLE);
        _view.WeatherBtn_UIButton.gameObject.SetActive(true);
        _view.DeviceGroup_Transform.gameObject.SetActive(true);
    }

    //战斗中更换指令按钮位置
    private void ChangeGMBtnPos(UIMode mode)
    {
        _view.Button_GmTest_UIButton.sprite.leftAnchor.absolute = mode == UIMode.BATTLE ? 101 : 1;
        _view.Button_GmTest_UIButton.sprite.rightAnchor.absolute = mode == UIMode.BATTLE ? 153 : 53;

        _view.Button_ModelTest_UIButton.sprite.leftAnchor.absolute = mode == UIMode.BATTLE ? 101 : 1;
        _view.Button_ModelTest_UIButton.sprite.rightAnchor.absolute = mode == UIMode.BATTLE ? 153 : 53;

        _view.Button_GmTest_UIButton.sprite.UpdateAnchors();
        _view.Button_ModelTest_UIButton.sprite.UpdateAnchors();
        //    _view.Button_GmTest_UIButton.transform.position = mode == UIMode.BATTLE
        //        ? new Vector3(-471, 5, 0) : new Vector3(-542, 5, 0);
        //    _view.Button_ModelTest_UIButton.transform.position = mode == UIMode.BATTLE
        //        ? new Vector3(-471, -66, 0) : new Vector3(-542, -66, 0);
    }

    private void OnSystemWeatherChange(bool night)
    {
        _view.WeatherBtn_UIButton.normalSprite = night ? "Moon" : "Sun";
        _view.WeatherBgSprite_UISprite.spriteName = night ? "nightBg" : "dayBg";
    }

    private void OnSystemTimeChange(long time)
    {
        string timeStr = DateUtil.UnixTimeStampToDateTime(time).ToString("HH:mm");
        _view.DeviceTimeLabel_UILabel.text = timeStr;
    }
    
    private void InitDeviceStatusTimer()
    {
        JSTimer.Instance.SetupTimer(DeviceStatusTimer, delegate
        {
            OnPowerChange();
            OnNetworkChange();
        }, 2f);
    }
  
    private void InitRightBtnGroup()
    {
        _expandBtnGroupController = AddController<RightExpandBtnGroupController, RightExpandBtnGroup>(_view.RightButtonGroup);
    }

    private void OnPowerChange()
    {
        var isCharging = BaoyugameSdk.IsBattleCharging();
            
        _view.ChargingSprite_UISprite.enabled = isCharging;
        _view.PowerBarSprite_UISprite.enabled = !isCharging;
            
        if (isCharging) return;
            
        var power = GameSetting.IsOriginWinPlatform ? 100 : BaoyugameSdk.GetBatteryLevel();
        _view.PowerBarSprite_UISprite.width = 20 * power / 100;    
    }
        
    private void OnNetworkChange()
    {
        var signal = DeviceHelper.GetWifiSignel();
        UpdateWifi(signal);
        _view.exchangeSprite_UISprite.enabled = Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
    }
    
    #region interface implement
    public void ShowTempBagBtn(bool show)
    {
        _view.TempBtn_UIButton.gameObject.SetActive(show);
    }

    public Vector3 BagBtnPos {
        get
        {
            return _view.Button_Pack_UIButton.transform.parent.transform.TransformPoint(_view.Button_Pack_UIButton.transform.localPosition);
        }
    }

    public void UpdateWifi(int signal)
    {
        var index = signal - 1;
        wifi.ForEachI(
            (sprite, idx) =>
            {
                sprite.enabled = idx <= index;
            }
        );
    }
    #endregion

    private void OnCameraClick(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (_view.BuffGroup.activeSelf &&
            panel != _view.BuffGroup_UIPanel &&
            panel != _view.BuffScrollView_UIScrollView.panel)
        {
            _view.BuffGroup.SetActive(false);
        }
    }


    #region
    private Action _useMissionPropsCallback = null;
    private bool _usePopupState = false;
    private AppMissionItem mS3MissionItem = null;
    #endregion
    #region 任务使用物品弹窗
    public void OnClickPopupUseMissionPropsBtn(AppMissionItem missionItem = null,Action callback = null,bool callbackState = false)
    {
        //GameDebuger.LogError("任务—使用物品弹窗");
        if(callback != null)
        {
            _useMissionPropsCallback = callback;
        }
        _usePopupState = callback != null;
        mS3MissionItem = missionItem;
        _view.MissionUseProps_TweenPosition.SetOnFinished(() =>
        {
            if(!_usePopupState) {
                View.ExpandPanel_UIPanel.enabled = false;
            }
        });

        if(_usePopupState && mS3MissionItem != null)
        {
            View.ExpandPanel_UIPanel.enabled = true;
            View.MissionUseProps_TweenPosition.PlayForward();
            SetUsePopsData(mS3MissionItem.icon,mS3MissionItem.name);
        }
        else
        {
            View.ExpandPanel_UIPanel.enabled = false;
            View.MissionUseProps_TweenPosition.PlayReverse();

            if (callbackState || _useMissionPropsCallback == null ) return;
            
            _useMissionPropsCallback();
            _useMissionPropsCallback = null;
        }
    }

    private void SetUsePopsData(string icon,string label)
    {
        string[] icons = icon.Split(':');
        switch(Int32.Parse(icons[0]))
        {
            case 1:
                UIHelper.SetItemIcon(View.IconSprite_UISprite,icons[1]);
                break;
            case 2:
                UIHelper.SetPetIcon(View.IconSprite_UISprite,icons[1]);
                break;
            case 3:
                UIHelper.SetSkillIcon(View.IconSprite_UISprite,icons[1]);
                break;
            case 4:
                UIHelper.SetOtherIcon(View.IconSprite_UISprite,icons[1]);
                break;
        }

        View.MissionUseItemName_UILabel.text = label;
    }

    /// <summary>
    /// 使用物品的按钮方法
    /// </summary>
    public void OnUsePropsBtnClick()
    {
        OnClickPopupUseMissionPropsBtn(null,null,false);
    }


    /// <summary>
    /// 隐藏使用物品的界面
    /// </summary>
    public void OnClickPopupUseMissionPropsClose()
    {
        OnClickPopupUseMissionPropsBtn(null,null,true);
    }
    #endregion

    public void SetActivityPollGo(bool b)
    {
        View.ActivityPoll_GO.SetActive(b);
    }

    #region 四轮之塔轮数  UI
    public void UpdateTowerGuildUI(ITowerData data)
    {
        var showTower = data.ShowTowerGuildUI();
        View.ActivityPoll_GO.SetActive(showTower);
        if(!showTower)
            return;
        else
        {
            View.ActivityName_UILabel.text = data.CurTowerName;
            View.ActivityTime_UILabel.text = data.LeftMonster;
        }
    }
    #endregion

    #region 武术大会
    public void UpdateKungFuInfo(IMartialData data)
    {
        if (data.ActivityInfo == null)
        {
            View.ActivityPoll_GO.SetActive(false);
            return;
        }

        var time = data.EndAtTime;
        //GameDebuger.Log("------------MainUITime----------" + time + "---state---" + data.ActivityInfo.state);
        UpdateKungFuTime(time);
        switch (data.ActivityInfo.state)
        {
            case (int)KungfuActivityInfo.KungfuStateEnum.Normal:
            case (int)KungfuActivityInfo.KungfuStateEnum.Ready:
                _view.ActivityName_UILabel.text = "准备开始";
                break;
            case (int)KungfuActivityInfo.KungfuStateEnum.Racing:
                _view.ActivityName_UILabel.text = "活动开始";
                break;
            case (int)KungfuActivityInfo.KungfuStateEnum.End:
                _view.ActivityName_UILabel.text = "活动结束";
                break;
        }
    }

    private void UpdateKungFuTime(long time)
    {
        JSTimer.Instance.SetupCoolDown("IMartialData", time, e =>
        {
            time -= 1;
            string txt = DateUtil.FormatSeconds(time);
            _view.ActivityTime_UILabel.text = txt;
        }, () =>
        {
            _view.ActivityName_UILabel.text = "准备开始";
            _view.ActivityTime_UILabel.text = "00:00:00";
            JSTimer.Instance.CancelCd("IMartialData");
        }, 1f);
    }
    #endregion

    public ICharacterPropertyController PlayerInfo {
        get { return _playerInfo; }
    }

    public IRightExpandBtnGroupController ExpandBtnGroupCtrl {
        get { return _expandBtnGroupController; }
    }

    public IMainUIExpandContentViewController ExpandCtrl {
        get { return expandCtrl; }
    }

    public GameObject WeatherBtn_GO {
        get { return _view.WeatherBtn_UIButton.gameObject; }
    }

    public ChatBoxController ChatBoxCtrl
    {
        get { return _chatBoxCtrl; }
    }
}