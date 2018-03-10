// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RecordDetailItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using System.Collections.Generic;

public partial class RecordDetailItemController
{

    public enum MissionDetailType
    {
        None,
        CanAccept,
        Playing,
        Finished,
        NoAccept
    }

    private string des = "";
    private string reason = "";
    private MissionDetailType type = MissionDetailType.None;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(int id,IMissionData data)
    {
        reason = "";
        bool isMaster = false;//是否是主线
        var dic = data.GetAllMissionDic;
        var submitedList = data.GetPreMissionIDList();
        List<int> bodyList = new List<int>();
        var bodyList_data = data.GetMissionListDto;
        bodyList_data.ForEach(e =>
        {
            bodyList.Add(e.missionId);
        });

        Mission mission = null;
        if (dic.ContainsKey(id))
        {
            mission = dic[id];
            if(mission.type == (int)AppDto.MissionType.MissionTypeEnum.Master)
            {
                isMaster = true;
            }
        }

        if(mission == null)
        {
            GameDebuger.LogError("当前配表是否存在id: " + id + " ");
            return;
        }
        View.State_UIButton.gameObject.SetActive(true);
        View.Finish_Trans.gameObject.SetActive(false);
        if (bodyList.Contains(id))
        {
            type = MissionDetailType.Playing;
            View.lblState_UILabel.text = "进行中";
            View.State_UISprite.spriteName = "Btn_3";
        }
        else if (submitedList.Contains(id))
        {
            type = MissionDetailType.Finished;
            //View.lblState_UILabel.text = "完成";
            View.State_UISprite.gameObject.SetActive(false);
            View.Finish_Trans.gameObject.SetActive(true);
        }
        else
        {
            type = MissionDetailType.NoAccept;
            View.lblState_UILabel.text = "不可接取";
            View.State_UIButton.gameObject.SetActive(false);
            var conditionList = mission.acceptConditions.acceptConditionList;
            for (int i = 0, max = conditionList.Count; i < max; i++)
            {
                var val = conditionList[i] as AcceptionCondtion_1;
                if (val != null)
                {
                    reason = "等级达到" + val.grade + "级，解锁该任务";
                }
            }
        }
        UnityEngine.Color color = ColorConstantV3.Color_White;
        if (isMaster)
        {
            //主线
            reason = "主线只存在一个进行中（后面看需求改文字）";
            color = ColorConstantV3.Color_MissionGreen;
        }
        else
        {
            //支线
            var canAcceptList_data = data.GetMissedSubMissionMenuList();
            color = ColorConstantV3.Color_MissionOringe;
            List<int> canAcceptList = new List<int>();
            canAcceptList_data.ForEach(e =>
            {
                canAcceptList.Add(e.id);
            });
            if (canAcceptList.Contains(id))
            {
                type = MissionDetailType.CanAccept;
                View.lblState_UILabel.text = "接取";
                View.State_UISprite.spriteName = "Btn_4";
                View.State_UIButton.gameObject.SetActive(true);
            }
        }
        View.lblMissName_UILabel.text = type == MissionDetailType.NoAccept ? "???" : MissionHelper.GetMissionTitleName(mission);
        des = MissionHelper.GetMissionTitleName(mission);
        string lblType = "[" + mission.missionType.name + "]";
        View.lblMissType_UILabel.text = lblType.WrapColor(color);
    }

    public void SetParent(UnityEngine.Transform trans)
    {
        trans.parent = View.DesTrans_Trans;
    }

    public MissionDetailType Type { get { return type; } }
    public string Des { get { return des; } }
    public string Reason { get { return reason; } }
}
