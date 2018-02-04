// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  EngraveViewController.cs
// Author   : xjd
// Created  : 9/1/2017 2:37:22 PM
// Porpuse  : 
// **********************************************************************

using System;
using UniRx;
using System.Collections.Generic;
using AppDto;

public partial interface IEngraveViewController
{
    void UpdateGroovePanel(IEngraveData data);

    void UpdateMedallionList(IEngraveData data,bool isAdd=false);

    void UpdateRuneList(IEngraveData data);

    UniRx.IObservable<SignItemViewController.SignClickEvent> OnMedallionIdStream { get; }

    UniRx.IObservable<SignItemViewController.SignClickEvent> OnRuneIdStream { get; }

    void SetCurSelMedallionSpr(long id);

    void SetCurSelRuneSpr(long id);
}
public partial class EngraveViewController    {

    private List<SignItemViewController> _medallionList = new List<SignItemViewController>();

    private List<SignItemViewController> _runeList = new List<SignItemViewController>();

    private GrooveViewController _grooveCtrl = null;

    private List<UILabel> AttrList = new List<UILabel>();

    Subject<SignItemViewController.SignClickEvent> medallionIdStream = new Subject<SignItemViewController.SignClickEvent>();

    public UniRx.IObservable<SignItemViewController.SignClickEvent> OnMedallionIdStream { get { return medallionIdStream; } }

    Subject<SignItemViewController.SignClickEvent> runeIdStream = new Subject<SignItemViewController.SignClickEvent>();

    public UniRx.IObservable<SignItemViewController.SignClickEvent> OnRuneIdStream { get { return runeIdStream; } }

    CompositeDisposable viewDisposable = new CompositeDisposable();
    
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        AttrList.AddRange(View.AttrContent.GetComponentsInChildren<UILabel>());
    }

    // 客户端自定义代码
    protected override void RegistCustomEvent ()
    {

    }

    protected override void RemoveCustomEvent ()
    {
        medallionIdStream = medallionIdStream.CloseOnceNull();
        runeIdStream = runeIdStream.CloseOnceNull();
    }
        
    protected override void OnDispose()
    {
        base.OnDispose();
        viewDisposable = viewDisposable.CloseOnceNull();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
    {
        
    }
        
    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(IEngraveData data){
        viewDisposable.Clear();
        UpdateMedallionList(data);
        UpdateRuneList(data);
        UpdateGroovePanel(data);
    }

    //装备纹章出售和打开商城部分暂时屏蔽

    public void UpdateMedallionList(IEngraveData data,bool isAdd=false)
    {
        #region medallionList个数判断
        int dif = data.GetMedallionDataList().ToList().Count - _medallionList.Count; //额外一个添加按钮(不用了 暂时？todo xjd)
        if (dif > 0)
        {
            for (int i=0; i< dif; i++)
            {
                var ctrl = AddChild<SignItemViewController, SignItemView>(View.LeftTable_UITable.gameObject, "SignItemView_left");
                _medallionList.Add(ctrl);
            }
        }
        else if(dif < 0)
        {
            for (int i = 0; i < Math.Abs(dif); i++)
            {
                _medallionList[_medallionList.Count-i-1].gameObject.SetActive(false);
            }
        }

        data.GetMedallionDataList().ForEachI((itemDto,index) =>
        {
            _medallionList[index].UpdateView(itemDto);

            viewDisposable.Add(_medallionList[index].OnClickItemStream.Subscribe(item =>
            {
                medallionIdStream.OnNext(item);
            }));
        });
        #endregion

        //添加按钮
        //_medallionList[data.GetMedallionDataList().ToList().Count].gameObject.SetActive(true);
        //BagItemDto addItem = new BagItemDto();
        //addItem.uniqueId = -1;
        //_medallionList[data.GetMedallionDataList().ToList().Count].UpdateView(addItem);
        //viewDisposable.Add(_medallionList[data.GetMedallionDataList().ToList().Count].OnClickItemStream.Subscribe(item =>
        //{
        //    medallionIdStream.OnNext(item);
        //}));

        View.LeftTable_UITable.Reposition();

        //View.SaleBtn_UIButton.gameObject.SetActive(data.isShowSaleBtn);

        SetCurSelMedallionSpr(data.CurSelMedallionId);
    }

    public void UpdateRuneList(IEngraveData data)
    {
        #region runeList个数判断
        int dif = data.GetRuneDataList().ToList().Count - _runeList.Count; //额外一个添加按钮
        if (dif > 0)
        {
            for (int i = 0; i < dif; i++)
            {
                var ctrl = AddChild<SignItemViewController, SignItemView>(View.RightTable_UITable.gameObject, SignItemView.NAME);
                _runeList.Add(ctrl);
            }
        }
        else if (dif < 0)
        {
            for (int i = 0; i < Math.Abs(dif); i++)
            {
                _runeList[_runeList.Count - i - 1].gameObject.SetActive(false);
            }
        }

        data.GetRuneDataList().ForEachI((itemDto, index) =>
        {
            _runeList[index].UpdateView(itemDto);

            viewDisposable.Add(_runeList[index].OnClickItemStream.Subscribe(item =>
            {
                runeIdStream.OnNext(item);
            }));
        });
        #endregion

        //添加按钮
        //_runeList[data.GetRuneDataList().ToList().Count].gameObject.SetActive(true);
        //BagItemDto addItem = new BagItemDto();
        //addItem.uniqueId = -1;
        //_runeList[data.GetRuneDataList().ToList().Count].UpdateView(addItem);
        //viewDisposable.Add(_runeList[data.GetRuneDataList().ToList().Count].OnClickItemStream.Subscribe(item =>
        //{
        //    runeIdStream.OnNext(item);
        //}));

        View.RightTable_UITable.Reposition();

        SetCurSelRuneSpr(data.CurSelRuneId);
    }

    public void SetCurSelMedallionSpr(long id)
    {
        _medallionList.ForEach(x =>
        {
            if (x.GetItemId() == id)
                x.View.BgSprite_UISprite.gameObject.SetActive(true);
            else
                x.View.BgSprite_UISprite.gameObject.SetActive(false);
        });
    }

    public void SetCurSelRuneSpr(long id)
    {
        _runeList.ForEach(x =>
        {
            if (x.GetItemId() == id)
                x.View.BgSprite_UISprite.gameObject.SetActive(true);
            else
                x.View.BgSprite_UISprite.gameObject.SetActive(false);
        });
    }
    List<string> strProps = new List<string>();
    public void UpdateGroovePanel(IEngraveData data)
    {
        if (data.SelMedallionData == null)
        {
            View.MiddlePanel.gameObject.SetActive(false);
            //View.SaleBtn_UIButton.gameObject.SetActive(false);
            return;
        }
            
        View.MiddlePanel.gameObject.SetActive(true);
        //View.SaleBtn_UIButton.gameObject.SetActive(true);

        #region 纹章属性数据
        //中间圣能 属性
        MedallionDto itemDto = data.SelMedallionData.extra as MedallionDto;
        int usedCap = 0;
        strProps.Clear();
        float propsTimes = 1.0f;
        Dictionary<int, float> idToEffect = new Dictionary<int, float>();

        itemDto.engraves.ForEach(ItemDto =>
        {
            var localData = ItemHelper.GetGeneralItemByItemId(ItemDto.itemId);
            var propsParam = (localData as Props).propsParam as PropsParam_3;
            //强化铭刻符提升属性总倍数
            if (propsParam.type == (int)PropsParam_3.EngraveType.STRENGTHEN)
            {
                propsTimes *= ItemDto.effect;
            }
            else if (propsParam.type == (int)PropsParam_3.EngraveType.ORDINARY) //相同属性叠加
            {
                if (idToEffect.ContainsKey(ItemDto.itemId))
                    idToEffect[ItemDto.itemId] = idToEffect[ItemDto.itemId] + ItemDto.effect;
                else
                    idToEffect.Add(ItemDto.itemId, ItemDto.effect);
            }

            //圣能
            usedCap += ItemDto.occupation;
        });

        idToEffect.ForEachI((item,index) =>
        {
            var localData = ItemHelper.GetGeneralItemByItemId(item.Key);
            var propsParam = (localData as Props).propsParam as PropsParam_3;
            var str = string.Format("{0}+{1}", DataCache.getDtoByCls<CharacterAbility>(propsParam.cpId).name, (int)(item.Value * propsTimes));
            strProps.Add(str);
        });
        AttrList.ForEach(x => x.gameObject.SetActive(false));
        strProps.ForEachI((x, i) => {
            AttrList[i].gameObject.SetActive(true);
            AttrList[i].text = x;
        });
        View.AttrContent_UIGrid.Reposition();

        #endregion

        #region 显示设置
        MedallionProps appItem = data.SelMedallionData.item as MedallionProps;
        View.PercentLabel_UILabel.text =  string.Format("{0}/{1}", usedCap, appItem.capacity);
        if (usedCap >= appItem.capacity)
            View.PercentLabel_UILabel.text = string.Format("{0}/{1}", usedCap.ToString().WrapColor(ColorConstantV3.Color_Red), appItem.capacity);
        View.CapacitySlider_UISlider.value = appItem.capacity > 0 ?
            (float)usedCap / (float)appItem.capacity : 0;

        if (itemDto.engraves.ToList().TryGetLength() == 0)
            View.EmptyTips_UILabel.gameObject.SetActive(true);
        else
            View.EmptyTips_UILabel.gameObject.SetActive(false);

        if (_grooveCtrl == null)
            _grooveCtrl = AddChild<GrooveViewController, GrooveView>(View.HoleAnchor, GrooveView.NAME);
        #endregion

        _grooveCtrl.UpdateView(data);
    }
}
