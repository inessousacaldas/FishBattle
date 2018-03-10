// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TreasurePanelController.cs
// Author   : DM-PC092
// Created  : 12/13/2017 4:15:20 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using AppDto;
using UnityEngine;

public partial interface ITreasurePanelController
{

}
public partial class TreasurePanelController    {
    private List<Transform> mRewardPonit = new List<Transform>();
    private int mCurIndex,mMovePoint,mRewardTarget,mFinalReward;
    List<PropsTreasureReward> mPropsTreasureReward = new List<PropsTreasureReward>();
    List<Transform> mFinalRewardArr = new List<Transform>();
    List<Vector3> mTurntablePosList = new List<Vector3>();
    int[] mMakeAngle = new int[] { 0,-30,-60,-90,-120,-150,180,150,120,90,60,30};
    private GeneralItem mGeneralItem;
    private int mGeneralItemNumber;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        View.RewardNameLabel1_UILabel.text = mPropsTreasureReward[19].name;
        View.RewardNameLabel2_UILabel.text = mPropsTreasureReward[20].name;
        View.RewardNameLabel3_UILabel.text = mPropsTreasureReward[21].name;
        View.Reward1_Transform.GetComponent<UISprite>().spriteName = "crew_ib_" + mPropsTreasureReward[19].quality + "_r";
        View.Reward2_Transform.GetComponent<UISprite>().spriteName = "crew_ib_" + mPropsTreasureReward[20].quality + "_r";
        View.Reward3_Transform.GetComponent<UISprite>().spriteName = "crew_ib_" + mPropsTreasureReward[21].quality + "_r";
        UIHelper.SetItemIcon(View.RewardIconl_UISprite,mPropsTreasureReward[19].icon);
        UIHelper.SetItemIcon(View.RewardIcon2_UISprite,mPropsTreasureReward[20].icon);
        UIHelper.SetItemIcon(View.RewardIcon3_UISprite,mPropsTreasureReward[21].icon);
        EventDelegate.Set(View.RewardIconl_UISprite.GetComponent<UIButton>().onClick,() => { OnClickShowTip("Icon_20"); });
        EventDelegate.Set(View.RewardIcon2_UISprite.GetComponent<UIButton>().onClick,() => { OnClickShowTip("Icon_21"); });
        EventDelegate.Set(View.RewardIcon3_UISprite.GetComponent<UIButton>().onClick,() => { OnClickShowTip("Icon_22"); });
        mFinalRewardArr.Add(View.Reward1_Transform);
        mFinalRewardArr.Add(View.Reward2_Transform);
        mFinalRewardArr.Add(View.Reward3_Transform);
        for(int i = 1;i< mRewardPonit.Count;i++)
        {
            UIHelper.SetItemIcon(mRewardPonit[i].Find("Icon_" + i).GetComponent<UISprite>(),mPropsTreasureReward[i -1].icon);
            UIButton iconBtn = mRewardPonit[i].Find("Icon_"+i).GetComponent<UIButton>();
            EventDelegate.Set(iconBtn.onClick,() => { OnClickShowTip(iconBtn.name); });
            mRewardPonit[i].GetComponent<UISprite>().spriteName = "crew_ib_"+ mPropsTreasureReward[i].quality+"_r";
        }
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(_view.ReturnBtn_Button.onClick,OnClickHandler);
        EventDelegate.Add(_view.BGCollider_UIButton.onClick,OnClickBG);
        EventDelegate.Add(_view.InfoTipsButton_UIButton.onClick,OnInfoTipClick);
    }

    protected override void RemoveCustomEvent ()
    {
        
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
        if(mGeneralItem != null)
            TipManager.AddTip("恭喜获得" + mGeneralItem.name);
        mGeneralItem = null;
        mGeneralItemNumber = 0;
        JSTimer.Instance.CancelTimer("FinalReward");
        JSTimer.Instance.CancelTimer("MoveSelect");
        JSTimer.Instance.CancelTimer("Turntable");
        View.TurntableSlect_Transorm.localPosition = mFinalRewardArr[0].localPosition;
        View.Select_Transform.localPosition = mRewardPonit[0].localPosition;
        TreasureMissionDataMgr.TreasureMissionNetMsg.CloseTreasury();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
        TreasureMissionDataMgr.TreasureMissionNetMsg.InitTreasureMission();
        mPropsTreasureReward = DataCache.getArrayByCls<PropsTreasureReward>();
        for(var i = 0;i < View.RewardArr_Transform.childCount;i++)
        {
            var tran = View.RewardArr_Transform.GetChild(i);
            if(tran.transform.name == "RewardBG")
            {
                mRewardPonit.Add(tran);
            }
        }
        mTurntablePosList.Add(new Vector3(40,99,0));
        mTurntablePosList.Add(new Vector3(88,65,0));
        mTurntablePosList.Add(new Vector3(113,12,0));
        mTurntablePosList.Add(new Vector3(106,-46,0));
        mTurntablePosList.Add(new Vector3(73,-94,0));
        mTurntablePosList.Add(new Vector3(21,-119,0));
        mTurntablePosList.Add(new Vector3(-39,-114,0));
        mTurntablePosList.Add(new Vector3(-85,-80,0));
        mTurntablePosList.Add(new Vector3(-111,-28,0));
        mTurntablePosList.Add(new Vector3(-107,31,0));
        mTurntablePosList.Add(new Vector3(-73,78,0));
        mTurntablePosList.Add(new Vector3(-20,103,0));
    }


    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(ITreasureMissionData data){

        int limit= DataCache.getDtoByCls<DailyLimit>((int)DailyLimit.DailyFuncion.DAILY_15).limit;
        View.TreasureLabel_UILabel.text = "寻宝次数"+ data.GetTreasureNumber() + "/" + DataCache.GetStaticConfigValue(limit).ToString();
        ServerType tServerType = data.GetServerType();
        switch(tServerType) {
            case ServerType.Init:
                InitView(data.GetCurIndex(),data.GetDiamondsNumber(),data.GetItemNumber());
                View.Select_Transform.localPosition = mRewardPonit[data.GetCurIndex()].localPosition;
                break;
            case ServerType.UpDate:
                InitView(mCurIndex,data.GetDiamondsNumber(),data.GetItemNumber());
                mMovePoint = data.GetMovePoint();
                mRewardTarget = data.GetPropsTreasureRewardId();
                mGeneralItem = data.GetGeneralItem();
                mGeneralItemNumber = data.GetGeneralItemNumber;
                View.TreasureBtn_UIButton.isEnabled = false;
                View.InfoTipsButton_UIButton.isEnabled = false;
                Turntable(mMovePoint);
                break;
        }
        View.DiamondsNumber_UILabel.text = data.GetDiamondsNumber().ToString();
    }



    public void InitView(int tCurIndex,int tDiamondsNumber,string costData) {
        //View.DiamondsNumber_UILabel.text = tDiamondsNumber.ToString();
        mCurIndex = tCurIndex;
        View.Label_UILabel.text = costData;
    }

    public void MoveSelect()
    {
        if(mMovePoint > 0 && mCurIndex <= 19)
        {
            mCurIndex++;
            mMovePoint--;
            if(mCurIndex <= 19)
                View.Select_Transform.localPosition = mRewardPonit[mCurIndex].localPosition;
        }
        else
        {
            if(mCurIndex > 19)
            {
                View.Select_Transform.localPosition = mRewardPonit[0].localPosition;
                mCurIndex = 0;
                View.FinalRewardPanel_GameObject.SetActive(true);
                View.NormalLayer_GameObject.SetActive(false);
                mFinalReward = 0;
                JSTimer.Instance.SetupTimer("FinalReward",FinalRewardMove,0.5f);
            }
            else {
                if(mGeneralItem != null && mGeneralItemNumber > 0)
                {
                    var appitem = mGeneralItem as AppItem;
                    var virtualitem = mGeneralItem as AppVirtualItem;
                    int quality = 0;
                    if(appitem != null) quality = appitem.quality;
                    else if(virtualitem != null) quality = virtualitem.quality;
                    TipManager.AddTip(string.Format("恭喜获得{0}*{1}",mGeneralItem.name.WrapColor(ItemHelper.GetItemNameColorByRank(quality)),mGeneralItemNumber));
                    //TipManager.AddColorTip(string.Format("恭喜获得{0}*{1}",mGeneralItem.name,mGeneralItemNumber),);
                    mGeneralItem = null;
                    mGeneralItemNumber = 0;
                }
            }
            View.TreasureBtn_UIButton.isEnabled = true;
            View.InfoTipsButton_UIButton.isEnabled = true;
            JSTimer.Instance.CancelTimer("TreasureSlectCallBackMove");
        }
    }

    void FinalRewardMove() {
        //因为mFinalReward从零开始，所以需要+1 求余3
        View.TurntableSlect_Transorm.localPosition = mFinalRewardArr[(mFinalReward+1) % 3].localPosition;
        mFinalReward++;
        if(mFinalReward >= (mRewardTarget - 20 + 3)) {
            JSTimer.Instance.CancelTimer("FinalReward");
            if(mGeneralItem != null && mGeneralItemNumber > 0) {
                var appitem = mGeneralItem as AppItem;
                var virtualitem = mGeneralItem as AppVirtualItem;
                int quality = 0;
                if(appitem != null) quality = appitem.quality;
                else if(virtualitem != null) quality = virtualitem.quality;
                TipManager.AddTip(string.Format("恭喜获得{0}*{1}",mGeneralItem.name.WrapColor(ItemHelper.GetItemNameColorByRank(quality)),mGeneralItemNumber));
                mGeneralItem = null;
                mGeneralItemNumber = 0;
                //TipManager.AddColorTip(string.Format("恭喜获得{0}*{1}",mGeneralItem.name,mGeneralItemNumber),);
            }
        }
    }


    public void OnClickHandler() {
        OnClickBG();
        View.FinalRewardPanel_GameObject.SetActive(false);
        View.NormalLayer_GameObject.SetActive(true);
        View.TurntableSlect_Transorm.localPosition = mFinalRewardArr[0].localPosition;
    }


    public void Turntable(int RoundNumber) {
        OnClickBG();
        float roundNumber = UnityEngine.Random.Range(0,2);
        //int cueAngle = 0;
        float textAngle = 0;
        float curTime = Time.time;
        View.TurntablePanel_GameObject.SetActive(true);
        JSTimer.Instance.SetupTimer("Turntable",() =>
        {
            float passTime = Time.time - curTime;
            textAngle = Mathf.Lerp(textAngle,(-720 - ((RoundNumber - 1) * 30 + 15) - 180 * roundNumber),passTime*0.03f);
            View.TurntableIcon_Transform.localEulerAngles = new Vector3(0,0,textAngle);
            if(Mathf.Abs(textAngle - (-720 - ((RoundNumber-1) * 30 + 15) - 180 * roundNumber)) < 1) {
                JSTimer.Instance.CancelTimer("Turntable");
                View.TurntablePanel_GameObject.SetActive(false);
                JSTimer.Instance.SetupTimer("TreasureSlectCallBackMove",MoveSelect,0.5f);
            }
            View.TurntableMask_Transform.localPosition= mTurntablePosList[(int)Mathf.Abs(textAngle / 30 % 12)];
            View.TurntableMask_Transform.localEulerAngles = new Vector3(0,0,mMakeAngle[(int)Mathf.Abs(textAngle / 30 % 12)]);
            //cueAngle = cueAngle - 5;
        },0.03f);
    }

    public void OnClickShowTip(string IconName) {
        int index =StringHelper.ToInt(IconName.Split('_')[1]);
        View.TipPanel_UILabel.text = mPropsTreasureReward[index - 1].desc;
        View.TipPanel_UILabel.transform.Find("TipBG").GetComponent<UISprite>().ResetAnchors();
        int width =  View.TipPanel_UILabel.width/2 + 40;
        if(!View.TipPanel_UILabel.gameObject.activeSelf) {
            View.TipPanel_UILabel.gameObject.SetActive(true);
        }
        if(index - 1 < 19)
        {
            View.TipPanel_UILabel.transform.localPosition = mRewardPonit[index].localPosition + new Vector3(width,0,0);
        }
        else {
            View.TipPanel_UILabel.transform.localPosition = mFinalRewardArr[index - 20].localPosition + new Vector3(width,0,0);
        }
    }

    public void OnClickBG() {
        if(View.TipPanel_UILabel.gameObject.activeSelf) {
            View.TipPanel_UILabel.gameObject.SetActive(false);
        }
    }

    public void OnInfoTipClick() {
        OnClickBG();
        int limit= DataCache.getDtoByCls<DailyLimit>((int)DailyLimit.DailyFuncion.DAILY_15).limit;
        ProxyTips.OpenTextTips(20,new UnityEngine.Vector3(80,-135),true,limit.ToString());
    }
}
