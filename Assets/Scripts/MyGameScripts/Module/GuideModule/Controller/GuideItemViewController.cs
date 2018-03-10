// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  GuideItemViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;
using System.Collections.Generic;

public partial class GuideItemViewController
{
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
 
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        GetBtn_UIButtonEvt.Subscribe(_ =>
        {
            GuideMainViewDataMgr.GuideMainViewNetMsg.ReqGetReward(_guide.id);
        });

        GoBtn_UIButtonEvt.Subscribe(_ =>
        {
            ProxyGuideMainView.Close();
            SmartGuideHelper.GuideTo(_guide.smartGuideId);
        });
    }

    protected override void OnDispose()
    {

    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private Guide _guide = null;
    private List<ItemCellController> _rewardItemList = new List<ItemCellController>();
    public void UpdateView(GuideInfoNotify dto)
    {
        _guide = dto.guide;
        View.Name_UILabel.text = dto.guide.title;
        View.Sprite_UISprite.width = View.Name_UILabel.width + 40;
        View.Detail_UILabel.text = dto.guide.description;
        if(dto.status == (int)GuideInfoNotify.GuideStatus.Progress)
        {
            View.GetBtn_UIButton.gameObject.SetActive(false);
            View.Complete_UISprite.gameObject.SetActive(false);
            View.GoBtn_UIButton.gameObject.SetActive(true);
            //参与和完成次数 /
            if(dto.guide.type == (int)Guide.GuideType.GT_1 || dto.guide.type == (int)Guide.GuideType.GT_2 || dto.guide.type == (int)Guide.GuideType.GT_5) 
            {
                View.Progress_UILabel.gameObject.SetActive(true);
                View.Progress_UILabel.text = dto.count + "/" + dto.guide.val;
                View.Progress_UILabel.transform.localPosition = new UnityEngine.Vector3(
                    View.Sprite_UISprite.transform.localPosition.x + View.Sprite_UISprite.width + 12, View.Progress_UILabel.transform.localPosition.y);
            }
            else
                View.Progress_UILabel.gameObject.SetActive(false);
        }
        else if (dto.status == (int)GuideInfoNotify.GuideStatus.CanReward)
        {
            View.GetBtn_UIButton.gameObject.SetActive(true);
            View.Complete_UISprite.gameObject.SetActive(false);
            View.GoBtn_UIButton.gameObject.SetActive(false);
            View.Progress_UILabel.gameObject.SetActive(false);
        }
        else if (dto.status == (int)GuideInfoNotify.GuideStatus.Finished)
        {
            View.GetBtn_UIButton.gameObject.SetActive(false);
            View.Complete_UISprite.gameObject.SetActive(true);
            View.GoBtn_UIButton.gameObject.SetActive(false);
            View.Progress_UILabel.gameObject.SetActive(false);
        }

        //奖励
        var itemCount = 0;
        _rewardItemList.GetElememtsByRange(itemCount, -1).ForEach(s => s.Hide());
        dto.guide.rewards.ForEachI((itemDto, index) =>
        {
            var itemCtrl = AddGuideItemIfNotExist(index);
            itemCtrl.UpdateView(itemDto,null,0,true);
            itemCtrl.Show();
            itemCtrl.SetTipsPosition(new UnityEngine.Vector3(-232, 75));
        });
        View.Grid_UIGrid.Reposition();
    }

    private ItemCellController AddGuideItemIfNotExist(int idx)
    {
        ItemCellController ctrl = null;
        _rewardItemList.TryGetValue(idx, out ctrl);
        if (ctrl == null)
        {
            ctrl = AddChild<ItemCellController, ItemCell>(View.Grid_UIGrid.gameObject, ItemCell.NAME);
            _rewardItemList.Add(ctrl);
        }

        return ctrl;
    }
}
