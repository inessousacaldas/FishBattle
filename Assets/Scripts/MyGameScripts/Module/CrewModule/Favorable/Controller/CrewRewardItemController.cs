// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewRewardItemController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using UniRx;

public class CrewRewardItemData: ICrewRewatdItem
{
    private int _itemId;
    private int _idx;

    public int GetItemId { get { return _itemId; } }
    public int GetItemIdx { get { return _idx; } }

    public static CrewRewardItemData Create(int id, int idx)
    {
        CrewRewardItemData data = new CrewRewardItemData();
        data._itemId = id;
        data._idx = idx;
        return data;
    }
}

public interface ICrewRewatdItem
{
    int GetItemId { get; }
    int GetItemIdx { get; }

}

public partial class CrewRewardItemController
{
    private int _tid;
    private ICrewRewatdItem _data;
    private Subject<int> _clickEvt = new Subject<int>();
    public UniRx.IObservable<int> GetClickHandler { get { return _clickEvt; } }  

    private Subject<ICrewRewatdItem> _iconClickEvt = new Subject<ICrewRewatdItem>();
    public UniRx.IObservable<ICrewRewatdItem> GetIconHandler { get { return _iconClickEvt;} }  

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        EventDelegate.Add(_view.CrewRewardItem_UIButton.onClick, () => { _clickEvt.OnNext(_tid); });
        EventDelegate.Add(_view.IconSprite_UIButton.onClick, () => { _iconClickEvt.OnNext(_data); });
    }

    protected override void OnDispose()
    {
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateItem(int tid, bool isLove, int idx)
    {
        _tid = tid;
        _data = CrewRewardItemData.Create(tid, idx);
        var p = DataCache.getDtoByCls<GeneralItem>(tid) as Props;
        if (p == null)
        {
            GameDebuger.LogError(string.Format("Props表没有{0},请检查", _tid));
            return;
        }
        var bagItem = BackpackDataMgr.DataMgr.GetItemByItemID(_tid);
        _view.NumLb_UILabel.text = bagItem == null
            ? "0".WrapColor(ColorConstantV3.Color_TradeRed_Str)
            : bagItem.count.ToString().WrapColor(ColorConstantV3.Color_White);

        _view.ExpMark.gameObject.SetActive(isLove);
        _view.NameLb_UILabel.text = p.name;
        var props = p.propsParam as PropsParam_12;
        if(isLove)
            _view.ValueLb_UILabel.text = props == null ? "" : string.Format("+{0}", props.value * 1.5);
        else
            _view.ValueLb_UILabel.text = props == null ? "" : string.Format("+{0}", props.value);
        UIHelper.SetItemIcon(_view.IconSprite_UISprite, p.icon);
    }
}
