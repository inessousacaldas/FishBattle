// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  SubMissionItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using UniRx;
using UnityEngine;
using System.Collections.Generic;
public partial interface ISubMissionItemController
{

}

public partial class SubMissionItemController
{
    private Mission mission;
    private MissionTypeItemController belongItem;
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
        mission = null;
        belongItem = null;
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }
    public void UpdateView(Mission mis, IMissionTypeItemController ctrl, MisViewTab tab,IMissionData data)
    {
        belongItem = ctrl.TypeItem;
        mission = mis;
        if (tab == MisViewTab.Accepted)
            View.lbltype_UILabel.text = mis.name;
        else
        {
            //可接任务不会存在主线，副本彩蛋
            MissionType.MissionTypeEnum misType = (MissionType.MissionTypeEnum)mis.type;
            if (misType >= MissionType.MissionTypeEnum.Faction && misType < MissionType.MissionTypeEnum.Copy)
            {
                View.lbltype_UILabel.text = mis.missionType.name;
            }
            else if(misType == MissionType.MissionTypeEnum.Copy)
            {
                var copyMisConfigList = data.GetCopyMisConfig;
                var copyMisList = data.GetCopyMisList;
                var val1 = copyMisConfigList.Find(e => e.id == mis.id);
                if (val1 != null)
                {
                    var val2 = copyMisList.Find(e => e.id == val1.copyId);
                    if (val2 != null)
                    {
                        View.lbltype_UILabel.text = val2.name;
                    }
                }
            }
            else
            {
                View.lbltype_UILabel.text = mis.name;
            }
        }
    }
    public void UpdateViewByClick(bool select)
    {
        View.bg_UISprite.spriteName = select ? "Opt_2_On" : "Opt_2_Off";//Ect_buzhen_normal
        View.lbltype_UILabel.color = select ? ColorExt.HexStrToColor("ffffff") : ColorExt.HexStrToColor("464646");
        View.lbltype_UILabel.effectStyle = select ? UILabel.Effect.Outline : UILabel.Effect.None; 
    }

    public Mission SubItemMission { get { return mission; } }

    public void SetParent(Transform parent)
    {
        View.transform.parent = parent;
        View.transform.localScale = Vector3.one;
    }
    
    public MissionTypeItemController BelongItem { get { return belongItem; } }
}
