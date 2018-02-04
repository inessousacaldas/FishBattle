// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  MissionUICellController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;
using System.Collections.Generic;

public sealed partial class MainUIDataMgr
{
    public static partial class MainUIViewLogic
    {
        private static void InitMissionCell(IMainUIViewController ctrl)
        {
            if (ctrl == null || ctrl.ExpandCtrl == null) return;
            _disposable.Add(MissionDataMgr.Stream.Subscribe(e =>
            {
                ctrl.ExpandCtrl.UpdateMissionUICell(e.GetMissionCellData());
            }
            ));
            ctrl.ExpandCtrl.UpdateMissionUICell(MissionDataMgr.DataMgr.GetMissionCellData());
        }
    }
}

public partial interface IMissionUICellController
{
    void UpdateView(object obj);
    object Obj{ get; }
    void ShowView();
    void HideView();
}

public partial class MissionUICellController
{
    // 界面初始化完成之后的一些后续初始化工作
    private object mission;
    protected override void AfterInitView ()
    {
        View.BackGround_UISprite.height = 54;
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        if (mission != null) mission = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(object obj)
    {
        if(obj is Mission)
        {
            Mission tMission = obj as Mission;
            mission = tMission;
            gameObject.name = tMission.type.ToString("D2") + "_" + tMission.id.ToString("D6");
            string txt = MissionHelper.GetMissionTitleName(tMission,true);
            View.NameLb_UILable.text = txt.WrapColor(ColorConstantV3.Color_MissionOringe);
            View.DescLb_UILabel.text = MissionHelper.GetCurTargetContent(tMission, null,MisViewTab.None,true);

            SubmitDto tSubmitDto = MissionHelper.GetSubmitDtoByMission(tMission);
            if(tSubmitDto == null || tSubmitDto.finish || tSubmitDto.count >= tSubmitDto.needCount)
            {
                //TODO finish
                View.stateIcon_UISprite.gameObject.SetActive(false);
            }
            else
            {
                //TODO 暗雷
                if (MissionHelper.IsShowMonster(tSubmitDto))
                {
                    //显示“战”
                    View.stateIcon_UISprite.gameObject.SetActive(true);
                }
                else
                {
                    View.stateIcon_UISprite.gameObject.SetActive(false);
                }
            }
        }
        int h = 19;
        int height = View.DescLb_UILabel.height;
        int rate = 2 + (height / h);
        int sub = h * rate;
        float half = sub / 2;
        View.BackGround_UISprite.height = sub;
        UnityEngine.BoxCollider box = View.BackGround_UISprite.GetComponent<UnityEngine.BoxCollider>();
        box.center = new UnityEngine.Vector3(0,-half,0);
        box.size =  new UnityEngine.Vector3(216, sub, 0);
    }

    public object Obj { get { return mission; } }

    public void ShowView()
    {
        Show();
    }

    public void HideView()
    {
        Hide();
    }
}
