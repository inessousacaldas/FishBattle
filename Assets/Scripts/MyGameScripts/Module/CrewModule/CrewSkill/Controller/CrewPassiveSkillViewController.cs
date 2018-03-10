// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewPassiveSkillViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using AppDto;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;
using System;
using System.Collections.Generic;
using UniRx;

public partial interface ICrewPassiveSkillViewController
{
    void SetData(ICrewSkillData data);
    void UpdateView(int id);
    void OnTabChange(PassiveType type, ICrewSkillData data);
    void ShowForgetWindow();
    void CloseForgetWindow();
    UniRx.IObservable<ICrewSkillItemController> PsvSkillIconClick { get; }
    UniRx.IObservable<ICrewSkillItemController> PsvSkillEquipClick { get; }
    UniRx.IObservable<int> TabBtnClick { get; }
    CrewSkillItemController LastCtrl { get; }

}

public partial class CrewPassiveSkillViewController
{
    private CompositeDisposable _disposable;

    private ICrewSkillData _data;
    //private CrewSkillItemController _curSkillItem;

    private int _curCrewId = -1;
    private int _unLockNum = 0;
    private PassiveType _psvType = PassiveType.Sup;
    private bool isFirst = true;
    private ItemCellController _material;

    private RoleSkillRangeController _rangeCtrl; //技能范围

    private List<CrewSkillItemController> _itemList = new List<CrewSkillItemController>();
    private List<CrewSkillItemController> bookList = new List<CrewSkillItemController>();

    private Subject<ICrewSkillItemController> _psvSkillIconClick = new Subject<ICrewSkillItemController>();
    public UniRx.IObservable<ICrewSkillItemController> PsvSkillIconClick { get { return _psvSkillIconClick; } }

    private Subject<ICrewSkillItemController> _psvSkillEquipClick = new Subject<ICrewSkillItemController>();
    public UniRx.IObservable<ICrewSkillItemController> PsvSkillEquipClick { get { return _psvSkillEquipClick; } }

    private Subject<int> _tabBtnClick = new Subject<int>();
    public UniRx.IObservable<int> TabBtnClick { get { return _tabBtnClick; } }

    private CrewSkillItemController lastCtrl;
    public CrewSkillItemController LastCtrl { get { return lastCtrl; } }


    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
            _disposable.Dispose();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent()
    {

    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        _data = null;
        //_curSkillItem = null;
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent()
    {

    }

    public void SetData(ICrewSkillData data)
    {
        _data = data;
    }

    //点击左侧伙伴，数据的更新
    public void UpdateView(int id)
    {
        _unLockNum = ExpressionManager.UnLockCrewSkillPassive(CrewViewDataMgr.DataMgr.GetCurCrewQuality);

        //var list = _data.GetSkillCrafts(id);
        //if (list == null) return;
        CreateSkillItem();
        UpdateSkillItem(id);

        CrewSkillItemController item = _itemList.TryGetValue(0);
        if (item.PsvItemData.state == PassiveState.NeedItem)
            ShowSkillBookView();
        else if (item.PsvItemData.state == PassiveState.HaveItem)
        {
            _data.SetPsvItemData(item.PsvItemData);
            ShowSkillDescriptView();
        }

        _curCrewId = id;
    }

    public void CreateSkillItem()
    {
        if (_itemList.Count > 0) return;
        for (int i = 0; i < _view.SkillIconGroup_Transform.childCount; i++)
        {
            var lb = _view.SkillIconGroup_Transform.GetChild(i).gameObject;
            var com = AddController<CrewSkillItemController, CrewSkillItem>(lb);
            _itemList.Add(com);
            _disposable.Add(com.OnCrewSkillItem_UIButtonClick.Subscribe(_ => { _psvSkillIconClick.OnNext(com); }));
        }
    }

    public void UpdateSkillItem(int id)
    {
        var list = _data.GetPsvDtoList(id);
        //list为空是因为此时没有招募伙伴
        if (list == null)
            return;

        int i = 0;
        //先将有的赋值
        for (int max = list.Count; i < max; i++)
        {
            CrewPassiveSkill skill = _data.GetPsvVO(list[i].id);
            if (skill != null)
                _itemList[i].UpdatePsvView(skill, list[i]);
        }
        //没有的比较处理
        for (; i < 4; i++)
        {
            _itemList[i].UpdatePsvView(_unLockNum, i);
            _itemList[i].ClearTexture();
        }

    }

    public void ShowSkillDescriptView()
    {
        _view.SelectSkillGroup.SetActive(false);
        _view.SkillDescriptGroup.SetActive(true);

        UpdateSkillDescript();
    }
    public void ShowSkillBookView()
    {
        _view.SelectSkillGroup.SetActive(true);
        _view.SkillDescriptGroup.SetActive(false);
        CreateTabBtn();
        OnTabChange(PassiveType.All, _data);
    }

    public void UpdateSkillDescript()
    {
        //_curSkillItem = skillItem;

        PsvItemData data = _data.GetPsvItemData;
        if (data == null) return;
        _view.SkillNameLbl_UILabel.text = data.psvVO.name;
        _view.SkillEffectLbl_UILabel.text = RoleSkillUtils.Formula(data.psvVO.shortDescription, data.psvDto.grade);
        _view.LvLbl_UILabel.text = string.Format("Lv.{0}", data.psvDto.grade);
        //_view.ForgetBtn_UIButton.gameObject.SetActive(true);
        UIHelper.SetItemIcon(View.MaterialIcon_UISprite, data.psvVO.item.icon);
        int consumNum = BackpackDataMgr.DataMgr.GetItemCountByItemID(data.psvVO.itemId);
        _view.ConsumeLbl_UILabel.text = consumNum.ToString();
        if (data.psvDto.grade == data.psvVO.maxGrade)
        {
            UpdateBar(data.psvDto.exp, _data.GetConsumeExp(data.psvDto.grade), true);
        }
        else
        {
            UpdateBar(data.psvDto.exp, _data.GetConsumeExp(data.psvDto.grade + 1), false);
        }
    }
    private void UpdateConsumeMaterial()
    {
        //_material = AddChild<ItemCellController, ItemCell>(View.MaterialTran_Transform.gameObject, ItemCell.Prefab_ItemCell);
        //int count = (int)ModelManager.Player.GetPlayerWealth(AppVirtualItem.VirtualItemEnum.SILVER);
        //_material.UpdateViewInCrewSkill(item, count, cost.silver, font: _view.SkillAfterLbl_UILabel.bitmapFont);
    }
    private void UpdateBar(int have, int max,bool isMax)
    {
        float tmpHave = have;
        float tmpMax = max;
        float value = tmpHave / tmpMax;
        View.PercentLb_UILabel.text = string.Format("{0}/{1}", isMax ? max : have, max);
        View.ExpSlider_UISlider.value = isMax ? 1 : value;
        View.ExpSlider_UISlider.foregroundWidget.gameObject.SetActive(true);
    }

    public void ClearDescripe()
    {
        _view.SkillNameLbl_UILabel.text = string.Empty;
        _view.SkillEffectLbl_UILabel.text = string.Empty;
        _view.LvLbl_UILabel.text = string.Empty;
        _view.ExpSlider_UISlider.value = 0;
        _view.ForgetBtn_UIButton.gameObject.SetActive(false);
    }

    public void ShowForgetWindow()
    {
        View.lblForgetName_UILabel.text = "名字：" + _data.GetPsvItemData.psvVO.name;
        View.lblForgetLevel_UILabel.text = "等级：" + _data.GetPsvItemData.psvDto.grade + "级";
        View.ForgetWindow.SetActive(true);
    }
    public void CloseForgetWindow()
    {
        View.ForgetWindow.SetActive(false);
    }

    #region

    private List<CrewPassiveTypeBtnController> _tabBtnList;
    private void CreateTabBtn()
    {
        if (_tabBtnList == null)
        {
            _tabBtnList = new List<CrewPassiveTypeBtnController>();
            for (int i = 3; i >= 0; i--)
            {
                var itemCtrl = AddCachedChild<CrewPassiveTypeBtnController, CrewPassiveTypeBtn>(
                    View.ToggleBtnGroup,
                    CrewPassiveTypeBtn.NAME
                    );
                itemCtrl.UpdateView(i, GetTypeName(i));
                _tabBtnList.Add(itemCtrl);
                _disposable.Add(itemCtrl.OnClickItemStream.Subscribe(idx => { _tabBtnClick.OnNext(idx); }));
            }
        }
    }
    private string GetTypeName(int type)
    {
        switch ((PassiveType)type)
        {
            case PassiveType.All:return "全部";
            case PassiveType.Atk:return "攻击";
            case PassiveType.Def:return "防御";
            case PassiveType.Sup: return "辅助";
            
        }
        return "";
    }

    public void OnTabChange(PassiveType type, ICrewSkillData data)
    {
        if (_psvType == type)
            return;
        _psvType = type;
        _tabBtnList.ForEach(item => { item.SetSelected((int)type == item.Index); });

        var allList = data.AllPsvBooks;                                 //全部技巧书
        var haveList = haveID(data);                                    //已学习的技巧书
        if (haveList == null) return;
        var sortHaveList = new List<PassiveSkillBook>();                    //用于给背包排序
        var sortNoHaveList = new List<PassiveSkillBook>();

        bool setDefault = false;

        //先生成所有的技巧书（这样做更容易做数据的更新）
        CreateAllBooks(allList);
        int itemListCount = bookList.Count;

        ///因为背包有的需要放在前头
        //第一次遍历出背包有的和没有的
        for (int i = 0, max = allList.Count; i < max; i++)
        {
            int itemCount = BackpackDataMgr.DataMgr.GetItemCountByItemID(allList[i].id);
            if (itemCount > 0)
                sortHaveList.Add(allList[i]);
            else
                sortNoHaveList.Add(allList[i]);
        }
        int j = 0;
        List<CrewSkillItemController> tmpList = new List<CrewSkillItemController>();
        //将有的放前面
        for (int i = 0, max = sortHaveList.Count; i < max; i++)
        {
            int itemCount = BackpackDataMgr.DataMgr.GetItemCountByItemID(sortHaveList[i].id);
            bookList[j].SetItemInBag(true);
            if (type == PassiveType.Atk)
            {
                if (sortHaveList[i].bookType != (int)PassiveSkillBook.BookTypeEnum.Attack)
                    bookList[j].SetParent(View.SkillGrid_UIGrid.transform);
                else
                {//如果已经学习了则隐藏
                    if (haveList.Contains(sortHaveList[i].skillId))
                        bookList[j].SetParent(View.pool_Transform);
                    else
                        tmpList.Add(bookList[j]);
                }
            }
            else if (type == PassiveType.Def)
            {
                if (sortHaveList[i].bookType != (int)PassiveSkillBook.BookTypeEnum.Defense)
                    bookList[j].SetParent(View.pool_Transform);
                else
                {//如果已经学习了则隐藏
                    if (haveList.Contains(sortHaveList[i].skillId))
                        bookList[j].SetParent(View.pool_Transform);
                    else
                        tmpList.Add(bookList[j]);
                }
            }
            else if (type == PassiveType.Sup)
            {
                if (sortHaveList[i].bookType != (int)PassiveSkillBook.BookTypeEnum.Assist)
                    bookList[j].SetParent(View.SkillGrid_UIGrid.transform);
                else
                {//如果已经学习了则隐藏
                    if (haveList.Contains(sortHaveList[i].skillId))
                        bookList[j].SetParent(View.pool_Transform);
                    else
                        tmpList.Add(bookList[j]);
                }
            }
            else
            {
                if (haveList.Contains(sortHaveList[i].skillId))
                    bookList[j].SetParent(View.pool_Transform);
                else
                    tmpList.Add(bookList[j]);
            }
            bookList[j].SetPsvBookData(sortHaveList[i]);
            bookList[j].UpdatePsvBookView(sortHaveList[i].icon, itemCount.ToString());
            j++;
        }

        //将没有的放后面
        for (int i = 0, max = sortNoHaveList.Count; i < max; i++)
        {
            int itemCount = BackpackDataMgr.DataMgr.GetItemCountByItemID(sortNoHaveList[i].id);
            bookList[j].SetItemInBag(false);

            if (type == PassiveType.Atk)
            {
                if (sortNoHaveList[i].bookType != (int)PassiveSkillBook.BookTypeEnum.Attack)
                    bookList[j].SetParent(View.pool_Transform);
                else
                {
                    if (haveList.Contains(sortNoHaveList[i].skillId))
                        bookList[j].SetParent(View.pool_Transform);
                    else
                        tmpList.Add(bookList[j]);
                }
            }
            else if (type == PassiveType.Def)
            {
                if (sortNoHaveList[i].bookType != (int)PassiveSkillBook.BookTypeEnum.Defense)
                    bookList[j].SetParent(View.pool_Transform);
                else
                {
                    if (haveList.Contains(sortNoHaveList[i].skillId))
                        bookList[j].SetParent(View.pool_Transform);
                    else
                        tmpList.Add(bookList[j]);
                }
            }
            else if (type == PassiveType.Sup)
            {
                if (sortNoHaveList[i].bookType != (int)PassiveSkillBook.BookTypeEnum.Assist)
                    bookList[j].SetParent(View.pool_Transform);
                else
                {//如果已经学习了则隐藏
                    if (haveList.Contains(sortNoHaveList[i].skillId))
                        bookList[j].SetParent(View.pool_Transform);
                    else
                        tmpList.Add(bookList[j]);
                }
            }
            else
            {
                if (haveList.Contains(sortNoHaveList[i].skillId))
                    bookList[j].SetParent(View.pool_Transform);
                else
                    tmpList.Add(bookList[j]);
            }
            bookList[j].SetPsvBookData(sortNoHaveList[i]);
            bookList[j].UpdatePsvBookView(sortNoHaveList[i].icon, itemCount.ToString());
            j++;
        }
        //设置默认技能书
        if (tmpList.Count > 0 && setDefault == false)
        {
            setDefault = true;
            OnSkillBookClick(tmpList[0]);
        }
        int sibling = 0;
        tmpList.ForEach(e =>
        {
            e.SetParent(View.SkillGrid_UIGrid.transform);
            e.transform.SetSiblingIndex(sibling);
            e.Show();
            sibling++;
        });
        View.SkillGrid_UIGrid.Reposition();
        View.SkillScroll_UIScrollView.ResetPosition();
    }

    private void CreateAllBooks(List<PassiveSkillBook> allList)
    {
        if (bookList.Count > 0) return;
        for (int i = 0, max = allList.Count; i < max; i++)
        {
            var itemCtrl = AddChild<CrewSkillItemController, CrewSkillItem>(
                View.pool_Transform.gameObject,
                CrewSkillItem.NAME
                );
            itemCtrl.Hide();
            bookList.Add(itemCtrl);
            itemCtrl.SetPsvBookData(allList[i]);
            _disposable.Add(itemCtrl.OnCrewSkillItem_UIButtonClick.Subscribe(_ => OnSkillBookClick(itemCtrl)));
        }
    }

    private void OnSkillBookClick(CrewSkillItemController ctrl)
    {
        lastCtrl = ctrl;
        if (ctrl.GetItemInBag)
        {
            View.EquipLbl_UILabel.text = "装备魔法";
        }
        else
        {
            View.EquipLbl_UILabel.text = "获得途径";
        }
        View.SkillEffLbl_UILabel.text = ctrl.PsvBookData.description;
        View.SkillNameLbl_UILabel.text = ctrl.PsvBookData.name;
    }
    private List<int> haveID(ICrewSkillData data)
    {
        var list = data.GetPsvDtoList(CrewSkillHelper.CrewID);
        List<int> tmpList = new List<int>();
        if (list == null) return null;
        for (int i = 0, max = list.Count; i < max; i++)
        {
            tmpList.Add(list[i].id);
        }
        return tmpList;
    }
    #endregion
}
