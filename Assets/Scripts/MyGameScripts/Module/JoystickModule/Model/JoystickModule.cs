// **********************************************************************
// Copyright (c) 2013 Baoyugame. All rights reserved.
// File     :  JoystickModule.cs
// Author   : willson
// Created  : 2014/12/4 
// Porpuse  : 
// **********************************************************************

using AssetPipeline;
using UnityEngine;
using UniRx;
using AppDto;
using System;

public interface IJoystickModuleData
{
    ScenePlayerDto SelectedPlayerDto{ get;} 
}

namespace StaticInit
{
    public partial class StaticInit
    {
        private StaticDispose.StaticDelegateRunner disposeJoystickModule = new StaticDispose.StaticDelegateRunner(
            ()=> { var mgr = JoystickModule.Instance; } );
    }
}
public sealed partial class JoystickModule
{

    private JoystickModuleData _data = null;
    public static UniRx.IObservable<IJoystickModuleData> Stream{get{return stream;}}
    private static Subject<IJoystickModuleData> stream;

    private const string NAME = "JoystickModule";
    private static JoystickModule instance;
    public static bool DisableMove{get{ return _disableMove;}}
    private static bool _disableMove = false;
    private JoystickController _joystickController;
    private GameObject _joystickView;
    private IDisposable _disposable = null;

    private JoystickModule()
    {
        _data = new JoystickModuleData();
    }

    public static JoystickModule Instance
    {
        get { 
            if (instance == null)
                instance = new JoystickModule();
            if (stream == null)
                stream = new Subject<IJoystickModuleData>();
            return instance; 
        }
    }

    public bool IsDragging
    {
        get
        {
            if (_joystickController != null)
            {
                return _joystickController.IsDragging;
            }
            return false;
        }
    }

    public void Setup()
    {
        _disposable = TeamDataMgr.Stream.Subscribe(s=>
            {
                if (s == null)
                    return;
                var m = s.GetTeamMemberDtoByPlayerID(ModelManager.IPlayer.GetPlayerId());
                _disableMove = m != null ? m.memberStatus == (int)TeamMemberDto.TeamMemberStatus.Member : false;
            }
        );

        if (_joystickView == null)
        {
            var modulePrefab = ResourcePoolManager.Instance.LoadUI(NAME) as GameObject;
            _joystickView = NGUITools.AddChild(LayerManager.Root.UIModuleRoot, modulePrefab);

            var panel = _joystickView.GetComponent<UIPanel>();
            if (panel != null)
            {
                var depth = UIModuleManager.Instance.GetCurDepthByLayerType(UILayerType.JOYSTICK);    
                panel.depth = depth;
            }

            _joystickView.name = "JoystickModule";

        }
        else
            _joystickView.SetActive(true);

        _joystickController = _joystickView.GetMissingComponent<JoystickController>();

        if(_data == null)
            _data = new JoystickModuleData();
    }

    //public void SetActive(bool active)
    //{
    //    if (_joystickView)
    //        _joystickView.SetActive(active);
    //}

    public void EnabledJoystick(bool enabled)
    {
        if (_joystickController != null)
        {
            _joystickController.SetCollider(enabled);
        }
    }

    #region 任务影响的玩家移动控制的处理
    private bool _isControl;

    public bool IsControl {
        get { return _isControl; }
    }

    /// <summary>
    /// 这个是用来标志玩家是否是在可控制状态,如果玩家在可控制状态下,就能触发任务的采集,使用物品
    /// True 能触发 False 不能触发
    /// 不可控状态:
    ///  1.挂机中
    ///  2.藏宝图中
    ///  3.任务寻路状态中(例如收集灵气任务, 如果触发条件是True, 因为触发是有范围的, 玩家走到点附近就能触发收集, 所以设置为False, 等待玩家走到某个点回调)
    /// 简单来说,True 就是在玩家在地图上点点点,控制玩家移动的时候,走到采集区域,触发采集逻辑
    /// </summary>
    public void CanControlByPlayer(bool isControl) {
        _isControl = isControl;
        if(_isControl) {
            MissionDataMgr.DataMgr.ClearLastFindMission();
        }   
    }
    #endregion

    private void UpdateSelectPlayerInfo(ScenePlayerDto dto){
        //if (dto == null) return;
        _data._selectedPlayerDto = dto;
        stream.OnNext(_data);
    }

    private void OnDispose()
    { 
        GameDebuger.LogError("joy controller dispose");
        if (_joystickView != null)
        {
            _joystickView.SetActive(false);
            _joystickView.RemoveComponent<JoystickController>();
        }
        if (_joystickController != null)
        {
            _joystickController.enabled = false;
            _joystickController = null;
        }
        if (_data != null)
            _data.Dispose();
        _data = null;
        if (_disposable != null)
        {
            _disposable.Dispose();
            _disposable = null;
        }
    }

    public static void Dispose(){
        if (instance != null)
            instance.OnDispose();
        if (stream != null)
            stream = stream.CloseOnceNull();
    }
    #region

    public sealed partial class JoystickModuleData:IJoystickModuleData
    {
        public ScenePlayerDto _selectedPlayerDto = null;
        public ScenePlayerDto SelectedPlayerDto{get{ return _selectedPlayerDto;}}
        public JoystickModuleData()
        {

        }

        public void Dispose()
        {

        }
    }

    #endregion
}