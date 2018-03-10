// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewFetterItemCellController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public partial interface ICrewFetterItemCellController
{
    ICrewFetterItemVo CrewFetterItemVo { get; }
    void UpdateView(ICrewFetterItemVo data, bool isSelect);

}

public partial class CrewFetterItemCellController
{
    private CompositeDisposable _disposable;

    private int defaultheight = 134;

    private List<UISprite> IconList = new List<UISprite>();
    private List<UISprite> itemList = new List<UISprite>();

    private ICrewFetterItemVo _vo;
    public ICrewFetterItemVo CrewFetterItemVo { get { return _vo; } }


    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
        {
            _disposable.Clear();
        }

        IconList.Add(_view.PartnerIcon_1_UISprite);
        IconList.Add(_view.PartnerIcon_2_UISprite);
        IconList.Add(_view.PartnerIcon_3_UISprite);
        IconList.Add(_view.PartnerIcon_4_UISprite);

        itemList.Add(_view.IconFrame_1_UISprite);
        itemList.Add(_view.IconFrame_2_UISprite);
        itemList.Add(_view.IconFrame_3_UISprite);
        itemList.Add(_view.IconFrame_4_UISprite);

    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {

    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    public void UpdateView(ICrewFetterItemVo data, bool isSelect)
    {

        UpdateFetterItem(data);
        if (isSelect)
            ShowDescribe();
        else
            HideDescribe();
    }

    private void UpdateFetterItem(ICrewFetterItemVo vo)
    {
        if (vo == null) return;

        _vo = vo;
        var crewInfo = vo.CrewFetter;
        _view.TitleLabel_UILabel.text = crewInfo.name;
        _view.EffectLabel_UILabel.text = string.Empty;
        _view.DescribeLabel_UILabel.text = string.Format("羁绊：{0}", vo.CrewFetter.shortDescription);
        _view.ActiveLabel_UILabel.text = !vo.Acitve ? "激活" : "取消";

        bool isActiveAll = true;
        vo.CrewDic.ForEach((data) =>
        {
            isActiveAll = isActiveAll && data.Value;
        });
        _view.WarnSprite_UISprite.gameObject.SetActive(isActiveAll);
            
        UpdateCrewIcon();
    }

    private void ShowDescribe()
    {
        _view.DescribeLabel_UILabel.gameObject.SetActive(true);
        Bounds b = NGUIMath.CalculateRelativeWidgetBounds(View.DescribeLabel_UILabel.transform);
        View.BackGround_UISprite.height = defaultheight + (int)b.size.y + 10;
        View.BackGround_UISprite.MakePixelPerfect();
    }

    private void HideDescribe()
    {
        _view.DescribeLabel_UILabel.gameObject.SetActive(false);
        View.BackGround_UISprite.height = defaultheight;
        View.BackGround_UISprite.MakePixelPerfect();
    }
    private void UpdateCrewIcon()
    {
        var tempVoList = _vo.CrewDic.ToList();
        //已经拥有的伙伴优先排序
        tempVoList.Sort((a, b) =>
        {
            if (a.Value == true && b.Value == false)
                return -1;
            else if (a.Value == false && b.Value == true)
                return 1;
            return 0;
        });
        var crew = DataCache.getDtoByCls<GeneralCharactor>(_vo.CrweId) as Crew;
        UIHelper.SetPetIcon(IconList[0], crew.icon);
        tempVoList.ForEachI((data, idx) =>
        {
            UIHelper.SetPetIcon(IconList[idx + 1], data.Key.icon);
            IconList[idx + 1].isGrey = !data.Value;
            itemList[idx + 1].isGrey = !data.Value;
        });
        
        View.IconGrid_UIGrid.Reposition();
    }


}
