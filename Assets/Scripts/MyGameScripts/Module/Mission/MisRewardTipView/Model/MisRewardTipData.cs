// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// Author   : DM-PC092
// Created  : 12/12/2017 11:42:28 AM
// **********************************************************************
using AppDto;
using System.Collections.Generic;

public interface IMisRewardTipData
{
    Dictionary<int, ItemTip> ItemTipDic { get; }
    ItemTipsNotify ItemTipsNotify { get; }
}

public sealed partial class MisRewardTipDataMgr
{
    public sealed partial class MisRewardTipData:IMisRewardTipData
    {
        private ItemTipsNotify notify = null;
        private Dictionary<int, ItemTip> tipDic = null;
        private Queue<ItemTipsNotify> battleNotifyQueue = new Queue<ItemTipsNotify>();
        private Queue<ItemTipsNotify> battleCommonQueue = new Queue<ItemTipsNotify>();
        private bool isTimer = false;
        private bool isFirst = false;
        private bool commonFirst = false;

        public void InitData()
        {

        }

        public void Dispose()
        {
            notify = null;
            if (tipDic != null) tipDic = null;
        }

        public void UpdateBattleTip()
        {
            if (BattleDataManager.DataMgr.IsWin == BattleResult.WIN && !isTimer)
            {
                isTimer = true;
                JSTimer.Instance.SetupTimer(MisRewardTipViewLogic.CheckItemTipsInBattle, () =>
                {
                    if (!BattleDataManager.DataMgr.IsInBattle && battleNotifyQueue.Count > 0 && !isFirst)
                    {
                        isFirst = true;
                        JSTimer.Instance.SetupCoolDown(MisRewardTipViewLogic.DelayItemTipsInBattle, 0.5f, null, delegate 
                        {
                            BattleRewardTips();
                        });
                    }
                    else if(!BattleDataManager.DataMgr.IsInBattle && battleCommonQueue.Count > 0 && !commonFirst)
                    {
                        commonFirst = true;
                        JSTimer.Instance.SetupCoolDown(MisRewardTipViewLogic.DelayCommonTipsInBattle, 0.5f, null, delegate
                        {
                            BattleCommonTips();
                        });
                    }
                    else if(!BattleDataManager.DataMgr.IsInBattle && battleNotifyQueue.Count == 0 && battleCommonQueue.Count == 0)
                    {
                        isTimer = false;
                        isFirst = false;
                        commonFirst = false;
                        JSTimer.Instance.CancelTimer(MisRewardTipViewLogic.CheckItemTipsInBattle);
                    }
                });
            }
        }

        private void BattleRewardTips()
        {
            var notify = battleNotifyQueue.Dequeue();
            MisRewardTipViewLogic.Open();
            if (battleNotifyQueue.Count > 0)
                JSTimer.Instance.SetupCoolDown(MisRewardTipViewLogic.UpdateItemTipsInBattle, 5f, null, BattleRewardTips);
            else
            {
                isTimer = false;
                isFirst = false;
                JSTimer.Instance.CancelTimer(MisRewardTipViewLogic.CheckItemTipsInBattle);
            }
        }

        private void BattleCommonTips()
        {
            var notify = battleCommonQueue.Dequeue();
            ShowCommonTips(notify);
            ShowLostTips(notify);
            if (battleCommonQueue.Count > 0)
                BattleCommonTips();
            else
            {
                isTimer = false;
                commonFirst = false;
                JSTimer.Instance.CancelTimer(MisRewardTipViewLogic.CheckItemTipsInBattle);
            }
        }

        public void UpdateItemTipNotify(ItemTipsNotify notify)
        {
            this.notify = notify;
            int id = notify.itemTipsId;
            if (ItemTipDic.ContainsKey(id))
            {
                var val = ItemTipDic[id];
                if(val.type == (int)ItemTip.ItemTipsEnum.Window)
                {
                    if (BattleDataManager.DataMgr.IsInBattle)
                    {
                        battleNotifyQueue.Enqueue(notify);
                    }
                    else
                    {
                        MisRewardTipViewLogic.Open();
                    }
                }
                else
                {
                    if (BattleDataManager.DataMgr.IsInBattle)
                    {
                        battleCommonQueue.Enqueue(notify);
                    }
                    else
                    {
                        ShowCommonTips(notify);
                    }
                }
            }
            ShowLostTips(notify);
        }
        private void ShowCommonTips(ItemTipsNotify notify)
        {
            var reward = notify.itemDtos;
            string des = "";
            reward.ForEach(e =>
            {
                des = "获得" + ItemHelper.GetItemName(e.itemId, e.count);
                TipManager.AddTip(des);
            });
        }

        private void ShowLostTips(ItemTipsNotify notify)
        {
            var lost = notify.lostItems;
            string des = "";
            lost.ForEach(e =>
            {
                string name = ItemHelper.GetItemName(e);
                if (!string.IsNullOrEmpty(name))
                {
                    des = string.Format("由于您的包裹已满，{0}无法正常获得，请及时清理包裹，以防道具丢失！", name);
                    TipManager.AddTip(des);
                }
                else
                    GameDebuger.Log("检查道具id: " + e + "是否存在");
            });
        }
        public ItemTipsNotify ItemTipsNotify { get { return notify; } }

        public Dictionary<int, ItemTip> ItemTipDic
        {
            get
            {
                if (tipDic == null)
                {
                    tipDic = DataCache.getDicByCls<ItemTip>();
                }
                return tipDic;
            }
        }
    }
}
