// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewMainViewController.cs
// Author   : xush
// Created  : 7/27/2017 7:53:13 PM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AppDto;
using MyGameScripts.Gameplay.Player;
using UniRx;
using UnityEngine;

public partial interface ICrewMainViewController
{
    void PullExtendPanel(bool b);
    bool UpdateView(PartnerTab pageIdx);
	UniRx.IObservable<ICrewItemData> GetIconClickEvt { get; }

    UniRx.IObservable<int> IGreIconScrollEvt { get;}

    ICrewFetterViewController ICrewFetterCtrl { get; }
    IStrengthContainerViewController IStrengthenCtrl { get; }

	ICrewSkillViewController ICrewSkillViewCrrl { get; }

	void UpdateInfo(int id);
    void ChangeInfoTab(CrewInfoTab tab);
    void UpdateInfoTabState(CrewInfoTab tab);
    void OpenShowTypeList();
    void GetCrewInfoIdx(int idx);
    void GetCrewInfoId(int id);
    TabbtnManager GetTabMgr { get; }
    void HideTabBtn(bool b);
    void ShowBackground(bool b);
    CrewIconController GetCurrCrewData(int index);
    UniRx.IObservable<IStrengthContainerViewController> TrainSaveBtnUp { get; }
    UniRx.IObservable<Unit> TrainBtnUp { get; }
    UniRx.IObservable<int> GetPageChange { get; }
    UniRx.IObservable<PropertyType> GetChangeCrewType { get; }
}

public partial class CrewMainViewController
{
	private ICrewViewData _data;

    private int _curCrewId = 0;
    private int _crewMaxLv = 79;        //暂时写死79级
    private bool _isPull = true;

    private List<CrewIconController> _itemList = new List<CrewIconController>();    //主界面滑动列表
    private List<CrewIconController> _itemPageList = new List<CrewIconController>();//平铺界面列表

	private TabbtnManager tabMgr;
	public static readonly ITabInfo[] TeamTabInfos =
	{
		TabInfoData.Create((int) PartnerTab.Info, "属性"),
        TabInfoData.Create((int) PartnerTab.Skill, "技能"),
        TabInfoData.Create((int) PartnerTab.Cultivate, "培养")
        //TabInfoData.Create((int) PartnerTab.Favorable, "好感度")
    };

    private Subject<ICrewItemData> _iconClickEvt = new Subject<ICrewItemData>();
	public UniRx.IObservable<ICrewItemData> GetIconClickEvt {get { return _iconClickEvt; }}

    private Subject<int>  _iconScrollEvt=new Subject<int>();

    private Subject<PropertyType> _changeCrewTypeEvt = new Subject<PropertyType>();
    public UniRx.IObservable<PropertyType> GetChangeCrewType { get { return _changeCrewTypeEvt; } } 

    public UniRx.IObservable<int> IGreIconScrollEvt { get { return _iconScrollEvt; } }

    private ICrewInfoController _infoController;
    private Subject<int> _pageChangeEvt = new Subject<int>();
    public UniRx.IObservable<int> GetPageChange { get { return _pageChangeEvt; } }  

	private StrengthContainerViewController _StrengthenController;
    public IStrengthContainerViewController IStrengthenCtrl { get { return _StrengthenController; } }
	
	private CrewFetterViewController _fetterController;
	public ICrewFetterViewController ICrewFetterCtrl { get { return _fetterController; } }

    private CrewSkillTrainingViewController _trainingCtrl;           //研修

    private ModelDisplayController _modelDisplayer;
	private PropertyType _curType = PropertyType.All;
	private CrewSkillViewController crewSkillCtrl;

	public ICrewSkillViewController ICrewSkillViewCrrl {
		get { return crewSkillCtrl; }
	}

	private bool _isShowListBtn;
    private bool _isShowTiledBtn = true;
    private List<Crew> _allCrewList = new List<Crew>();
    private List<CrewFavor> _crewFavors = new List<CrewFavor>();

    private List<UIButton> _mainBtnList = new List<UIButton>();
    private List<UIButton> _tiledBtnList = new List<UIButton>();
    private List<UISprite> _mainBtnSprites = new List<UISprite>();
    private List<UISprite> _tiledBtnSprites = new List<UISprite>();

    private string[] _btnName = { "全部", "力量", "魔法", "控制", "辅助", };

    private delegate void _func(UIButton btn, int idx);
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
	{
        InitTabInfo();
	    InitPartnerList();
		InitModel();
		InitInfoView();
        InitStrengthView();
        InitSkillTrainView();
    }
	
	// 客户端自定义代码
	protected override void RegistCustomEvent ()
	{
	    UICamera.onClick += OnCameraClick;

	    _func listf = (btn, idx) =>
	    {
	        _disposable.Add(btn.AsObservable().Subscribe(_ =>
	        {
	            UpdateCrewList((PropertyType) idx);
                _view.ListBtnGroup_UIPanel.gameObject.SetActive(false);

                //点击刷选伙伴类型开始刷选,
	            UpdateReposition();
                _view.PartnerList_UITable.transform.parent.GetComponent<UI_Contorl_ScrollFlow>().BrushItem(1);
            }));
	    };

        _func pagef = (btn, idx) =>
        {
            _disposable.Add(btn.AsObservable().Subscribe(_ =>
            {
                UpdateCrewList((PropertyType)idx);
                SetTiledCurTypeBtn(idx);
                //点击刷选伙伴类型开始刷选,
                UpdateReposition();
                _changeCrewTypeEvt.OnNext((PropertyType)idx);
                _view.PartnerList_UITable.transform.parent.GetComponent<UI_Contorl_ScrollFlow>().BrushItem(1);
            }));
        };

        for (int i = 0; i < _view.ListBtnGroup_Transform.childCount; i++)
        {
            var listGo = _view.ListBtnGroup_Transform.GetChild(i);
            var listBtn = listGo.GetComponent<UIButton>();
            var listSprite = listGo.FindChild("Sprite").GetComponent<UISprite>();

            var tiledGo = _view.BtnGroup_Transform.GetChild(i);
            var tiledBtn = tiledGo.GetComponent<UIButton>();
            var tiledSprite = tiledGo.FindChild("Sprite").GetComponent<UISprite>();

            _mainBtnList.Add(listBtn);
            _tiledBtnList.Add(tiledBtn);
            _mainBtnSprites.Add(listSprite);
            _tiledBtnSprites.Add(tiledSprite);

            listf(listBtn, i);
            pagef(tiledBtn, i);
	    }

	    _view.CrewInfoContent_PageScrollView.onChangePage += onChangePage;

	}
	
	protected override void RemoveCustomEvent ()
	{
        
    }
	
	protected override void OnDispose()
	{
        _view.CrewInfoContent_PageScrollView.onChangePage -= onChangePage;
        UICamera.onClick -= OnCameraClick;
        _modelDisplayer.CleanUpModel();
		_disposable = _disposable.CloseOnceNull();
	}

	//在打开界面之前，初始化数据
	protected override void InitData()
	{
		_disposable = new CompositeDisposable();

        var list = DataCache.getArrayByCls<GeneralCharactor>();
        list.ForEach(d =>
        {
            if (d is Crew)
                _allCrewList.Add(d as Crew);
        });
        _crewFavors = DataCache.getArrayByCls<CrewFavor>();
    }


    // 业务逻辑数据刷新
    protected override void UpdateDataAndView(ICrewViewData data)
	{
		_data = data;

		UpdateTabData(data);
        if (_curCrewId != data.GetCurCrewId)
            UpdateModel(data.GetCurCrewId);

        _curCrewId = data.GetCurCrewId;
        UpdateCrewList(_curType);
        HideTabBtn(data.IsHadCurPantner(_curCrewId) == null);
        UpdatePageBtnState(data.IsHadCurPantner(_curCrewId) == null);
        ChangeInfoTab(data.IsHadCurPantner(_curCrewId) == null ? CrewInfoTab.InfoTab:_data.PartnerInfoData.GetCurInfoTab);
        UpdateBuyCrewSroll();
        UpdateReposition();
    }

    private void UpdatePageBtnState(bool b)
    {
        //_view.PageInfoBtn_UIButton.gameObject.SetActive(b);
        _view.pageSprite_UIButton.gameObject.SetActive(!b);
        _view.pageSprite_1_UIButton.gameObject.SetActive(!b);
    }

	private void UpdateTabData(ICrewViewData data)
	{
	    UpdateMainInfo(data.GetCurCrewId);
        switch (data.GetCurCrewTab)
		{
			case PartnerTab.Skill:
                UpdateSkillDataAndView(data.GetCurCrewId);
				break;
			case PartnerTab.Info:
				UpdateInfo(_data.GetCurCrewId);
				break;
            case PartnerTab.Cultivate:
                UpdateTrainView();
                UpdatePhaseAndRaise(_data.GetCurCrewId);
                break;
		}
	}

	private void InitTabInfo()
	{
		tabMgr = TabbtnManager.Create(TeamTabInfos, GetFunc());
	}

    public Func<int, ITabBtnController> GetFunc()
    {
        Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
            _view.TabGroup.gameObject
            , TabbtnPrefabPath.TabBtnWidget.ToString()
            , "Tabbtn_" + i);

        return func;
    }

    private bool _isShowExpaned = true;
    public void InitPartnerList()
	{
        for (int i = 0; i < _allCrewList.Count;i++)
		{
			var com = AddChild<CrewIconController, CrewIcon>(
				_view.PartnerList_UITable.gameObject
				,CrewIcon.NAME);

            var con = AddChild<CrewIconController, CrewIcon>(
                _view.TiledIconGrid_UIPageGrid.gameObject
                , CrewIcon.NAME);

            _itemList.Add(com);
            _itemPageList.Add(con);

            com.gameObject.AddComponent<UI_Contorl_ScrollFlowItem>().Index = i;
        }
        _disposable.Add(_view.UIScroll.GetClickEvt.Subscribe(d => { _iconScrollEvt.OnNext(d);
                                                                   
        }));
        _view.UIScroll.Init(new Vector3(-23,178),0.2f);
	}

	private void InitModel()
	{
		_modelDisplayer = AddChild<ModelDisplayController, ModelDisplayUIComponent>(
			View.ModelAnchor
			, ModelDisplayUIComponent.NAME);

		_modelDisplayer.Init (300, 300);
		_modelDisplayer.SetBoxColliderEnabled(true);
        
	}

    private ModelStyleInfo InitModelStyleInfo(int id)
    {
//        GameDebuger.Log("伙伴id=========" + id);
        ModelStyleInfo model = new ModelStyleInfo();
        model.defaultModelId = id;
        return model;
    }

    public void UpdateModel(int CrewId)
    {
        var crew = _data.GetCrewBookList.Find(d => d.GetCrewId== CrewId);
        _modelDisplayer.SetupModel(InitModelStyleInfo(crew.GetCrew.modelId));
        _modelDisplayer.SetModelScale(1f);
        _modelDisplayer.SetModelOffset(-0.15f);
    }

    public void UpdateMainInfo(int id)
    {
        var data = _data.IsHadCurPantner(id);
        _fetterController.gameObject.SetActive(data != null);
        _view.PageGrid.enabled = true;
        UpdateReposition();
        _view.HadPartner.gameObject.SetActive(data != null);
        _view.NoHadPartner.gameObject.SetActive(data == null);

        var crew = _data.GetCrewBookList.Find(d => d.GetCrewId == id);
        if (crew == null)
            return;

        if (data == null)
        {
            _view.NameLb_UILabel.text = crew.GetCrew.name;
            _infoController.UpdateView(null, id);
            SetOtherPartnerInfo(crew.GetCrew);
            _view.AttLb_UILabel.gameObject.SetActive(false);

            var orbment = DataCache.getDtoByCls<Orbment>(crew.GetCrew.orbmentId);
            SetCrewFaction(orbment.quartzProperty.elementId, crew.GetCrew.typeIcon);
            _view.FavorableLb_UILabel.gameObject.SetActive(false);
            return;
        }

        SetCrewFaction(crew.GetInfoDto.slotsElementLimit, crew.GetCrew.typeIcon);
        _view.NameLb_UILabel.text = crew.GetCrew.name;
        _view.AttLb_UILabel.gameObject.SetActive(true);
        _view.AttLb_UILabel.text = Mathf.Ceil(data.power).ToString();
        _view.FavorableLb_UILabel.gameObject.SetActive(true);
        SetFavorLv(data.favor);
        SetSelfPartnerInfo(data);
    }

    private void SetFavorLv(int favor)
    {
        int lv = 0;
        _crewFavors.ForEach(d =>
        {
            if (favor >= d.need)
                lv = d.id;
        });
        _view.FavorableLb_UILabel.text = string.Format("Lv.{0}", lv);
    }

    public void UpdateInfo(int id)
	{
        var data = _data.IsHadCurPantner(id);

        _infoController.UpdateView(data);
		_fetterController.UpdateView(_data.CrewFetterData);
	}

    private void UpdateTrainView()
    {
        var trainData = _data.GetCrewSkillTrainData();
        _StrengthenController.UpdateTrainView(trainData, _data.GetCurCrewId);
    }

    private void SetCrewFaction(int slots, string faction)
    {
        _view.MagicSprite_UISprite.spriteName = GlobalAttr.GetMagicIcon(slots);
        _view.FactionSprite_UISprite.spriteName = faction;
    }

    public void UpdatePhaseAndRaise(int id)
    {
        var data = _data.IsHadCurPantner(id);
        var crew = _data.GetCrewBookList.Find(d => d.GetCrewId == id);
        if (crew == null)
            return;

        var chips = _data.GetChipList.Find(d => d.chipId == crew.GetCrew.chipId);
        _StrengthenController.UpdateView(data, crew.GetCrew, chips == null ? 0 : chips.chipAmount);
    }

    public void UpdateCrewList(PropertyType type)
    {
        _curType = type;
        //_isShowListBtn = false;
        var dataList = _data.GetCrewListByType(type);
        _itemList.ForEach(item => { item.gameObject.SetActive(false); });
        _itemPageList.ForEach(item => { item.gameObject.SetActive(false); });

        dataList.ForEachI((data,idx) =>
        {
            _itemList[idx].gameObject.SetActive(true);
            _itemPageList[idx].gameObject.SetActive(true);
            
            _itemList[idx].UpdateDataAndView(data, idx, false);
            _itemPageList[idx].UpdateDataAndView(data, idx, true);
        });
        _view.TypeBtnLabel_UILabel.text = _btnName[(int)type];
        _view.TypeLabel_UILabel.text = _btnName[(int) type];

        //if(!_isShowExpaned)
        if(!_view.UIScroll.isRun)
        {
            _view.PartnerList_UITable.gameObject.SetActive(true);
        }
        SelectPageIcon();
        UpdateReposition();
    }

	private void SetSelfPartnerInfo(CrewInfoDto dto)
	{
		if (dto == null) return;

	    var lv = dto.grade + 1;
	    if (lv > _crewMaxLv)
	        lv = _crewMaxLv;

        ExpGrade grade = DataCache.getDtoByCls<ExpGrade>(lv);
		_view.ExpSlider_UISlider.value = (float)dto.exp / (float)grade.petExp;
		_view.ExpLb_UILabel.text = string.Format("{0}/{1}", dto.exp, grade.petExp);
	    _view.LvLb_UILabel.text = dto.grade.ToString();
	}

	private void SetOtherPartnerInfo(Crew data)
	{
	    var dto = _data.GetChipList.Find(d => d.chipId == data.chipId);
	    if (dto == null)
	    {
            _view.OtherExpSlider_UISlider.value = 0 / data.chipAmount;
            _view.OtherExpLb_UILabel.text = string.Format("{0}/{1}", 0, data.chipAmount);
        }
	    else
	    {
            _view.OtherExpSlider_UISlider.value = (float)dto.chipAmount / (float)data.chipAmount;
            _view.OtherExpLb_UILabel.text = string.Format("{0}/{1}", dto.chipAmount, data.chipAmount);
        }

		_view.Slider.SetActive(_view.OtherExpSlider_UISlider.value < 1);
		_view.CrewAddBtn_UIButton.gameObject.SetActive(_view.OtherExpSlider_UISlider.value >= 1);
	}

	public void PullExtendPanel(bool b)
	{
	    if (b)
            SetTiledCurTypeBtn((int)_curType);

	    if (!b)
	    {
            _view.PartnerListGroup.transform.localScale = Vector3.one;
            _isShowExpaned = true;
        }
        else
	    {
            _view.PartnerListGroup.transform.localScale = Vector3.zero;
	        _isShowExpaned = false;
	    }
        _view.PullBtn_UIButton.gameObject.SetActive(!b);
        _view.PullbackBtn_UIButton.gameObject.SetActive(b);
        _view.ExtendPanel.gameObject.SetActive(b);

	    SelectPageIcon();
        _view.PartnerModelGroup.SetActive(!b);
        _view.PartnerInfoGroup.SetActive(!b && _data.GetCurCrewTab == PartnerTab.Info);
	    _view.PartnerSkillGroup.SetActive(!b && _data.GetCurCrewTab == PartnerTab.Skill);
        _view.PartnerCultivateGroup.SetActive(!b && _data.GetCurCrewTab == PartnerTab.Cultivate);
	}

    private void UpdateReposition()
    {
        _view.PageGrid.enabled = true;
        _view.PageGrid.Reposition();
        _view.TiledIconGrid_UIGrid.Reposition();
        _view.TiledIconGrid_UIPageGrid.Reposition();
        _view.TiledIconGrid_UIPageGrid.enabled = true;
    }

    public bool UpdateView(PartnerTab pageIdx)
    {
        if (!_isShowExpaned) { return false; }

        HideAllGroup();
        UpdateInfo(_data.GetCurCrewId);
        _view.PartnerModelGroup.gameObject.SetActive(true);
		var isNeedInit = false;
        switch (pageIdx)
		{
			case PartnerTab.Info:
				_view.PartnerInfoGroup.gameObject.SetActive(true);
		        UpdateReposition();
                ChangeInfoTab(_data.PartnerInfoData.GetCurInfoTab);
				break;
			case PartnerTab.Skill:
                _view.PartnerSkillGroup.gameObject.SetActive(true);
                if (crewSkillCtrl == null)
                {
                    var ctrl = AddChild<CrewSkillViewController, CrewSkillView>(
                    _view.PartnerSkillGroup,
                    CrewSkillView.NAME
                    );
                    crewSkillCtrl = ctrl;
                    isNeedInit = true;
                }
                crewSkillCtrl.UpdateDataAndView(_data.GetCurCrewId);
                break;
			case PartnerTab.Cultivate:
                _view.PartnerCultivateGroup.gameObject.SetActive(true);
                UpdateTrainView();
                UpdatePhaseAndRaise(_data.GetCurCrewId);
                break;
			case PartnerTab.Favorable:
                _view.PartnerFavorableGroup.gameObject.SetActive(true);
                break;
		}
		return isNeedInit;
	}

	private void HideAllGroup()
	{
		_view.PartnerInfoGroup.gameObject.SetActive(false);
		_view.PartnerSkillGroup.gameObject.SetActive(false);
		_view.PartnerCultivateGroup.gameObject.SetActive(false);
		_view.PartnerFavorableGroup.gameObject.SetActive(false);
		_view.PartnerModelGroup.gameObject.SetActive(false);
	}

    private void SelectPageIcon()
    {
        _itemPageList.ForEach(item =>
        {
            item.IsSelect(item.GetCrewId == _data.GetCurCrewId);
        });
    }

    #region 研修
    private Subject<IStrengthContainerViewController> saveBtnUp = new Subject<IStrengthContainerViewController>();
    public UniRx.IObservable<IStrengthContainerViewController> TrainSaveBtnUp { get { return saveBtnUp; } }
    private Subject<Unit> trainBtnUp = new Subject<Unit>();
    public UniRx.IObservable<Unit> TrainBtnUp { get { return trainBtnUp; } }
    #endregion
    private void InitInfoView()
	{
        _infoController = AddChild<CrewInfoController, CrewInfoView>(
                _view.CrewInfoContent_Transform.gameObject
                 , CrewInfoView.NAME);

        _fetterController = AddChild<CrewFetterViewController, CrewFetterView>(_view.CrewInfoContent_Transform.gameObject
                 , CrewFetterView.NAME);
    }

    private void InitStrengthView()
    {
        _StrengthenController = AddChild<StrengthContainerViewController, StrengthContainerView>(
                _view.PartnerCultivateGroup.gameObject
                , StrengthContainerView.NAME);
        _view.PartnerCultivateGroup.gameObject.SetActive(false);
    }

    private void InitSkillTrainView()
    {
        _trainingCtrl = _StrengthenController.InitCrewSkillTrain();
        if (_trainingCtrl != null)
        {
            _disposable.Add(_trainingCtrl.OnbtnSave_UIButtonClick.Subscribe(e => saveBtnUp.OnNext(_StrengthenController)));
            _disposable.Add(_trainingCtrl.OnbtnTraining_UIButtonClick.Subscribe(e => trainBtnUp.OnNext(e)));
            _disposable.Add(_trainingCtrl.OnbtnTips_UIButtonClick.Subscribe(_ =>
            {
                ProxyTips.OpenTextTips(7, new Vector3(174, 125, 0));
            }));
        }
    }

    //列表上的选项按钮列表
    public void OpenShowTypeList()
    {
        //if(_view.UIScroll.isRun)
        //{
        //    return;
        //}
        if (_view.ListBtnGroup_UIPanel.gameObject.active)
        {
            _view.ListBtnGroup_UIPanel.gameObject.SetActive(false);
            _view.PartnerList_UITable.gameObject.SetActive(true);
            _isShowListBtn = false;
        }
        else
        {
            _view.ListBtnGroup_UIPanel.gameObject.SetActive(true);
            _view.PartnerList_UITable.gameObject.SetActive(false);
            _isShowListBtn = true;

            SetListCurTypeBtn((int)_curType);
        }
    }

    public void ChangeInfoTab(CrewInfoTab tab)
    {
        //View.CrewInfoContent_PageScrollView.SkipToPage((int)tab, false);  --策划需求,屏蔽滑动翻页
        _infoController.SetGoActive(tab == CrewInfoTab.InfoTab);
        _fetterController.SetGoActive(tab == CrewInfoTab.FetterTab);
        UpdateInfoTabState(tab);
    }

    public void UpdateInfoTabState(CrewInfoTab tab)
    {
        if(tab == 0)
            tab = CrewInfoTab.InfoTab;

        View.InfoTabLb_UILabel.fontSize = tab == CrewInfoTab.InfoTab ? 22 : 20;
        View.FetterTabLb_UILabel.fontSize = tab == CrewInfoTab.FetterTab ? 22 : 20;
        View.InfoTabLb_UILabel.text = tab == CrewInfoTab.InfoTab
            ? "属性".WrapColor(ColorConstantV3.Color_VerticalSelectColor_Str)
            : "属性".WrapColor(ColorConstantV3.Color_VerticalUnSelectColor2_Str);
        View.FetterTabLb_UILabel.text = tab == CrewInfoTab.FetterTab
            ? "羁绊".WrapColor(ColorConstantV3.Color_VerticalSelectColor_Str)
            : "羁绊".WrapColor(ColorConstantV3.Color_VerticalUnSelectColor2_Str);
        View.pageSprite_UIButton.sprite.depth = tab == CrewInfoTab.InfoTab ? 9 : 8;
        View.pageSprite_1_UIButton.sprite.depth = tab == CrewInfoTab.FetterTab ? 9 : 8;
        View.pageSprite_UIButton.sprite.spriteName = tab == CrewInfoTab.InfoTab ? "Tab_2_On" : "Tab_2_Off";
        View.pageSprite_1_UIButton.sprite.spriteName = tab == CrewInfoTab.FetterTab ? "Tab_2_On" : "Tab_2_Off";
        View.pageSprite_UIButton.normalSprite = tab == CrewInfoTab.InfoTab ? "Tab_2_On" : "Tab_2_Off";
        View.pageSprite_1_UIButton.normalSprite = tab == CrewInfoTab.FetterTab ? "Tab_2_On" : "Tab_2_Off";
    }

	public void UpdateSkillDataAndView(int id)
	{
		if (crewSkillCtrl != null)
			crewSkillCtrl.UpdateDataAndView(id);
	}


    #region 弧形滑动条需要方法
    /// <summary>
    /// 点击伙伴头像之后的回调方法
    /// </summary>
    /// <param name="id"></param>
    public void GetCrewInfoIdx(int id)
    {
        _isShowListBtn = true;
        UI_Contorl_ScrollFlowItem item=_itemList[id].gameObject.GetComponent<UI_Contorl_ScrollFlowItem>();
        _view.UIScroll.OnPointerClick(item);
    }

    public void GetCrewInfoId(int id)
    {
        //var item = _itemList.Find(t => t.GetCrewId == id);
        //item.OnClickHandler();
    }

    /// <summary>
    /// 获得当前滑动觉得的CrewData
    /// </summary>
    /// <param name="index">_itemPageList的缩影</param>
    /// <returns></returns>
    public CrewIconController GetCurrCrewData(int index)
    {
        return _itemPageList[index];
    }

    private void UpdateBuyCrewSroll()
    {
        if(_data.GetBuyCrew == BuyCrew.None)
            return;
        _data.GetBuyCrew = BuyCrew.None;
                _isShowListBtn = true;
        for(int i = 0;i < _itemList.Count;i++)
        {
            if(_itemList[i].GetCrewId == _data.GetCurCrewId)
            {
                UI_Contorl_ScrollFlowItem item=_itemList[i].gameObject.GetComponent<UI_Contorl_ScrollFlowItem>();
                _view.UIScroll.OnPointerClick(item);
                break;
            }
        }
    }



    #endregion

    public TabbtnManager GetTabMgr { get { return tabMgr; } }

    public void HideTabBtn(bool b)
    {
        if (b)
        {
            ITabInfo[] tabIndo = { TabInfoData.Create((int)PartnerTab.Info, "属性") };
            tabMgr.UpdateTabs(tabIndo, GetFunc(), (int)PartnerTab.Info); //当选中未拥有的伙伴时,回到属性界面
        }
        else
            tabMgr.UpdateTabs(TeamTabInfos, GetFunc(), (int)_data.GetCurCrewTab);
    }

    public void ShowBackground(bool b)
    {
        _view.BackGround.SetActive(b);
    }

    private void OnCameraClick(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (_view.ListBtnGroup_UIPanel.gameObject.active == true && go.name != _view.ShowTypeBtn_UIButton.name
            && panel != _view.ListBtnGroup_UIPanel)
        {
            _view.ListBtnGroup_UIPanel.gameObject.SetActive(false);
            _view.PartnerList_UITable.gameObject.SetActive(true);
            _isShowListBtn = false;
        }
    }

    private void SetListCurTypeBtn(int idx)
    {
        _mainBtnList.ForEachI((go, i) =>
        {
            go.Label.effectStyle = i == idx ? UILabel.Effect.None : UILabel.Effect.Outline;
            go.Label.color = i == idx ?
            ColorExt.HexStrToColor("000000") : ColorExt.HexStrToColor("ffffff");
            _mainBtnSprites[i].gameObject.SetActive(i == idx);
        });
    }

    private void SetTiledCurTypeBtn(int idx)
    {
        _tiledBtnList.ForEachI((go, i) =>
        {
            go.Label.effectStyle = i == idx ? UILabel.Effect.None : UILabel.Effect.Outline;
            go.Label.color = i == idx ?
            ColorExt.HexStrToColor("000000") : ColorExt.HexStrToColor("ffffff");
            _tiledBtnSprites[i].gameObject.SetActive(i == idx);
        });
    }

    private void onChangePage(int page)
    {
        _pageChangeEvt.OnNext(page);
    }
}
