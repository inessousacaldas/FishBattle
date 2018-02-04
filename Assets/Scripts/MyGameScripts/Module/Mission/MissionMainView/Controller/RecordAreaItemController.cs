// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  RecordAreaItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using AppDto;

public partial class RecordAreaItemController
{
    private List<MissionRecord> recordList = new List<MissionRecord>();
    public int id = -1;
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
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateRecordView(List<MissionRecord> record,int idx)
    {
        id = idx;
        View.lbltype_UILabel.text = record[0].area.ToString();
        recordList = record;
    }

    public void UpdateTween(bool show)
    {
        View.Tween_Transform.gameObject.SetActive(show);
    }

    public List<MissionRecord> RecordList { get { return recordList; } }

    public List<MissionRecord> test(IMissionData data)
    {
        recordList.Sort((_, k) => IsAllEvtOpen(k.missionList, data).CompareTo(IsAllEvtOpen(_.missionList, data)));
        return recordList;
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
        for(int i = 0,max = canAcces.Count; i < max; i++)
        {
            if (list.Contains(canAcces[i].id))
            {
                return true;
            }
        }
        return false;
    }

}
