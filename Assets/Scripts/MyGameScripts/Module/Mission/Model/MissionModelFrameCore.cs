using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public sealed partial class MissionDataMgr
{
    public partial class MissionData
    {
        // 存储用于战斗结束后的延迟调用,调用后要从列表内删除
        public List<Action> _battleFinishActions;
        // 战斗结果缓存
        private BattleResult _battleResult;
        //private IDisposable _disposables;

        private void SetupCore()
        {
            _battleFinishActions = new List<Action>();
            _battleResult = BattleResult.UNKNOW;
            _disposable = BattleDataManager.Stream.Select((data,state) => data.battleState).Subscribe(stateTuple => OnBattleFinishCallback(stateTuple));
        }

        private void DisposeCore()
        {
            _battleResult = BattleResult.UNKNOW;
            if(_battleFinishActions != null)
            {
                _battleFinishActions.Clear();
                _battleFinishActions = null;
            }
            //if(_disposable != null)
            //    _disposable.Dispose();
        }

        #region 任务数据延迟处理逻辑
        /// <summary>
        /// 任务状态的变化分为:玩家主动请求服务器 和 服务器下发通知改变 两种情况
        /// 
        /// 目前客户端的任务处理逻辑,可以简化描述为不能在战斗中执行,要等战斗结束后再执行
        /// 
        /// 任务状态变化和战斗的关系:
        /// 1.玩家主动请求服务器不会在战斗中请求,所以理论上 任务状态变化和战斗 没有关系
        /// 2.服务器下发通知时候,客户端有几率在战斗中,所以 任务状态变化和战斗 有关联
        /// 
        /// 为了统一处理和兼容莫名其妙的潜规则,无论 1,2 都在数据入口时候做统一处理,如果在战斗中,就延迟到战斗后,才执行数据处理流程
        /// 这些可以让其它系统(场景NPC,界面,玩家寻路行为),不需要再次处理事件 BattleManager.Instance.OnBattleFinishCallback += xxx
        /// </summary>
        private void BattleDelayHandle(Action delayAction,bool isDelay = true)
        {
            if(delayAction == null)
                return;
            if(isDelay && BattleDataManager.DataMgr.IsInBattle)
            {
                _battleFinishActions.Add(delayAction);
            }
            else
            {
                delayAction();
            }
        }
        private void OnBattleFinishCallback(BattleSceneStat battleResult)
        {
            _battleResult = BattleResult.UNKNOW;
            if(BattleDataManager.DataMgr.IsWin == BattleResult.WIN && _battleResult != BattleResult.WIN) {
                _battleResult = BattleResult.WIN;
                JSTimer.Instance.SetupTimer("BattleCallBackMove",() =>
                {
                    if(!BattleDataManager.DataMgr.IsInBattle)
                    {
                        for(int index = 0;index < _battleFinishActions.Count;index++)
                        {
                            if(_battleFinishActions[index] != null)
                            {
                                _battleFinishActions[index]();
                            }
                        }
                        _battleFinishActions.Clear();
                        _battleResult = BattleResult.UNKNOW;
                        JSTimer.Instance.CancelTimer("BattleCallBackMove");
                    }
                },1f);
            }
        }
        #endregion
    }
}