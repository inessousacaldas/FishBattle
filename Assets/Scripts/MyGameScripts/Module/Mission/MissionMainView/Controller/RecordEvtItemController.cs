// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RecordEvtItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System.Collections.Generic;

public partial class RecordEvtItemController
{
    private MissionRecord missionRecord;
    private bool isEvtOpen = false;
    public int idx = -1;
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

    //1.任务是否在身上 2.是否已提交 3.是否可接取
    private bool IsAllEvtOpen(List<int> list, IMissionData data)
    {
        List<int> submit = data.GetPreMissionIDList();
        var bodyMis = data.GetMissionListDto.ToList();
        var canAcces = data.GetMissedSubMissionMenuList();
        int bodyMisCount = bodyMis.Count;
        for (int i = 0; i < bodyMisCount; i++)
        {
            if (list.Contains(bodyMis[i].missionId))
            {
                return true;
            }
        }
        for (int j = 0, m = list.Count; j < m; j++)
        {
            if (submit.Contains(list[j]))
            {
                return true;
            }
        }
        for (int i = 0, max = canAcces.Count; i < max; i++)
        {
            if (list.Contains(canAcces[i].id))
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateAreaEvtView(MissionRecord missionRecord,int i = -1,IMissionData data = null)
    {
        if (data == null) return;
        if (i != -1) idx = i;
        isEvtOpen = IsAllEvtOpen(missionRecord.missionList, data);
        if (!IsEvtOpen)
        {
            Hide();
            return;
        }
        else Show();
        this.missionRecord = missionRecord;
        View.lbltype_UILabel.text = isEvtOpen ? missionRecord.name : "暂无记录";
    }

    public void UpdateDepth(int i)
    {
        View.RecordEvtItem_UISprite.depth = i;
    }

    public void UpdateSprite(bool show)
    {
        View.RecordEvtItem_UISprite.spriteName = show ? "Tab_2_On" : "Tab_2_Off";
        View.lbltype_UILabel.color = show ? ColorConstantV3.Color_MissionBlack : ColorConstantV3.Color_MissionGray;
    }
    public MissionRecord MissionRecord { get { return missionRecord; } }
    public bool IsEvtOpen { get { return isEvtOpen; } }
}
