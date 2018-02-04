// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  AssistCookProductViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System.Collections.Generic;
using AppDto;
using UniRx;

public partial class AssistCookProductViewController
{
    private const int Count = 4;
    
    private static Dictionary<int, AssistSkillMakeConsume> clsConsumeData = DataCache.getDicByCls<AssistSkillMakeConsume>();
    private static Dictionary<int, AssistSkillGradeConsume> gradeData = DataCache.getDicByCls<AssistSkillGradeConsume>();
    private List<AssistProductItemController> _recipeList = new List<AssistProductItemController>();
    private List<AssistProductItemController> _getList = new List<AssistProductItemController>();
    private List<AssistProductItemController> _costList = new List<AssistProductItemController>();

    private CompositeDisposable _disposable;

    readonly UniRx.Subject<int> _chooseRecipeStream = new UniRx.Subject<int>();
    public UniRx.IObservable<int> OnChooseRecipeStream { get { return _chooseRecipeStream ; } }

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if(_disposable == null)
            _disposable = new CompositeDisposable();

        for (var i = 0; i < Count; i++)
        {
            var ctrl = AddChild<AssistProductItemController, AssistProductItem>(View.Grid_UIGrid.gameObject, AssistProductItem.NAME);
            _recipeList.Add(ctrl);

            var ctrl2 = AddChild<AssistProductItemController, AssistProductItem>(View.GridGet_UIGrid.gameObject, AssistProductItem.NAME);
            _getList.Add(ctrl2);

            var ctrl3 = AddChild<AssistProductItemController, AssistProductItem>(View.GridCost_UIGrid.gameObject, AssistProductItem.NAME);
            _costList.Add(ctrl3);
        }
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        if (_disposable != null)
            _disposable.Dispose();
        _disposable = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    public void UpdateView(IAssistSkillMainData data)
    {
        var skillLevel = data.SkillLevel;
        var index = 0;
        var isGrey = false;
        _disposable.Clear();

        #region 生产配方
        clsConsumeData.ForEach(item =>
        {
            if (item.Value.skillId != data.SkillId) return;
            _recipeList[index].UpdateView(item.Value.id, item.Value.name, false, item.Value.icon);
            _recipeList[index].SetIsTips(false);

            _recipeList[index].SetIsChosed(data.CurRecipeId == item.Value.id);

            _disposable.Add(_recipeList[index].OnClickItemStream.Subscribe(id =>
            {
                _chooseRecipeStream.OnNext(id);
            }));

            if (!isGrey && !item.Value.gradeMaitchItem.IsNullOrEmpty() && skillLevel < item.Value.gradeMaitchItem[0].level)
                isGrey = true;

            if (isGrey)
                _recipeList[index].IsLock();

            index++;
        });

        for (var i = index; i < Count; i++)
        {
            _recipeList[i].gameObject.SetActive(false);
        }
        View.Grid_UIGrid.Reposition();
        #endregion

        #region 获得产品
        index = 0;
        var getItemIdToLv = new Dictionary<int, int>();
        clsConsumeData.ForEach(item =>
        {
            if (item.Value.id == data.CurRecipeId)
            {
                item.Value.gradeMaitchItem.ForEach(x =>
                {
                    x.itemId.ForEach(y =>
                    {
                        getItemIdToLv.Add(y, x.level);
                    });
                });
            }
        });

        isGrey = false;
        getItemIdToLv.ForEachI((item,i) =>
        {
            _getList[i].UpdateView(item.Key, string.Format("{0}级", item.Value));

            if (!isGrey && skillLevel < item.Value)
                isGrey = true;

            if (isGrey)
                _getList[index].IsLock();
            index++;
        });

        for(var i =index;i< Count;i++)
        {
            _getList[i].gameObject.SetActive(false);
        }
        View.GridGet_UIGrid.Reposition();
        #endregion

        #region 生产消耗
        index = 0;
        clsConsumeData.ForEach(item =>
        {
            if (item.Value.id == data.CurRecipeId)
            {
                item.Value.materials.ForEachI((x,i) =>
                {
                    var haveCount = BackpackDataMgr.DataMgr.GetItemCountByItemID(x.itemId);
                    var haveStr = haveCount < x.count ? haveCount.ToString().WrapColor(ColorConstantV3.Color_Red) : haveCount.ToString();
                    _costList[i].UpdateView(x.itemId, string.Format("{0}/{1}", haveStr, x.count));
                    _costList[i].SetScale();
                    index++;
                });
            }
        });

        if (index < Count)
        {
            var itemData = gradeData[data.SkillLevel].makeVirtualConsumeItem;
            var haveCount = ModelManager.Player.GetPlayerWealthById(itemData[0].itemId);
            var haveStr = haveCount < itemData[0].count ? haveCount.ToString().WrapColor(ColorConstantV3.Color_Red) : haveCount.ToString();
            _costList[index].UpdateView(itemData[0].itemId, string.Format("{0}/{1}", haveStr, itemData[0].count));
            _costList[index].SetScale();
            _costList[index].SetVirtualIcon(itemData[0].itemId);
            index++;
        }

        for (var i = index; i < Count; i++)
        {
            _costList[i].gameObject.SetActive(false);
        }
        View.GridCost_UIGrid.Reposition();
        #endregion
    }
}
