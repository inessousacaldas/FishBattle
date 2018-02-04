// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  SignItemViewController.cs
// Author   : xjd
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using UniRx;
using System.Collections.Generic;

public partial class SignItemView
{
    public const string NAME2 = "SignItemView_left";
}
public partial class SignItemViewController
{
    public class SignClickEvent
    {
        public long id;
        public bool isAddbtn;
    }

    private SignClickEvent clickEvt = new SignClickEvent();

    private bool isAddBtn = false;

    private long _itemId = 0;

    private List<UISprite> _iconSprite = new List<UISprite>();

    private ItemCellController itemCellCtrl;
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        //icon初始化
        _iconSprite.Add(View.Item_1_UISprite);
        _iconSprite.Add(View.Item_2_UISprite);
        _iconSprite.Add(View.Item_3_UISprite);
        _iconSprite.Add(View.Item_4_UISprite);
        _iconSprite.Add(View.Item_5_UISprite);

        if (itemCellCtrl == null)
            itemCellCtrl = AddChild<ItemCellController, ItemCell>(View.IconAnchor, ItemCell.Prefab_BagItemCell);
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        //EventDelegate.Add(_view.AddBtn_UIButton.onClick, OnAddbtnClickHandler);

        ItemBg_UIButtonEvt.Subscribe(_ =>
        {
            if(!isAddBtn)
            {
                clickEvt.id = _itemId;
                clickEvt.isAddbtn = false;
                clickItemStream.OnNext(clickEvt);
            }
            else
            {
                clickEvt.id = -1;
                clickEvt.isAddbtn = true;
                clickItemStream.OnNext(clickEvt);
            }
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        ItemBg_UIButtonEvt.CloseOnceNull();
    }

    public void UpdateView(BagItemDto itemDto)
    {
        View.BgSprite_UISprite.gameObject.SetActive(false);
        View.DisableBg_UISprite.gameObject.SetActive(false);
        View.AddIcon_UISprite.gameObject.SetActive(false);
       
        if (itemDto.uniqueId < 0)
        {
            View.AddIcon_UISprite.gameObject.SetActive(true);
            View.Item_UIWidget.gameObject.SetActive(false);
            itemCellCtrl.Bg.spriteName = "equipment_bg";
            View.IconAnchor.SetActive(false);
            isAddBtn = true;

            return;
        }

        View.IconAnchor.SetActive(true);
        itemCellCtrl.UpdateView(itemDto);
        isAddBtn = false;
        _itemId = itemDto.uniqueId;       

        var quality = itemDto.item.quality;
        #region 刷新数据 medallion/rune
        View.Name_UILabel.text = itemDto.item.name;
        var appItem = itemDto.item as RealItem;
        if (appItem.itemType == (int)AppItem.ItemTypeEnum.Medallion)
        {
            if(itemCellCtrl.Border.spriteName == string.Empty)
                itemCellCtrl.Bg.spriteName = "equipment_bg";
            View.Tabel_UITable.gameObject.SetActive(true);
            View.Props_UILable.gameObject.SetActive(false);
            //_iconSprite.ForEach(item =>
            //{
            //    item.gameObject.SetActive(false);
            //});

            List<EngraveDto> engravesList = (itemDto.extra as MedallionDto).engraves;

            engravesList.ForEachI((item, index) =>
            {
                if (index >= _iconSprite.Count) return;
                UIHelper.SetItemIcon(_iconSprite[index], ItemHelper.GetGeneralItemByItemId(item.itemId).icon);
                _iconSprite[index].gameObject.SetActive(true);
            });
        }
        else if (appItem.itemType == (int)AppItem.ItemTypeEnum.Engrave)
        {
            View.Tabel_UITable.gameObject.SetActive(false);
            View.Props_UILable.gameObject.SetActive(true);

            var propsParam = ((itemDto.item as Props).propsParam) as PropsParam_3;
            string effectStr = string.Empty;
            if (propsParam.type == (int)PropsParam_3.EngraveType.ORDINARY)
                effectStr = string.Format("{0}:{1}~{2}", DataCache.getDtoByCls<CharacterAbility>(propsParam.cpId).name, propsParam.emin, propsParam.emax);
            else if(propsParam.type == (int)PropsParam_3.EngraveType.STRENGTHEN)
                effectStr = string.Format("强化属性:{0}~{1}", propsParam.emin, propsParam.emax);

            View.Props_UILable.text = effectStr;
        }
        #endregion
    }

    public long GetItemId()
    {
        return _itemId;
    }

    readonly UniRx.Subject<SignClickEvent> clickItemStream = new UniRx.Subject<SignClickEvent>();
    public UniRx.IObservable<SignClickEvent> OnClickItemStream
    {
        get { return clickItemStream; }
    }

}
