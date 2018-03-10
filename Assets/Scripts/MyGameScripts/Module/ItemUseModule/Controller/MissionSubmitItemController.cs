using UnityEngine;
using System.Collections.Generic;
using System;
using AppDto;

public class MissionSubmitItemController : ItemUseViewController {
    private const string _submitStr = "提交";
    private Action<List<BagItemDto>> _callBackDelegate;
    //任务物品需要提交的道具的数量
    private Dictionary<int,int> mitemNeetNumberList = new Dictionary<int, int>();
    //任务物品当前已经选择的道具数量
    private Dictionary<int,int> mitemSelectNumberList = new Dictionary<int, int>();

    public List<UseItemCellController> GetItemCellList()
    {
        return _itemCellList;
    }

    protected override void InitLeftGroup()
    {
        _view.OptlblLabel.text = _submitStr;
        _view.TitleLabel_UILabel.text = string.Format("{0}道具",_submitStr);
        _leftView = MissionSubmitItemViewController.Setup(_view.LGroup);
    }

    public void SetOtherParam(Action<List<BagItemDto>> callBackDelegate)
    {
        _callBackDelegate = callBackDelegate;
    }


    virtual public void SetData(BagItemDto useDto,List<BagItemDto> items,Dictionary<int,int> itemListNeet,bool isMultiple = false,bool isBefore = false,bool isCanReClick = true)
    {
        BagItemDto selectDto = null;
        if(items.Count >= 1)
        {
            selectDto = items[0];
            for(int index = 1;index < items.Count;index++)
            {
                if(selectDto.item is Equipment && items[index].item is Equipment)
                {
                    if((selectDto.item as Equipment).grade > (items[index].item as Equipment).grade)
                    {
                        selectDto = items[index];
                        continue;
                    }

                    int currS = selectDto.item.quality;
                    int newS = items[index].item.quality;

                    if(currS > newS)
                        selectDto = items[index];
                }
                //else if(selectDto.item is Props && items[index].item is Props)
               // {
                    //PropsExtraDto_21 curr = selectDto.extra as PropsExtraDto_21;
                    //PropsExtraDto_21 newDto = items[index].extra as PropsExtraDto_21;
                    //if(curr != null && newDto != null && curr.rarity > newDto.rarity)
                    //    selectDto = items[index];
                //}
            }

            //if(selectDto.item is Equipment && !EquipmentHelper.IsLowGradeEquip(selectDto))
            //{
            //    selectDto = null;
            //}
        }
        base.SetData(useDto,items,isMultiple,isBefore,isCanReClick);
        if(selectDto != null)
        {
            for(int index = 0;index < _itemCellList.Count;index++)
            {
                if(_itemCellList[index].GetData() == selectDto)
                {
                    OnItemClick(_itemCellList[index]);
                    break;
                }
            }

            int cd = 20;
            View.OptlblLabel.text = string.Format("{0}({1}秒)",_submitStr,cd);
            View.OptBtn.GetComponent<ButtonLabelSpacingAdjust>().ReAdjust();
            // 如果是在自动化模式下,选择最差的物品,并且倒计时 cd 秒,自动提交
            JSTimer.Instance.SetupCoolDown("__AutoSubmitMissionItem",cd,(remainTime) =>
            {
                View.OptlblLabel.text = string.Format("{0}({1}秒)",_submitStr,(int)remainTime);
            },
            OnOptBtn,1f);
        }
        else
        {
            base.SetData(useDto,items,isMultiple,isBefore,isCanReClick);
        }
    }

    //override protected void OnItemClick(UseItemCellController cell)
    //{
    //    if(cell.GetData() != null)
    //    {
    //        if(_isMultiple)
    //        {
    //            cell.SelectMultiple();
    //        }
    //        else
    //        {
    //            if(mitemNeetNumberList.ContainsKey(cell.GetData().itemId))
    //            {
    //                int index = cell.GetData().itemId;
    //                if(mitemSelectNumberList[index] >= mitemNeetNumberList[index])
    //                {
    //                    if(!cell.IsSelect)
    //                    {
    //                        return;
    //                    }
    //                    else
    //                    {
    //                        mitemSelectNumberList[index]--;
    //                    }
    //                }
    //                else
    //                {
    //                    if(!cell.IsSelect)
    //                    {
    //                        mitemSelectNumberList[index]++;
    //                    }
    //                    else
    //                    {
    //                        mitemSelectNumberList[index]--;
    //                    }
    //                }
    //                cell.SelectSingle(!cell.IsSelect);
    //            }
    //        }
    //        _leftView.SetData(cell.GetData());
    //    }
    //}
    override protected void OnOptBtn()
    {
        List<BagItemDto> dto = _leftView.GetDataList();
        if(dto != null)
        {
            if(_callBackDelegate != null)
            {
                _callBackDelegate(dto);
                _callBackDelegate = null;
            }
            ProxyItemUseModule.Close();
        }
        else
        {
            TipManager.AddTip(string.Format("请选择需要{0}的物品",_submitStr));
        }
    }


    protected override void OnDispose()
    {
        JSTimer.Instance.CancelCd("__AutoSubmitMissionItem");
        _callBackDelegate = null;
        base.OnDispose();
    }
}
