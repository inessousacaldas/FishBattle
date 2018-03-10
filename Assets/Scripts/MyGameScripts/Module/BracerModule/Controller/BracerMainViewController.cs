// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BracerMainViewController.cs
// Author   : xjd
// Created  : 11/9/2017 3:18:17 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using AppDto;

public partial interface IBracerMainViewController
{

}
public partial class BracerMainViewController    {

    private List<BracerMissionItemController> _missionCtrlList = new List<BracerMissionItemController>();
    private Dictionary<int, BracerGrade> _bracerGradeDic = DataCache.getDicByCls<BracerGrade>(); 
    private Dictionary<int, BracerMissionCfg> _bracerMissionDic = DataCache.getDicByCls<BracerMissionCfg>();
    private Dictionary<int, NpcDialog> _npcDialogDic = DataCache.getDicByCls<NpcDialog>();
    private long _bracerExp;
    private JSTimer.TimerTask _showDialogTimer;
    private JSTimer.TimerTask _hideDialogTimer;
    private int BubbleHdefault = 35;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        //Npc对话
        ResetTimerTask();
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        RewardBtn_UIButtonEvt.Subscribe(_ =>
        {
            BracerMainViewDataMgr.BracerMainViewNetMsg.ReqRankUp();
        });

        NPC_UIButtonEvt.Subscribe(_ =>
        {
            ResetTimerTask();
        });
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        if (_showDialogTimer != null)
        {
            _showDialogTimer.Cancel();
            _showDialogTimer = null;
        }
        if (_hideDialogTimer != null)
        {
            _hideDialogTimer.Cancel();
            _hideDialogTimer = null;
        }

        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
            
    }

    private void ResetTimerTask()
    {
        if (_npcDialogDic.ContainsKey(5016))
        {
            //todo 服务器开服时间
            ShowNpcDialog();

            _showDialogTimer = JSTimer.Instance.SetupTimer("BracerMainViewShow", () =>
            {
                ShowNpcDialog();
                _hideDialogTimer.Reset(() => { View.Bubble_UISprite.gameObject.SetActive(false); }, 4f, false);
            }, 10f, false);

            _hideDialogTimer = JSTimer.Instance.SetupTimer("BracerMainViewHide", () =>
            {
                View.Bubble_UISprite.gameObject.SetActive(false);
            }, 4f, false);
        }
    }

    private void ShowNpcDialog()
    {
        View.Bubble_UISprite.gameObject.SetActive(true);
        var randomInt = new RandomHelper().GetRandomInt(0, _npcDialogDic[5016].dialogContent.Count);
        View.Talk_UILabel.text = randomInt == 0 ? string.Format(_npcDialogDic[5016].dialogContent[randomInt], 7) : _npcDialogDic[5016].dialogContent[randomInt];
        View.Bubble_UISprite.height = View.Talk_UILabel.height + BubbleHdefault;
    }

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IBracerMainViewData data)
    {
        UpdateMyInfo(data);
        UpdateMissionPanel(data);
    }

    private void UpdateMyInfo(IBracerMainViewData data)
    {
        if (!_bracerGradeDic.ContainsKey(data.BracerRank))
            return;

        //等级 经验
        View.LvName_UILabel.text = _bracerGradeDic[data.BracerRank].name;
        View.GradeIcon_UISprite.spriteName = _bracerGradeDic[data.BracerRank].icon;
        View.GradeIcon_UISprite.isGrey = data.BracerRank == 0;
        var needExp = _bracerGradeDic.ContainsKey(data.BracerRank + 1) ? _bracerGradeDic[data.BracerRank + 1].exp : 99999999999;
        View.ExpLabel_UILabel.text = string.Format("{0}/{1}", data.BracerExp, needExp);
        View.Slider_UISlider.value = (float)data.BracerExp / (float)needExp;
        //升级按钮
        View.RewardBtn_UIButton.gameObject.SetActive(data.BracerExp >= needExp);
    }

    private void UpdateMissionPanel(IBracerMainViewData data)
    {
        var missionCount = 0;
        _missionCtrlList.GetElememtsByRange(missionCount, -1).ForEach(item => item.Hide());
        int index = 0;
        data.MissionDataList.ForEach(missionData =>
        {
            if(_bracerMissionDic.ContainsKey(missionData.missionId))
            {
                var ctrl = AddMissionItemNoExist(index);
                ctrl.UpdateView(true, 0, missionData, _bracerMissionDic[missionData.missionId]);
                ctrl.Show();
                index++;
            }
        });

        //周任务 游击士G（9级）之前无
        if(data.BracerRank >= 9)
        {
            var nowWeek = (int)SystemTimeManager.Instance.GetServerTime().DayOfWeek;
            if (nowWeek != (int)DayOfWeek.Sunday)
            {
                for (int i = nowWeek + 1; i <= (int)DayOfWeek.Saturday; i++)
                {
                    var ctrl = AddMissionItemNoExist(index);
                    ctrl.UpdateView(false, i);
                    ctrl.Show();
                    index++;
                }
            }
        }

        View.Grid_UIGrid.Reposition();
    }

    private BracerMissionItemController AddMissionItemNoExist(int index)
    {
        BracerMissionItemController ctrl = null;
        _missionCtrlList.TryGetValue(index, out ctrl);
        if(ctrl == null)
        {
            ctrl = AddChild<BracerMissionItemController, BracerMissionItem>(View.Grid_UIGrid.gameObject, BracerMissionItem.NAME);
            _missionCtrlList.Add(ctrl);
        }

        return ctrl;
    }
}
