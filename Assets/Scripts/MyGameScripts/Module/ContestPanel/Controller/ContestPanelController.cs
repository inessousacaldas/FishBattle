// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  ContestPanelController.cs
// Author   : DM-PC092
// Created  : 12/17/2017 2:57:47 PM
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial interface IContestPanelController
{

}
public partial class ContestPanelController    {
    private int[] mSkillArr ={0,0,0,0};
    private BagItemDto mBagItemDto;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
            
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(_view.SkillIcon_1_UIButton.onClick,OnDeleGateClickHandler1);
        EventDelegate.Add(_view.SkillIcon_2_UIButton.onClick,OnDeleGateClickHandler2);
        EventDelegate.Add(_view.SkillIcon_3_UIButton.onClick,OnDeleGateClickHandler3);
        EventDelegate.Add(_view.SkillIcon_4_UIButton.onClick,OnDeleGateClickHandler4);
        EventDelegate.Add(_view.CancelBtn_Button.onClick,Close);
        EventDelegate.Add(_view.StartBtn_Button.onClick,StartContest);
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IContestData data) {
        if(data.GetServerType() == ServerType.Init) {
            mBagItemDto = data.GetItem();
            ViewInit(data.MianCrew());
        }
    }


    private void ViewInit(Crew tCrewData) {
        if(tCrewData != null)
        {
            View.PlayerNanmeLabel_UILabel.text = "致：" + ModelManager.Player.GetPlayerName();
            View.NpcNanmeLabel_UILabel.text = "--" + tCrewData.name;
            View.NpcBodyNameLabel_UILabel.text = tCrewData.name;
            UIHelper.SetUITexture(View.NPCBody_UITexture,"texture_" + tCrewData.texture +"_1");
            switch(tCrewData.property)
            {
                case 0:
                    //末知
                    break;
                case 1:
                    //力量
                    View.PloygonMesh_UISprite.spriteName = "AstralFigure_strength";
                    break;
                case 2:
                    //魔法
                    View.PloygonMesh_UISprite.spriteName = "AstralFigure_magic";
                    break;
                case 3:
                    //控制
                    View.PloygonMesh_UISprite.spriteName = "AstralFigure_control";
                    break;
                case 4:
                    //辅助
                    View.PloygonMesh_UISprite.spriteName = "AstralFigure_ancillary";
                    break;
            }
            if(tCrewData != null)
            {
                string[] mSkillIconArr ={"","","",""};
                Dictionary<int,Skill> tCraftsSkillDic = DataCache.getDicByCls<Skill>();
                for(int i = 0;i < tCrewData.crafts.Count;i++)
                {
                    if(tCraftsSkillDic[tCrewData.crafts[i]] != null)
                    {
                        mSkillIconArr[i] = tCraftsSkillDic[tCrewData.crafts[i]].icon;
                        mSkillArr[i] = tCrewData.crafts[i];
                    }
                }
                if(mSkillIconArr[0] != "") {
                    UIHelper.SetUITexture(View.SkillIcon_1_UISprite,mSkillIconArr[0],true);
                }
                if(mSkillIconArr[1] != "")
                {
                    UIHelper.SetUITexture(View.SkillIcon_2_UISprite,mSkillIconArr[1],true);
                }
                if(mSkillIconArr[2] != "")
                {
                    UIHelper.SetUITexture(View.SkillIcon_3_UISprite,mSkillIconArr[2],true);
                }
                if(mSkillIconArr[3] != "")
                {
                    UIHelper.SetUITexture(View.SkillIcon_4_UISprite,mSkillIconArr[3],true);
                }
            }
        }
        else {
            TipManager.AddTip("连接服务器失败");
        }
    }

    private void OnDeleGateClickHandler1()
    {
        if(mSkillArr[0] != 0)
        {
            SkillTipsController tip = ProxyTips.OpenSkillTips(mSkillArr[0]);
            Vector3 tPos = View.SkillIcon_1_UIButton.transform.parent.localPosition + new Vector3(-422,25,0);
            tip.SetTipsPosition(tPos);
        }
    }

    private void OnDeleGateClickHandler2()
    {
        if(mSkillArr[1] != 0)
        {
            SkillTipsController tip = ProxyTips.OpenSkillTips(mSkillArr[1]);
            Vector3 tPos = View.SkillIcon_2_UIButton.transform.parent.localPosition + new Vector3(-422,25,0);
            tip.SetTipsPosition(tPos);
        }
    }
    private void OnDeleGateClickHandler3()
    {
        if(mSkillArr[2] != 0)
        {
            SkillTipsController tip = ProxyTips.OpenSkillTips(mSkillArr[2]);
            Vector3 tPos = View.SkillIcon_3_UIButton.transform.parent.localPosition + new Vector3(-422,25,0);
            tip.SetTipsPosition(tPos);
        }
    }

    private void OnDeleGateClickHandler4()
    {
        if(mSkillArr[3] != 0)
        {
            SkillTipsController tip = ProxyTips.OpenSkillTips(mSkillArr[3]);
            Vector3 tPos = View.SkillIcon_4_UIButton.transform.parent.localPosition + new Vector3(-422,25,0);
            tip.SetTipsPosition(tPos);
        }
    }

    private void Close() {
        ProxyContest.Close();
    }

    private void StartContest() {
        if(mBagItemDto != null) {
            ContestDataMgr.ContestNetMsg.BackpackApply(mBagItemDto.index,1);
        }
    }
}
