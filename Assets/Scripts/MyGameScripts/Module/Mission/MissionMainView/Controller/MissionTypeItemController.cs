// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MissionTypeItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using AppDto;
using UniRx;

public partial interface IMissionTypeItemController
{
    MissionType MissionType { get; }
    UIGrid Tween_UIGrid { get; }
    UnityEngine.Transform PoolParent { get; }
    int Index { get; }
    List<SubMissionItemController> SubItemList { get; }
    MissionTypeItemController TypeItem { get; }
}

public partial class MissionTypeItemController
{
    private MissionType missionType;
    private CompositeDisposable _disposable;
    private int index;
    private List<SubMissionItemController> subItemList = new List<SubMissionItemController>();
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        if (subItemList.Count > 0) subItemList.Clear();
        missionType = null;
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }
    public void SetIndex(int index)
    {
        this.index = index;
    }

    public void UpdateView(MissionType mission, IMissionData data)
    {
        missionType = mission;
        UpdateView(mission);
    }

    public void UpdateClickView(bool select)
    {
        View.bg_UISprite.spriteName = select ? "Opt_1_On" : "Opt_1_Off";
        View.lbltype_UILabel.color = select ? ColorExt.HexStrToColor("ffffff") : ColorExt.HexStrToColor("464646");
        View.lbltype_UILabel.effectStyle = select ? UILabel.Effect.Outline : UILabel.Effect.None;
    }

    private void UpdateView(MissionType mission)
    {
        string name = "";
        MissionType.MissionTypeEnum misType = (MissionType.MissionTypeEnum)mission.id;
        if (misType >= MissionType.MissionTypeEnum.Faction && misType < MissionType.MissionTypeEnum.Copy)
        {
            name = "日常任务";
        }
        else if(misType >= MissionType.MissionTypeEnum.Copy && misType <= MissionType.MissionTypeEnum.CopyExtra)
        {
            name = "副本任务";
        }
        else
        {
            name = mission.name;
        }
        View.lbltype_UILabel.text = name;
    }
    
    public MissionType MissionType { get { return missionType; } }
    public UIGrid Tween_UIGrid { get { return View.tween_UIGrid; } }
    public UnityEngine.Transform PoolParent { get { return View.pool_Transform; } }
    public int Index { get { return index; } }
    public List<SubMissionItemController> SubItemList { get { return subItemList; } }
    public MissionTypeItemController TypeItem { get { return this; } }

}
