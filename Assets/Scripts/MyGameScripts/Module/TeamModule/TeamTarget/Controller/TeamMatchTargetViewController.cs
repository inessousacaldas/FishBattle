// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamMatchTargetViewController.cs
// Author   : xush
// Created  : 10/9/2017 9:52:26 AM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AppDto;
using UniRx;
using UnityEngine;

public class TeamMatchTargetData : ITeamMatchTargetData
{
    private int _activeId;

    public int GetActiveId { get { return _activeId; } }

    private int _maxLv;

    public int GetMaxLv { get { return _maxLv; } }

    private int _minLv;

    public int GetMinLv { get { return _minLv; } }

    private bool _isAuto;

    public bool GetIsAuto { get { return _isAuto; } }

    public static TeamMatchTargetData Create(int id, int maxLv, int minLv, bool isAuto)
    {
        TeamMatchTargetData data = new TeamMatchTargetData();
        data._activeId = id;
        data._maxLv = maxLv;
        data._minLv = minLv;
        data._isAuto = isAuto;
        return data;
    }
}

public interface ITeamMatchTargetData
{
    int GetActiveId { get; }
    int GetMaxLv { get; }
    int GetMinLv { get; }
    bool GetIsAuto { get; }
}

public partial interface ITeamMatchTargetViewController
{

}

public partial class TeamMatchTargetViewController
{
    private TeamDto _selfTeam;
    private ITeamData _teamData;

    private bool _isAuto = false;

    private int _maxLv;     //最大值
    private int _minLv;     //最小值
    private int _curMaxLv;  //选中最大等级
    private int _curMinLv;  //选中最小等级

    private int _targetID;  //选中活动id
    private int _allTargetId = 10;  //全部活动对应的id

    private List<TeamMainAction> _firstList = new List<TeamMainAction>();         //一级菜单
    private List<TeamActionTarget> _secondList = new List<TeamActionTarget>();    //二级菜单

    private Dictionary<int, UIButton> _optionList = new Dictionary<int, UIButton>();
    private List<TeamActivitySceneItemController> _activitySceneList = new List<TeamActivitySceneItemController>();

    private Dictionary<GameObject, UILabel> _lvItemDic;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _view.ChangeLvPanel.gameObject.SetActive(false);
        InitSecondListItem();
        InitOptionListItem();
        InitLvScopeList();
        HideLvRanke();
    }

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
    {
        UICamera.onClick += OnClickHandler;
        _disposable.Add(OnChangeLvBtn_UIButtonClick.Subscribe(_ => OnChangeLvBtnHandler()));
        _disposable.Add(OnConfirmBtn_UIButtonClick.Subscribe(_ => OnConfirmBtnHandler()));
    }

    protected override void RemoveCustomEvent ()
    {
    }
        
    protected override void OnDispose()
    {
        UICamera.onClick -= OnClickHandler;
        base.OnDispose();
    }

	//在打开界面之前，初始化数据
	protected override void InitData()
	{
	    _selfTeam = TeamDataMgr.DataMgr.GetSelfTeamDto();
	    if (_selfTeam != null)
	    {
	        var target = _secondList.Find(d => d.id == _selfTeam.actionTargetId);
	        _maxLv = target == null ? 100 : target.maxGrade;
	        _minLv = target == null ? 1 : target.minGrade;
	    }
	    else
	    {
            _maxLv = ModelManager.Player.GetPlayerLevel() + 100;
            _minLv = ModelManager.Player.GetPlayerLevel() - 10 > 0
                ? ModelManager.Player.GetPlayerLevel() - 10
                : 0;
        }

	    var alllist = DataCache.getArrayByCls<TeamMainAction>();

        alllist.ForEach(target =>
	    {
            if(target.id < 1000)    //目录id
                _firstList.Add(target);
            else
                _secondList.Add(target as TeamActionTarget);
        });

        //列表中需要显示全部
        TeamActionTarget allActtionTarget = new TeamActionTarget();
        allActtionTarget.id = 0;
        allActtionTarget.parentId = _allTargetId;
        allActtionTarget.name = "全部";
        allActtionTarget.minGrade = 1;
        allActtionTarget.maxGrade = 100;
        allActtionTarget.skipMatchCount = 2;
        allActtionTarget.type = 1;

        _secondList.Insert(0, allActtionTarget);
	}

    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(ITeamData data)
    {
        _teamData = data;
        var target = _secondList.Find(d => d.id == data.TeamMainViewData.GetMatchTargetData.GetActiveId);
        if (target != null)
            SetActivitySecenItem(target.parentId);

        //var activeItem = _activitySceneList.Find(d => d.GetId == data.TeamMainViewData.GetMatchTargetData.GetActiveId);
        //if (activeItem != null)
        //    OnSecenItemClick(activeItem);
    }

    private void OnClickHandler(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (_view.ChangeLvPanel.activeSelf && panel != _view.ChangeLvPanel_UIPanel)
        {
            _view.ChangeLvPanel.SetActive(false);
            GetLvRange();
            RefreshDescLabel(_curMinLv, _curMaxLv);
        }
    }

    #region 一级菜单

    private void InitOptionListItem()
    {
        TeamMainAction action = new TeamMainAction();
        action.id = 10;
        action.name = "全部";
        action.minGrade = 1;
        action.maxGrade = 100;

        _firstList.Insert(0, action);
        _firstList.ForEach(data => { CreateBtn(data); });

        _view.ItemTable_UITable.Reposition();
    }

    private void CreateBtn(TeamMainAction target)
    {
        UIButton btn = UIHelper.CreateBigBaseBtn(_view.ItemTable_UITable.gameObject
              , target.name
              , () => { SetActivitySecenItem(target.id); }
              , "BaseButton_5");

        _optionList.Add(target.id, btn);
    }

    private void SetActivitySecenItem(int parentid)
    {
        var list = parentid == _allTargetId ? _secondList : _secondList.Filter(f => f.parentId == parentid);
        _activitySceneList.ForEachI((item,idx)=>
        {
            item.gameObject.SetActive(idx < list.Count());
            if(idx < list.Count())
                item.Open(list.TryGetValue(idx));
        });

        var action = _secondList.Find(d => d.id == _teamData.TeamMainViewData.GetMatchTargetData.GetActiveId);
        if (_teamData.TeamMainViewData.GetMatchTargetData.GetActiveId == 0
            || parentid != action.parentId && parentid != _allTargetId)
            OnSecenItemClick(_activitySceneList[0]);
        else if (_teamData.TeamMainViewData.GetMatchTargetData.GetActiveId != 0)
        {
            var activeItem = _activitySceneList.Find(d => d.GetId == action.id);
            if (activeItem != null)
                OnSecenItemClick(activeItem);
        }

        _optionList.ForEach(btn =>
        {
            btn.Value.sprite.isGrey = btn.Key != parentid;
        });
        _view.RightScrollView_UIScrollView.ResetPosition();
    }
    #endregion

    #region 二级菜单
    private void InitSecondListItem()
    {
        _secondList.ForEachI((data, idx) =>
        {
            TeamActivitySceneItemController item = AddCachedChild<TeamActivitySceneItemController, TeamActivitySceneItem>(
                _view.RightTable_UIGrid.gameObject
                , TeamActivitySceneItem.NAME
                , "item_" + idx);

            item.gameObject.SetActive(true);
            item.OnClick.Subscribe(_ => { OnSecenItemClick(item); });
            _activitySceneList.Add(item);
        });

        _view.RightTable_UIGrid.Reposition();
    }

    private void OnSecenItemClick(TeamActivitySceneItemController item)
    {
        var itemId = item.GetItemData == null ? 0 : item.GetItemData.id;
        _activitySceneList.ForEach(con =>
        {
            var b = (con.GetItemData == null ? 0 : con.GetItemData.id) == itemId;
            con.IsSelect = b;
        });

        _targetID = item.GetItemData == null ? 0 : item.GetItemData.id; 
        var target = _secondList.Find(d => d.id == _targetID);
        if (target != null)
        {
            _curMinLv = target.minGrade;
            _curMaxLv = target.maxGrade;
            RefreshDescLabel(target.minGrade, target.maxGrade);
        }

        int maxlv = 100;    //上限为100级
        int minlv = 1;      //下限为1级
        _maxLv = _targetID == 0 ? 100 : target.maxGrade;
        _minLv = _targetID == 0 ? 1 : target.minGrade;
    }
    #endregion

    private void RefreshDescLabel(int min, int max)
    {
        _view.DescLabel_UILabel.text = string.Format("等级限制:   {0}-{1}级", min, max);
    }

    private void OnChangeLvBtnHandler()
    {
        //if (_targetID == 0)
        //{
        //    TipManager.AddTip("请选择组对目标");
        //    return;
        //}

        _view.ChangeLvPanel.gameObject.SetActive(true);
        _view.LeftItemGrid_UIWrapContent.SortBasedOnScrollMovement();
        _view.LeftItemScrollView_UIScrollView.panel.cachedTransform.localPosition = Vector3.zero;
        _view.LeftItemScrollView_UIScrollView.panel.clipOffset = Vector2.zero;
        _view.LeftItemGrid_UIWrapContent.WrapContent();
        _view.LeftItemGrid_UICenterOnChild.Recenter();

        _view.RightItemGrid_UIWrapContent.SortBasedOnScrollMovement();
        _view.RightItemScrollView_UIScrollView.panel.cachedTransform.localPosition = Vector3.zero;
        _view.RightItemScrollView_UIScrollView.panel.clipOffset = Vector2.zero;
        _view.RightItemGrid_UIWrapContent.WrapContent();
        _view.RightItemGrid_UICenterOnChild.Recenter();
    }

    private void OnConfirmBtnHandler()
    {
        GetLvRange();
        _isAuto = _view.autoMatchToggle_UIToggle.value;

        if (_targetID == 0 && _view.autoMatchToggle_UIToggle.value)
        {
            TipManager.AddTip("请选择组队目标");
            return;
        }

        if (_curMaxLv == 0 && _curMinLv == 0)
        {
            TipManager.AddTip("请选择等级范围");
            return;
        }

        if (_curMaxLv < _curMinLv)
        {
            var maxlv = _curMaxLv;
            _curMaxLv = _curMinLv;
            _curMinLv = maxlv;
        }

        if (_selfTeam != null)
            TeamDataMgr.TeamNetMsg.ChangeTarget(_targetID, _curMinLv, _curMaxLv, _isAuto);
        else
            TeamDataMgr.TeamNetMsg.ChangeSelfTarget(_targetID, _curMinLv, _curMaxLv, _isAuto);

        //if (_targetID == 0) return;

        OnCloseBtnHandler();
    }

    private void OnCloseBtnHandler()
    {
        UIModuleManager.Instance.CloseModule(TeamMatchTargetView.NAME);
    }

    #region 等级滑动label
    private void InitLvScopeList()
    {
        Transform leftTran = View.LeftItemGrid_UIWrapContent.transform;
        Transform rightTran = View.RightItemGrid_UIWrapContent.transform;

        _lvItemDic = new Dictionary<GameObject, UILabel>(leftTran.childCount + rightTran.childCount);

        UILabel label;
        for (int i = 0; i < leftTran.childCount; i++)
        {
            label = leftTran.GetChild(i).GetComponent<UILabel>();
            if (label != null)
                _lvItemDic.Add(label.cachedGameObject, label);
        }

        for (int i = 0; i < rightTran.childCount; i++)
        {
            label = rightTran.GetChild(i).GetComponent<UILabel>();
            if (label != null)
                _lvItemDic.Add(label.cachedGameObject, label);
        }

        View.LeftItemGrid_UIWrapContent.onInitializeItem = (go, index, realIndex) =>
        {
            OnUpdateLvItem(go, _curMinLv - realIndex);
        };

        View.RightItemGrid_UIWrapContent.onInitializeItem = (go, index, realIndex) =>
        {
            OnUpdateLvItem(go, _curMaxLv - realIndex);
        };
    }

    private void HideLvRanke()
    {
        //_view.lvTitleLbl_UILabel.gameObject.SetActive(_selfTeam != null);
        _view.descBg_UISprite.gameObject.SetActive(_selfTeam != null);
        _view.ChangeLvBtn_UIButton.gameObject.SetActive(_selfTeam != null);
    }

    private void OnUpdateLvItem(GameObject item, int lv)
    {
        UILabel label = null;
        if (_lvItemDic.TryGetValue(item, out label))
        {
            //暂时写死,等待配表
            if (lv < _minLv || lv> _maxLv)
            {
                label.text = "";
                item.SetActive(false);
            }
            else
            {
                label.text = lv.ToString();
                item.SetActive(true);
            }
        }
    }

    private void GetLvRange()
    {
        try
        {
            GameObject leftCenterGo = _view.LeftItemGrid_UICenterOnChild.centeredObject;
            if (leftCenterGo != null)
                _curMinLv = StringHelper.ToInt(_lvItemDic[leftCenterGo].text);

            GameObject rightCenterGo = _view.RightItemGrid_UICenterOnChild.centeredObject;
            if (rightCenterGo != null)
                _curMaxLv = StringHelper.ToInt(_lvItemDic[rightCenterGo].text);
        }
        catch (Exception)
        {
            TipManager.AddTip("等级设置错误");
            return;
        }
    }
    #endregion
}
