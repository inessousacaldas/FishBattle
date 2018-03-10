using AppDto;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule.View;
using System;
using UniRx;
using UnityEngine;

public sealed partial class RoleSkillDataMgr
{
    
    public partial class RoleSkillPotentialViewLogic
    {
        private static CompositeDisposable _disposable;

        public static void InitReactiveEvents(IRoleSkillPotentialViewController ctrl)
        {
            if (ctrl == null) return;
            if (_disposable == null)
                _disposable = new CompositeDisposable();
            else
            {
                _disposable.Dispose();
            }
        
            _disposable.Add(ctrl.OnbtnUp_UIButtonClick.Subscribe(_=>btnUp_UIButtonClick()));
            _disposable.Add(ctrl.OnbtnUp_10_UIButtonClick.Subscribe(_=>btnUp_10_UIButtonClick()));  
        }

        public static void Dispose()
        {
            if (DataMgr._data.PotentialData.LastItem != null)
            {
                DataMgr._data.PotentialData.LastItem = null;
            }
            if(_disposable != null)
            {
                _disposable = _disposable.CloseOnceNull();
            }
        }

        public static void OnSelectItem(RoleSkillPotentialItem item)
        {
            var lastItem = DataMgr._data.PotentialData.LastItem;
            if(lastItem != null)
            {
                if (lastItem.vo.id == item.vo.id) return;
                DataMgr._data.PotentialData.LastItem.SetSelected(false);
            }
            item.SetSelected(true);
            DataMgr._data.PotentialData.LastItem = item;
            FireData();
        }

        private static void btnUp_UIButtonClick()
        {
            ReqUpgrade(1);
        }
        private static void btnUp_10_UIButtonClick()
        {
            ReqUpgrade(10);
        }

        private static void ReqUpgrade(int time)
        {
            var potentialId = DataMgr._data.PotentialData.LastItem.vo.id;
            var pDto = DataMgr.PotentialData.GetVOByID(potentialId);
            var cost = DataMgr.PotentialData.GetCostByID(potentialId);
            
            int tempCost = (int)Cost(cost, DataMgr.PotentialData.GetLevelByID(DataMgr.PotentialData.LastItem.vo.id));
            if (pDto.Level - ModelManager.Player.GetPlayerLevel() == 0)
            {
                TipManager.AddTip(string.Format("{0}潜能等级不能超过人物等级",pDto.Name));
            }
            else
            {
                if(time == 1)
                {
                    ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, tempCost, () =>
                    {
                        RoleSkillNetMsg.ReqPotentialUpgrade(potentialId, time);
                    });
                }
                else
                {
                    int canUpCount = 0;
                    canUpCount = (int)ModelManager.Player.GetPlayerWealthSilver() / tempCost;
                    if(canUpCount > 0)
                    {
                        RoleSkillNetMsg.ReqPotentialUpgrade(potentialId, time);
                    }
                    else
                    {
                        int level = pDto.Level;
                        canUpCount = Math.Min(ModelManager.Player.GetPlayerLevel() - level, 10);
                        long needCost = 0;
                        int index = 0;
                        for(int i = level+1, max = level + canUpCount; i <= max; i++)
                        {
                            var val = DataMgr._data.potentialData.GetCostByLevel(i);
                            needCost += Cost(val,i);
                            index++;
                        }
                        ExChangeHelper.CheckIsNeedExchange(AppVirtualItem.VirtualItemEnum.SILVER, (int)needCost, () =>
                        {
                            RoleSkillNetMsg.ReqPotentialUpgrade(potentialId, index);
                        });
                    }
                }
            }
        }

        private static long Cost(long cost,int lv2)
        {
            int tmp = 0;
            int serverMax = ModelManager.Player.ServerGrade + 5;
            int serverMin = ModelManager.Player.ServerGrade - 5;
            float tmpCost = 0;
            if (lv2 >= serverMax)
                tmpCost = cost * 1.5f;
            else if (lv2 <= serverMin)
                tmpCost = cost * 0.5f;
            else
                tmpCost = cost;

            tmp = Mathf.CeilToInt(tmpCost);
            return tmp;
        }
    }
}

