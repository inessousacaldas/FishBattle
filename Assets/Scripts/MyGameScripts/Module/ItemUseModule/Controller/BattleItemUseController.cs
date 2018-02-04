using AppDto;
using System;

public class BattleItemUseController : ItemUseViewController
{
    public override void SetData(BagItemDto useDto, System.Collections.Generic.List<BagItemDto> items, bool isMultiple = false, bool isBefore = false, bool isCanReClick = true)
    {
        base.SetData(useDto, items, isMultiple, isBefore, isCanReClick);
	    BattleDataManager.DataMgr.SetState(BattleSceneStat.ON_COMMAND_ENTER);
    }

	protected override void InitLeftGroup()
	{
		_view.OptlblLabel.text = "使用";
		_leftView = BattleItemUseViewController.Setup(_view.LGroup);
	}
	
	/*
	public void SetOptBtn(string btnName,System.Action<BagItemDto> onClickFun)
	{
	}
	*/

	private int _itemUsedCount = 0;
	private Action<BagItemDto> _callBackDelegate;

	public void SetOtherParam(int itemUsedCount, Action<BagItemDto> callBackDelegate)
	{
		_itemUsedCount = itemUsedCount;
		_callBackDelegate = callBackDelegate;
		(_leftView as BattleItemUseViewController).UpdateItemUsedCount(_itemUsedCount);
	}

	private void Refresh()
	{
		var isChange = false;
		// 删除已经鉴定的装备
		for(var index = _items.Count - 1;index >= 0;index--)
		{
		}

		if (!isChange) return;
		{
			var index = 0;
			for(;index < _items.Count;index++)
			{
				_itemCellList[index].SetData(_items[index]);
				_itemCellList[index].SelectSingle(_leftView.GetData().index == _items[index].index);
			}
			
			while(index < _itemCellList.Count)
			{
				_itemCellList[index].SetData(null);
				index++;
			}
		}
	}
	
	protected override void OnOptBtn()
	{
		var dto = _leftView.GetData();
		if(dto != null)
		{
			//这里加上客户端预判处理
			if (_itemUsedCount >= 10 && ServiceRequestAction.ServerRequestCheck)
			{
				TipManager.AddTip("本场战斗使用物品数量已达上限");
			}
			else
			{
                CloseView(dto);
			}
		}
		else
		{
			TipManager.AddTip("请选择需要使用的物品");
		}
	}
	
	private void UpdateRigth(BagItemDto itemDto)
	{
		for(var index = 0;index < _items.Count;index++)
		{
			if (_items[index].index != itemDto.index) continue;
			_items[index] = itemDto;
			break;
		}
		
		for(var index = 0;index < _itemCellList.Count;index++)
		{
			if (_itemCellList[index].GetData().index != itemDto.index) continue;
			_itemCellList[index].SetData(itemDto);
			break;
		}
	}

    protected override void OnDispose ()
    {
        CloseView(_leftView.GetData());
    }

    private void CloseView(BagItemDto pBagItemDto)
    {
        ProxyItemUseModule.Close();
	    if (_callBackDelegate == null) return;
	    _callBackDelegate(pBagItemDto);
	    _callBackDelegate = null;
    }
}

