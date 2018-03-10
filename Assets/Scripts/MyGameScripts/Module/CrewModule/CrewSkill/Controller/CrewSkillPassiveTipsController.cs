// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewSkillPassiveTipsController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using AppDto;
using UniRx;
using Assets.Scripts.MyGameScripts.Module.RoleSkillModule;

public interface ICrewSkillPassiveTipsController
{
    TabbtnManager TabMgr { get; }

    void OnTabChange(PassiveType type, ICrewSkillData data);
    UniRx.IObservable<Unit> OnbtnLearn_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnbtnMinus_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnbtnAdd_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnbtnMax_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnbtnUp_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnbtnUse_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnbtnForget_UIButtonClick { get; }
    UniRx.IObservable<Unit> OnbtnWindowsForget_UIButtonEvt { get; }
    UniRx.IObservable<Unit> OnbtnBlackBG_UIButtonEvt { get; }
    UniRx.IObservable<Unit> OnbtnCancel_UIButtonEvt { get; }
    UniRx.IObservable<string> InputValueChange { get; }
    ICrewSkillItemCellController LastCtrl { get; }
    UniRx.IObservable<CrewSkillItemCellController> PsvTipItemClick { get; }
    void OnBtnAddClick();
    void OnBtnMinusClick();
    void OnValueChange();
    void OnBtnMaxClick();
    int GetConsumNum { get; }
    void ShowForgetWindow();
    void CloseForgetWindow();
}

public enum PsvWindowType
{
    None,
    Backpack,
    Property
}


public partial class CrewSkillPassiveTipsController:ICrewSkillPassiveTipsController
{

    private static readonly ITabInfo[] tabInfoList =
    {
        TabInfoData.Create((int)PassiveType.All,"全部")
       ,TabInfoData.Create((int)PassiveType.Atk,"攻击")
       ,TabInfoData.Create((int)PassiveType.Def,"防御")
       ,TabInfoData.Create((int)PassiveType.Sup,"辅助")
    };
    private TabbtnManager tabMgr = null;
    private List<CrewSkillItemCellController> itemList = new List<CrewSkillItemCellController>();

    private int tmpNum = 0;
    private int consumNum = 0;

    private PsvItemData tmpData;
    private ICrewSkillData skillData;

    private PsvWindowType windowType = PsvWindowType.None;
    private CrewSkillItemCellController lastCtrl;

    private CompositeDisposable _disposable;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        if (_disposable == null)
            _disposable = new CompositeDisposable();
        else
            _disposable.Dispose();
        CreateTabItem();
    }

    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {

    }

    protected override void OnDispose()
    {
        base.OnDispose();
        _disposable = _disposable.CloseOnceNull();
        tabMgr = null;
        tmpData = null;
        skillData = null;
        lastCtrl = null;
        if (itemList.Count > 0) itemList.Clear();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    private void CreateTabItem()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
            View.btnList_UIGrid.gameObject,
            TabbtnPrefabPath.TabBtnWidget_S1.ToString(),
            "Tabbtn_" + i
             );

        tabMgr = TabbtnManager.Create(tabInfoList, func);
        tabMgr.SetBtnLblFont(normalColor: ColorConstantV3.Color_VerticalUnSelectColor2_Str);
        tabMgr.SetTabBtn(3);    //为了刷新tab字体颜色
        tabMgr.SetTabBtn(0);
    }
    public TabbtnManager TabMgr
    {
        get { return tabMgr; }
    }

    public void ShowStudyView(ICrewSkillWindowController IWindowCtrl)
    {
        IWindowCtrl.UpdateView("技巧学习");
        windowType = PsvWindowType.Backpack;
        View.HaveLearned_Transform.gameObject.SetActive(false);
        View.NeedLearned_Transform.gameObject.SetActive(true);
    }

    public void ShowDesView(ICrewSkillWindowController IWindowCtrl)
    {
        IWindowCtrl.UpdateView("技巧升级");
        windowType = PsvWindowType.Property;
        View.HaveLearned_Transform.gameObject.SetActive(true);
        View.NeedLearned_Transform.gameObject.SetActive(false);
    }

    public void SetData(ICrewSkillData _data)
    {
        skillData = _data;
    }

    //学习成功切换界面
    public void UpdateBackView(ICrewSkillWindowController IWindowCtrl)
    {
        if(skillData.GetNextType == PsvWindowType.Property)
        {
            ShowDesView(IWindowCtrl);
            UpdateProperty();
            CloseForgetWindow();
        }
        skillData.SetNextType(PsvWindowType.None);
    }
    
    public void UpdatePropertyView(ICrewSkillWindowController IWindowCtrl = null)
    {
        if (skillData.GetNextType != PsvWindowType.Backpack)
        {
            //升级或使用操作
            UpdateProperty();
        }
        else
        {
            //遗忘成功切换界面
            ShowStudyView(IWindowCtrl);
            OnTabChange(PassiveType.All, skillData);
            CloseForgetWindow();
        }
        skillData.SetNextType(PsvWindowType.None);
    }

    private void UpdateProperty()
    {
        PsvItemData data = skillData.GetPsvItemData;
        tmpData = data;
        tmpNum = 0;
        View.btnNum_UIInput.value = tmpNum + "";
        OnValueChange();

        //UIHelper.SetSkillIcon(View.itemIcon_UISprite, data.psvVO.icon);
        UIHelper.SetUITexture(View.itemIcon_UISprite, data.psvVO.icon, false);
        View.lblGrade_UILabel.text = data.psvDto.grade + "";
        View.lblName_UILabel.text = data.psvVO.name;
        View.lblType_UILabel.text = "技巧";
        View.lblEff_UILabel.text = "[272020]技能效果:[-] [174181]" + RoleSkillUtils.Formula(data.psvVO.shortDescription,data.psvDto.grade)+"[-]";
        UIHelper.SetItemIcon(View.consumIcon_UISprite, data.psvVO.item.icon);
        consumNum = BackpackDataMgr.DataMgr.GetItemCountByItemID(data.psvVO.itemId);
        View.lblNum_UILabel.text = consumNum.ToString();
        View.Label_UILabel.text = tmpNum + "";
        //当技巧达到最大等级时，取自身等级
        if(data.psvDto.grade == data.psvVO.maxGrade)
        {
            UpdateBar(data.psvDto.exp, skillData.GetConsumeExp(data.psvDto.grade));
        }
        else
        {
            UpdateBar(data.psvDto.exp, skillData.GetConsumeExp(data.psvDto.grade + 1));
        }
    }
    
    public void OnValueChange()
    {
        tmpNum = StringHelper.ToInt(View.btnNum_UIInput.value);

        if(tmpNum > consumNum)
            tmpNum = consumNum;
        else if(tmpNum < 0)
            tmpNum = 0;

        View.btnNum_UIInput.value = tmpNum + "";
        UpdateTmpBar();
    }

    /// <summary>
    /// 模拟进度条
    /// </summary>
    private void UpdateTmpBar()
    {
        int grade = tmpData.psvDto.grade;
        int tmpSub = 0;         //现有消耗材料总数量
        int tmpSub2 = 0;        //到达每级需要消耗的总材料数量
        bool turn = false;
        for(int i = 1; i <= grade; i++)
        {
            //不等于一为了特殊处理第一级，反正就是会出BUG，解释不清的，不信你放开！
            if (skillData.GetConsumeExp(i) != 1)
            {
                tmpSub += skillData.GetConsumeExp(i);
            }
        }
        tmpSub += tmpData.psvDto.exp;
        tmpSub += tmpNum;
        var list = skillData.GetPsvGradeList;
        int idx = 1;
        //对模拟进度条进行数据模拟
        int max = list.Count - 1;
        for (; idx <= max; idx++) 
        {
            tmpSub2 += list[idx].consume;
            // && list[idx].id > grade
            if (tmpSub2 >= tmpSub)
            {
                if (list[idx].id > grade) 
                    break;
            }
        }
        float tmpVal1 = tmpSub2 - tmpSub;
        float tmpVal2 = 0;
        if (idx <= max)
        {
            tmpVal2 = list[idx].consume;
        }
        else
        {
            tmpVal2 = list[max].consume;
        }
        if (tmpVal1 >= 0)
        {
            //当材料不足到最高级的总消耗材料数量
            float tmpVal3 = tmpVal2 - tmpVal1;
            if(tmpVal3 / tmpVal2 < 1)
            {
                View.TmpProgress_UISlider.value = tmpVal3 / tmpVal2;
                View.lblProgress_UILabel.text = tmpVal3 + "/" + tmpVal2;
            }
            else
            {
                View.TmpProgress_UISlider.value = 0;
                int tmpIdx = idx + 1;
                if (tmpIdx < list.Count)
                {
                    View.lblProgress_UILabel.text = 0 + "/" + list[idx + 1].consume;
                    View.TmpProgress_UISlider.value = 0;
                }
                else
                {
                    //到达最大等级
                    View.TmpProgress_UISlider.value = 1;
                    View.lblProgress_UILabel.text = list[max].consume + "/" + list[max].consume;
                }
                turn = true;
            }
        }
        else
        {
            //当材料充足或是达到最大等级
            View.TmpProgress_UISlider.value = 1;
            View.lblProgress_UILabel.text = list[max].consume + "/" + list[max].consume;
        }
        if (tmpNum != 0)
        {
            //如果可以模拟升级到了下一等级就取idx
            int tmpIdx = idx;
            if (!turn)
                tmpIdx = idx - 1;

            if (tmpIdx == tmpData.psvDto.grade)
                View.Progress_UISlider.foregroundWidget.gameObject.SetActive(false);
        else
                View.Progress_UISlider.foregroundWidget.gameObject.SetActive(true);

            View.lblExp_UILabel.text = string.Format("[2D2D2D]Lv.{0}[-] [1D8E00]→ {1}[-]", tmpData.psvDto.grade, list[tmpIdx].id);
        }
        else
            View.lblExp_UILabel.text = string.Format("[2D2D2D]Lv.{0}[-]", tmpData.psvDto.grade);
    }

    private void UpdateBar(int have,int max)
    {
        float tmpHave = have;
        float tmpMax = max;
        float value = tmpHave / tmpMax;
        View.lblProgress_UILabel.text = have + "/" + max;
        View.Progress_UISlider.value = value;
        View.Progress_UISlider.foregroundWidget.gameObject.SetActive(true);
    }

    public void OnTabChange(PassiveType type,ICrewSkillData data)
    {
        SpringPanel.Begin(View.ScrollPanel, new UnityEngine.Vector3(-99, -53, 0), 8);
        SetDefaultView();
        var allList = data.AllPsvBooks;                                 //全部技巧书
        var haveList = haveID(data);                                    //已学习的技巧书
        if (haveList == null) return;
        var sortHaveList = new List<PassiveSkillBook>();                    //用于给背包排序
        var sortNoHaveList = new List<PassiveSkillBook>();

        bool setDefault = false;

        //先生成所有的技巧书（这样做更容易做数据的更新）
        CreateAllBooks(allList);
        int itemListCount = itemList.Count;
        
        ///因为背包有的需要放在前头
        //第一次遍历出背包有的和没有的
        for(int i = 0,max = allList.Count; i < max; i++)
        {
            int itemCount = BackpackDataMgr.DataMgr.GetItemCountByItemID(allList[i].id);
            if (itemCount > 0)
                sortHaveList.Add(allList[i]);
            else
                sortNoHaveList.Add(allList[i]);
        }
        int j = 0;
        List<CrewSkillItemCellController> tmpList = new List<CrewSkillItemCellController>();
        //将有的放前面
        for (int i = 0,max = sortHaveList.Count; i < max; i++)
        {
            int itemCount = BackpackDataMgr.DataMgr.GetItemCountByItemID(sortHaveList[i].id);
            itemList[j].SetItemInBag(true);
            if (type == PassiveType.Atk)
            {
                if (sortHaveList[i].bookType != (int)PassiveSkillBook.BookTypeEnum.Attack)
                    itemList[j].SetParent(View.pool_Trans);
                else
                {//如果已经学习了则隐藏
                    if (haveList.Contains(sortHaveList[i].skillId))
                        itemList[j].SetParent(View.pool_Trans);
                    else
                        tmpList.Add(itemList[j]);
                }
            }
            else if(type == PassiveType.Def)
            {
                if (sortHaveList[i].bookType != (int)PassiveSkillBook.BookTypeEnum.Defense)
                    itemList[j].SetParent(View.pool_Trans);
                else
                {//如果已经学习了则隐藏
                    if (haveList.Contains(sortHaveList[i].skillId))
                        itemList[j].SetParent(View.pool_Trans);
                    else
                        tmpList.Add(itemList[j]);
                }
            }
            else if(type == PassiveType.Sup)
            {
                if(sortHaveList[i].bookType != (int)PassiveSkillBook.BookTypeEnum.Assist)
                    itemList[j].SetParent(View.pool_Trans);
                else
                {//如果已经学习了则隐藏
                    if (haveList.Contains(sortHaveList[i].skillId))
                        itemList[j].SetParent(View.pool_Trans);
                    else
                        tmpList.Add(itemList[j]);
                }
            }
            else
            {
                if (haveList.Contains(sortHaveList[i].skillId))
                    itemList[j].SetParent(View.pool_Trans);
                else
                    tmpList.Add(itemList[j]);
            }
            itemList[j].SetPsvBookData(sortHaveList[i]);
            itemList[j].UpdatePsvView(sortHaveList[i].icon, itemCount.ToString(), sortHaveList[i].quality);
            j++;
        }

        //将没有的放后面
        for (int i = 0, max = sortNoHaveList.Count; i < max; i++)
        {
            int itemCount = BackpackDataMgr.DataMgr.GetItemCountByItemID(sortNoHaveList[i].id);
            itemList[j].SetItemInBag(false);

            if (type == PassiveType.Atk)
            {
                if (sortNoHaveList[i].bookType != (int)PassiveSkillBook.BookTypeEnum.Attack)
                    itemList[j].SetParent(View.pool_Trans);
                else
                {
                    if (haveList.Contains(sortNoHaveList[i].skillId))
                        itemList[j].SetParent(View.pool_Trans);
                    else
                        tmpList.Add(itemList[j]);
                }
            }
            else if (type == PassiveType.Def)
            {
                if (sortNoHaveList[i].bookType != (int)PassiveSkillBook.BookTypeEnum.Defense)
                    itemList[j].SetParent(View.pool_Trans);
                else
                {
                    if (haveList.Contains(sortNoHaveList[i].skillId))
                        itemList[j].SetParent(View.pool_Trans);
                    else
                        tmpList.Add(itemList[j]);
                }
            }
            else if (type == PassiveType.Sup)
            {
                if (sortNoHaveList[i].bookType != (int)PassiveSkillBook.BookTypeEnum.Assist)
                    itemList[j].SetParent(View.pool_Trans);
                else
                {//如果已经学习了则隐藏
                    if (haveList.Contains(sortNoHaveList[i].skillId))
                        itemList[j].SetParent(View.pool_Trans);
                    else
                        tmpList.Add(itemList[j]);
                }
            }
            else
            {
                if (haveList.Contains(sortNoHaveList[i].skillId))
                    itemList[j].SetParent(View.pool_Trans);
                else
                    tmpList.Add(itemList[j]);
            }
            itemList[j].SetPsvBookData(sortNoHaveList[i]);
            itemList[j].UpdatePsvView(sortNoHaveList[i].icon, itemCount.ToString(), sortNoHaveList[i].quality);
            j++;
        }
        //设置默认技能书
        if (tmpList.Count > 0 && setDefault == false)
        {
            setDefault = true;
            OnItemClick(tmpList[0]);
        }
        int sibling = 0;
        tmpList.ForEach(e =>
        {
            e.SetParent(View.itemList_UIGrid.transform);
            e.transform.SetSiblingIndex(sibling);
            e.Show();
            sibling++;
        } );
        View.itemList_UIGrid.Reposition();
    }
    
    private Subject<CrewSkillItemCellController> psvTipItemClick= new Subject<CrewSkillItemCellController>();
    public UniRx.IObservable<CrewSkillItemCellController> PsvTipItemClick { get { return psvTipItemClick; } }
    private void CreateAllBooks(List<PassiveSkillBook> allList)
    {
        if (itemList.Count > 0) return;
        for (int i = 0, max = allList.Count; i < max; i++)
        {
            var itemCtrl = AddChild<CrewSkillItemCellController, CrewSkillItemCell>(
                View.pool_Trans.gameObject,
                CrewSkillItemCell.NAME
                );
            itemCtrl.Hide();
            itemList.Add(itemCtrl);
            itemCtrl.SetPsvBookData(allList[i]);
            _disposable.Add(itemCtrl.OnCellClick.Subscribe(_ => OnItemClick(itemCtrl)));
        }
    }

    private List<int> haveID(ICrewSkillData data)
    {
        var list = data.GetPsvDtoList(CrewSkillHelper.CrewID);
        List<int> tmpList = new List<int>();
        if (list == null) return null;
        for(int i = 0, max = list.Count; i < max; i++)
        {
            tmpList.Add(list[i].id);
        }
        return tmpList;
    }

    public void OnItemClick(CrewSkillItemCellController ctrl)
    {
        if (lastCtrl != null)
            lastCtrl.HideSel();
        ctrl.ShowSel();
        lastCtrl = ctrl;

        if (ctrl.GetItemInBag)
        {
            View.lblLearn_UILabel.text = "学 习";
        }
        else
        {
            View.lblLearn_UILabel.text = "获得途径";
        }
        
        View.btnLearn_UIButton.gameObject.SetActive(true);
        View.lblLearn_UILabel.gameObject.SetActive(true);
        View.lblTip_UILabel.text = ctrl.PsvBookData.description;
        View.lblTipName_UILabel.text = ctrl.PsvBookData.name;
        
    }

    private void SetDefaultView()
    {
        View.lblTip_UILabel.text = "";
        View.lblTipName_UILabel.text = "";
        View.btnLearn_UIButton.gameObject.SetActive(false);
        View.lblLearn_UILabel.gameObject.SetActive(false);
    }

    public void OnBtnAddClick()
    {
        tmpNum++;
        string num = tmpNum + "";
        View.btnNum_UIInput.value = num;
    }

    public void OnBtnMinusClick()
    {
        tmpNum--;
        if (tmpNum <= 0) tmpNum = 0;
        string num = tmpNum + "";
        View.btnNum_UIInput.value = num;
    }

    public void OnBtnMaxClick()
    {
        tmpNum = consumNum;
        View.btnNum_UIInput.value = tmpNum + "";
    }

    public int GetConsumNum
    {
        get
        {
            return tmpNum;
        }
    }

    public bool IsShow
    {
        get { return View.gameObject.activeSelf; }
    }

    public PsvWindowType WindowType
    {
        get { return windowType; }
    }

    public void ShowForgetWindow()
    {
        View.lblForgetName_UILabel.text = "名字："+tmpData.psvVO.name;
        View.lblForgetLevel_UILabel.text = "等级：" + tmpData.psvDto.grade + "级";
        View.ForgetWindow_Transform.gameObject.SetActive(true);
    }

    public void CloseForgetWindow()
    {
        View.ForgetWindow_Transform.gameObject.SetActive(false);
    }

    public ICrewSkillItemCellController LastCtrl
    {
        get { return lastCtrl; }
    }

}
