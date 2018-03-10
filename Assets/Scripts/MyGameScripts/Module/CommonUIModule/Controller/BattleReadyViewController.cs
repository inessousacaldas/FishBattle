// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  BattleReadyViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;

public partial class BattleReadyViewController
{
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

    //默认5s倒计时关闭界面
    public void OpenBattleReadyView(BattleReadyNotify notify, int time = 5)
    {
        notify.teamPlayersA.ForEachI((data, idx) =>
        {
            var go = _view.SelfTeamGrid_Transform.GetChild(idx);
            var item = AddController<BattleReadyPlayerItemController, BattleReadyPlayerItem>(go.gameObject);

            item.SetPlayerInfo(data);
            go.gameObject.SetActive(true);
        });

        notify.teamPlayersB.ForEachI((data, idx) =>
        {
            var go = _view.EnemyTeamGrid_Transform.GetChild(idx);
            var item = AddController<BattleReadyPlayerItemController, BattleReadyPlayerItem>(go.gameObject);

            item.SetPlayerInfo(data);
            go.gameObject.SetActive(true);
        });

        JSTimer.Instance.SetupCoolDown("BattleReadyViewController", time, e =>
        {
            time -= 1;
            _view.TimeLb_UILabel.text = string.Format("{0}s", time);
        }, () =>
        {
            _view.TimeLb_UILabel.text = "0s";
            JSTimer.Instance.CancelCd("BattleReadyViewController");
            UIModuleManager.Instance.CloseModule(BattleReadyView.NAME);
        }, 1f);
    }

}
