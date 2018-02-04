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
        _useDto = useDto;
        _items = items;
        mitemNeetNumberList = itemListNeet;
        List<int> tItemListNeet = itemListNeet.Keys.ToList<int>();
        for(int i = 0;i < tItemListNeet.Count;i++)
        {
            mitemSelectNumberList.Add(tItemListNeet[i],0);
        }
        _isMultiple = isMultiple;
        _isBefore = isBefore;
        _isCanReClick = isCanReClick;
        mitemNeetNumberList = itemListNeet;
        int iTotalPage = (int)Math.Ceiling((double)_items.Count / PAGE_COUNT);
        _summary = Summary.create(_items.Count,iTotalPage,1,PAGE_COUNT);
        iTotalPage = iTotalPage == 0 ? 1 : iTotalPage;
        AddItem(iTotalPage);
        View.LGroup.SetActive(_items.Count > 0);
        _leftView.SetUseDto(useDto);
    }

    override protected void OnItemClick(UseItemCellController cell)
    {
        if(cell.GetData() != null)
        {
            if(_isMultiple)
            {
                cell.SelectMultiple();
            }
            else
            {
                if(mitemNeetNumberList.ContainsKey(cell.GetData().itemId))
                {
                    int index = cell.GetData().itemId;
                    if(mitemSelectNumberList[index] >= mitemNeetNumberList[index])
                    {
                        if(!cell.IsSelect)
                        {
                            return;
                        }
                        else
                        {
                            mitemSelectNumberList[index]--;
                        }
                    }
                    else
                    {
                        if(!cell.IsSelect)
                        {
                            mitemSelectNumberList[index]++;
                        }
                        else
                        {
                            mitemSelectNumberList[index]--;
                        }
                    }
                    cell.SelectSingle(!cell.IsSelect);
                }
            }
            _leftView.SetData(cell.GetData());
        }
    }
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
        _callBackDelegate = null;
        base.OnDispose();
    }
}
