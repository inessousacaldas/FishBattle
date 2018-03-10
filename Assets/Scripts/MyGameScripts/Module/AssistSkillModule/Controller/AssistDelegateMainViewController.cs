// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistDelegateMainViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using System.Linq;
using AppDto;
using System.Text;
using UniRx;
using UnityEngine;

public partial interface IAssistDelegateMainViewController
{
    Subject<int> OnDelegateChoseMissionStream { get; }
}

public partial class AssistDelegateMainViewController
{
    private List<UISprite> _crewSpriteList = new List<UISprite>();
    private List<AssistDelegateMissionBtnController> _missionItemCtrlList = new List<AssistDelegateMissionBtnController>();
    private Subject<int> _choseMissionStream = new Subject<int>();
    public Subject<int> OnDelegateChoseMissionStream { get { return _choseMissionStream; } }
    private CompositeDisposable _disposable = new CompositeDisposable();
    private List<ItemCellController> _itemCellCtrlList = new List<ItemCellController>(rewardCount);
    private Dictionary<int, GeneralCharactor> _crewDataDic = DataCache.getDicByCls<GeneralCharactor>();
    private Dictionary<int, DelegateMission> _delegateMissionDic = DataCache.getDicByCls<DelegateMission>();
    private Dictionary<int, DelegateRandomAward> _delegateRandomAwardDic = DataCache.getDicByCls<DelegateRandomAward>();
    private Dictionary<int, DelegateAcceptConditions> _delegateAcceptConditionsDic = DataCache.getDicByCls<DelegateAcceptConditions>();
    private const int missionCount = 5;
    private const int crewCount = 4;
    private const int rewardCount = 5;
    private const int minToHour = 60;
    private const int secToMin = 60;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        UIHelper.SetUITexture(View.Npc_UITexture, "npc_311");
        _crewSpriteList.Add(View.Add_UISprite_1);
        _crewSpriteList.Add(View.Add_UISprite_2);
        _crewSpriteList.Add(View.Add_UISprite_3);
        _crewSpriteList.Add(View.Add_UISprite_4);

        for (int i = 0; i < rewardCount; i++)
        {
            var ctrl = AddChild<ItemCellController, ItemCell>(View.RewardGrid_UIGrid.gameObject, ItemCell.NAME);
            _itemCellCtrlList.Add(ctrl);
            ctrl.SetTipsPosition(new Vector3(-88, 185));
            ctrl.transform.localScale = new Vector3(0.8f,0.8f);
        }
        View.RewardGrid_UIGrid.Reposition();
    }
    
    protected override void OnDispose()
    {
        JSTimer.Instance.CancelCd(TimerName + this.GetHashCode());
        _disposable = _disposable.CloseOnceNull();
    }

    //单位 s
    private void GetCountDownStr(int time, out string str)
    {
        var hour = time / (minToHour * secToMin);
        var leftSec = time % (minToHour * secToMin);
        var min = leftSec / secToMin;
        leftSec = leftSec % secToMin;
        str = string.Format("{0}:{1}:{2}", hour.ToString("D2"), min.ToString("D2"), leftSec.ToString("D2"));
    }

    private static readonly string TimerName = "AssistDelegateMission";

    public void UpdateView(IAssistSkillMainData data)
    {
        var isOpen = data.MissionList != null && data.MissionList.Any();
        
        View.MissionTra.gameObject.SetActive(isOpen);
        View.DetailBg.gameObject.SetActive(isOpen);
        _view.NumBg.SetActive(isOpen);
        View.Npc_UITexture.enabled = !isOpen;
        View.NoMissionTips.SetActive(!isOpen);

        if (!isOpen)
            return;
        
        data.MissionList.GetEnumerator().Reset();
           
        //今天任务数量和今天限量
        View.MissionProgressLabel_UILabel.text = string.Format("{0}/{1}", data.AcceptNum, data.AcceptLimit);

        DelegateMissionDto curMissionMsg = new DelegateMissionDto();

        #region 任务按钮

        _missionItemCtrlList.ForEach(item => { item.Hide(); });
        data.MissionList.ForEachI((itemDto, index) =>
        {
            if (index < missionCount && _delegateMissionDic.ContainsKey(itemDto.id))
            {
                var ctrl = AddMissionItemIfNotExist(index);
                ctrl.UpdateView(itemDto, _delegateMissionDic[itemDto.id], itemDto.id == data.CurMissionId);
                ctrl.Show();
                if (itemDto.id == data.CurMissionId)
                    curMissionMsg = itemDto;              
            }
        });

        #endregion

        var curMissionDto = _delegateMissionDic[data.CurMissionId];

        #region 描述 委托要求

        View.MissionDesLabel_UILabel.text = curMissionDto.content;
        var sb = new StringBuilder();
        curMissionMsg.conditions.ForEach(itemId =>
        {
            if (_delegateAcceptConditionsDic.ContainsKey(itemId))
            {
                sb.Append(_delegateAcceptConditionsDic[itemId].desc);
                sb.Append('，');                              
            }
            //foreach (var item in _delegateAcceptConditionsDic[itemId].param.Split(','))
            //{
            //    item.Split(':');
            //    foreach (var singleItem in item.Split(':'))
            //    {
                    
            //    }
            //}
        });
        sb.Remove(sb.ToString().LastIndexOf('，'), 1);
        
        View.DemandDesLabel_UILabel.text = sb.ToString();
        //字体变色   加判断？
        //View.DemandDesLabel_UILabel.text = string.Format("{0}{1}[-]", "[7EE830FF]", sb.ToString());
        #endregion

        #region 倒计时

        if (curMissionMsg.finishTime <= 0) //未开始
        {
            JSTimer.Instance.CancelCd(TimerName + this.GetHashCode());
            var str = string.Empty;
            var lefttime = curMissionMsg.needTime / 1000;
            GetCountDownStr((int) lefttime, out str);
            View.LeftTime_UILabel.text = str;
            //接受 取消 快速完成 按钮
            View.AcceptBtn_UIButton.gameObject.SetActive(true);
            View.GetRewardBtn_UIButton.gameObject.SetActive(false);
            View.CancelOrFast_Transform.gameObject.SetActive(false);
        }
        else if (curMissionMsg.finishTime < SystemTimeManager.Instance.GetUTCTimeStamp()) //已完成可领取奖励
        {
            JSTimer.Instance.CancelCd(TimerName + this.GetHashCode());
            View.LeftTime_UILabel.text = "00:00:00";
            //接受 取消 快速完成 按钮
            View.AcceptBtn_UIButton.gameObject.SetActive(false);
            View.GetRewardBtn_UIButton.gameObject.SetActive(true);
            View.CancelOrFast_Transform.gameObject.SetActive(false);
        }
        else //进行中
        {
            var str = string.Empty;
            var lefttime = (curMissionMsg.finishTime - SystemTimeManager.Instance.GetUTCTimeStamp()) / 1000;
            GetCountDownStr((int) lefttime, out str);
            View.LeftTime_UILabel.text = str;

            var tDuration = 1.0f;
            //所有任务通用一个计时器
            JSTimer.CdTask tCdTask = JSTimer.Instance.GetCdTask(TimerName + this.GetHashCode());
            JSTimer.CdTask.OnCdFinish tOnCdFinish = () =>
            {
                JSTimer.Instance.CancelCd(TimerName + this.GetHashCode());

                View.LeftTime_UILabel.text = "00:00:00";
                //接受 取消 快速完成 按钮
                View.AcceptBtn_UIButton.gameObject.SetActive(false);
                View.GetRewardBtn_UIButton.gameObject.SetActive(true);
                View.CancelOrFast_Transform.gameObject.SetActive(false);
            };
            JSTimer.CdTask.OnCdUpdate tOnCdUpdate = (pRemain) =>
            {
                GetCountDownStr((int) pRemain, out str);
                View.LeftTime_UILabel.text = str;
            };

            if (null == tCdTask)
            {
                tCdTask = JSTimer.Instance.SetupCoolDown(TimerName + this.GetHashCode(), tDuration, tOnCdUpdate,
                    tOnCdFinish);
                tCdTask.remainTime = lefttime;
            }
            else
            {
                tCdTask.Reset(lefttime, tOnCdUpdate, tOnCdFinish);
            }

            //接受 取消 快速完成 按钮
            View.AcceptBtn_UIButton.gameObject.SetActive(false);
            View.GetRewardBtn_UIButton.gameObject.SetActive(false);
            View.CancelOrFast_Transform.gameObject.SetActive(true);
        }

        #endregion

        #region 奖励

        _itemCellCtrlList.ForEach(itemCtrl => { itemCtrl.Hide(); });
        //生活积分
        var hourTime = curMissionMsg.needTime / 1000 / 60 / 60;
        var expression = curMissionDto.assistIntegralFormula.Replace("needTime", hourTime.ToString());
        expression = expression.Replace("grade", ModelManager.Player.GetPlayerLevel().ToString());
        var assistIntergalCount =
            Mathf.CeilToInt((float) ExpressionManager.DelegateMissionProps(
                string.Format("DelegateMisassistIntegralFormula{0}", curMissionMsg.id), expression));
        _itemCellCtrlList[0].Show();
        _itemCellCtrlList[0]
            .UpdateView(ItemHelper.GetGeneralItemByItemId((int) AppVirtualItem.VirtualItemEnum.ASSISTINTEGRAL),
                assistIntergalCount.ToString());
        if (_delegateRandomAwardDic.ContainsKey(curMissionMsg.randomAwardId))
        {
            _itemCellCtrlList[4].Show();
            _itemCellCtrlList[4]
                .UpdateView(
                    ItemHelper.GetGeneralItemByItemId(_delegateRandomAwardDic[curMissionMsg.randomAwardId].itemId),
                    "概率获得", true);
        }

        var rewardItem = curMissionDto.props.Split(',');
        rewardItem.ForEachI((itemData, index) =>
        {
            if (index < 3)
            {
                var tempStr = itemData.Split(':');
                if (tempStr.Length < 2) return;
                _itemCellCtrlList[index + 1].UpdateView(ItemHelper.GetGeneralItemByItemId(int.Parse(tempStr[0])),
                    tempStr[1]);
                _itemCellCtrlList[index + 1].Show();
            }
        });

        #endregion

        var teamLenth = 0;
        var likeCount = 0;

        #region 选择伙伴或好友

        if (data.ChoseFriendId > 0)
        {
            View.FriendIcon_UISprite.gameObject.SetActive(true);
            View.FriendLabel_UILabel.gameObject.SetActive(false);
            var friendDto = FriendDataMgr.DataMgr.GetFriendDtoById(data.ChoseFriendId);
            if (friendDto == null)
                UIHelper.SetPetIcon(View.FriendIcon_UISprite, "101");
            else
                UIHelper.SetPetIcon(View.FriendIcon_UISprite,
                    (friendDto.charactor as MainCharactor).gender == 1 ? "101" : "103");
            teamLenth = 1;
        }
        else
        {
            View.FriendIcon_UISprite.gameObject.SetActive(false);
            View.FriendLabel_UILabel.gameObject.SetActive(true);
        }
        
        int choseCrewIndex = 0;
        data.ChoseCrewCrewIdList.ForEach(crewId =>
        {
            if (choseCrewIndex < crewCount && _crewDataDic.ContainsKey(crewId))
            {
                var crewData = _crewDataDic[crewId] as Crew;
                UIHelper.SetPetIcon(_crewSpriteList[choseCrewIndex], crewData.icon);
                _crewSpriteList[choseCrewIndex].width = 66;
                _crewSpriteList[choseCrewIndex].height = 66;
                if (crewData.delegateTypeIds.Contains(curMissionDto.type))
                    likeCount++;           
            }
            choseCrewIndex++;            
        });


        //    var conCnt_1 = 0;
        //    var conCnt_2 = 0;
        //    var conCnt_3 = 0;
        //    var conCnt_4 = 0;

        //    int[] intArrCrewInfo = new int[] { conCnt_1, conCnt_2, conCnt_3, conCnt_4 };
        //    DelegateAcceptConditions.ParamType typeUnknow = DelegateAcceptConditions.ParamType.Unknown;
        //    DelegateAcceptConditions.ParamType typeGrade = DelegateAcceptConditions.ParamType.Grade;
        //    DelegateAcceptConditions.ParamType typeNumber = DelegateAcceptConditions.ParamType.Number;
        //    DelegateAcceptConditions.ParamType typeCrewType = DelegateAcceptConditions.ParamType.CrewType;
        //    DelegateAcceptConditions.ParamType typeCrewGender = DelegateAcceptConditions.ParamType.CrewGender;
        //data.ChoseCrewCrewIdList.ForEachI((crewId, misInfo) =>
        //    {
                //if (checkCon_1(misInfo))
                //    conCnt_1++;

                //获取伙伴
                //var crewData = _crewDataDic[crewId] as Crew;
                //伙伴的性别、类型
            //    intArrCrewInfo[crewData.property]++;
            //    string[] strArrParam = _delegateAcceptConditionsDic[misInfo].param.Split(',');
                
                
            //    for (int i = 0;i < strArrParam.Length; i=i+2)
            //    {
            //        string[] arrParamItem = strArrParam[i].Split(':');
            //    }
            //});             

        for (int i = choseCrewIndex; i < crewCount; i++)
        {
            UIHelper.SetCommonIcon(_crewSpriteList[i], "btn_crewadd", true);            
        }

        #endregion

        #region 计算成功 大成功概率

        var expressionStr = curMissionDto.successFormula.Replace("teamLen", teamLenth.ToString());
        expressionStr = expressionStr.Replace("likeNum", likeCount.ToString());
        var successPer = ExpressionManager.DelegateMissionProps(
            string.Format("DelegateMisSuccessFormula{0}{1}", curMissionMsg.id,
                SystemTimeManager.Instance.GetUTCTimeStamp()), expressionStr);
        View.ProNum_2_UILabel.text = string.Format("{0}%", successPer * 100);
        expressionStr = curMissionDto.superSuccessFormula.Replace("teamLen", teamLenth.ToString());
        expressionStr = expressionStr.Replace("likeNum", likeCount.ToString());
        var supSuccessPer = ExpressionManager.DelegateMissionProps(
            string.Format("DelegateMisSuperSuccessFormula{0}", curMissionMsg.id), expressionStr);
        View.ProNum_3_UILabel.text = string.Format("{0}%", supSuccessPer * 100);
        var smlSuccessPer = 1 - successPer - supSuccessPer;
        View.ProNum_1_UILabel.text = string.Format("{0}%", smlSuccessPer * 100);

        #endregion
    }

    private AssistDelegateMissionBtnController AddMissionItemIfNotExist(int idx)
    {
        AssistDelegateMissionBtnController ctrl = null;
        _missionItemCtrlList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<AssistDelegateMissionBtnController, AssistDelegateMissionBtn>(
                View.MissionGrid_UIGrid.gameObject, AssistDelegateMissionBtn.NAME);
            _missionItemCtrlList.Add(ctrl);
            _disposable.Add(
                ctrl.OnClickItemStream.Subscribe(missionId => { _choseMissionStream.OnNext(missionId); }));
        }

        return ctrl;
    }
    
}
