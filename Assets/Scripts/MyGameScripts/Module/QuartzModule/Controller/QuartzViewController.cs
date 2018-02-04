// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  QuartzViewController.cs
// Author   : xush
// Created  : 8/25/2017 6:05:18 PM
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AppDto;
using UniRx;
using UnityEngine;

public partial interface IQuartzViewController
{
	UniRx.IObservable<OrbmentInfoDto> GetClickHandler { get; }
	UniRx.IObservable<QuartzDataMgr.TabEnum> GetChangeTabHandler { get; }
    UniRx.IObservable<int> IGreIconScrollEvt { get; }
    IQuartzStrengthController StrengthCrtl { get; }
    IQuartzInfoController InfoCtrl { get; }
    UniRx.IObservable<SelectOrbmentData> GetSelectOrbmentHandler { get; }
    void GetCrewInfoIdx(int idx);
}

public partial class QuartzViewController
{
    public IQuartzStrengthController StrengthCrtl { get { return _strengthController; } }
    public IQuartzInfoController InfoCtrl { get { return _infoController; } }

    private QuartzInfoController _infoController;
	private QuartzStrengthController _strengthController;
	private QuartzForgeController _forgeController;
    private TabbtnManager _tabbtnManager;

    private IQuartzData _data;

    private int _curCrewType;
    private int _curPageIdx;
    private List<CrewIconController> _crewList = new List<CrewIconController>();
    private List<CrewIconController> _expentCrewList = new List<CrewIconController>(); 
    private delegate void _func(UIButton btn, int idx);
    private List<string> _crewTypeList = new List<string> {"全部", "力量", "魔法", "控制", "辅助"};
    private bool InitPartnerScrollView = false;
    public static readonly ITabInfo[] TabInfo =
	{
		TabInfoData.Create((int) QuartzDataMgr.TabEnum.Info, "属性"),
		TabInfoData.Create((int) QuartzDataMgr.TabEnum.Strength, "强化"),
		TabInfoData.Create((int) QuartzDataMgr.TabEnum.Forge, "制造")
	};
	
	#region Subject
	private Subject<OrbmentInfoDto> _iconClickEvt = new Subject<OrbmentInfoDto>();
	public UniRx.IObservable<OrbmentInfoDto> GetClickHandler {get { return _iconClickEvt; }}
	
	private Subject<QuartzDataMgr.TabEnum> _changeTabEvt = new Subject<QuartzDataMgr.TabEnum>();
	public UniRx.IObservable<QuartzDataMgr.TabEnum> GetChangeTabHandler{get { return _changeTabEvt; }}

    private Subject<SelectOrbmentData> _selectOrbmentEvt = new Subject<SelectOrbmentData>();
    public UniRx.IObservable<SelectOrbmentData> GetSelectOrbmentHandler { get { return _selectOrbmentEvt; } }

    private Subject<int>  _iconScrollEvt=new Subject<int>();
    public UniRx.IObservable<int> IGreIconScrollEvt { get { return _iconScrollEvt; } }
    #endregion

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
	{
		InitInfoGroup();
		CreateTabInfo();
	    InitStrengthGroup();

	}

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
	{
	    UICamera.onClick += OnCameraClick;
        _disposable.Add(PullBtn_UIButtonEvt.Subscribe(_ =>
        {
            _view.ExtendPanel.gameObject.SetActive(true);
            HideContent();
        }));
        _disposable.Add(CloseExtendBtn_UIButtonEvt.Subscribe(_ =>
        {
            ChangeTabPage(_curPageIdx);
            _view.ExtendPanel.gameObject.SetActive(false);
        }));
        _disposable.Add(PullbackBtn_UIButtonEvt.Subscribe(_ => {
            ChangeTabPage(_curPageIdx);
            _view.ExtendPanel.gameObject.SetActive(false);
        }));
        _disposable.Add(TypeBtn_UIButtonEvt.Subscribe(_ => { _view.BtnGroup_UIPanel.gameObject.SetActive(true);}));

        _disposable.Add(_infoController.GetChangeTabHandler.Subscribe(d =>
        {
            _selectOrbmentEvt.OnNext(d);
            _tabbtnManager.SetTabBtn((int)d.GetTab);
            ChangeTabPage((int)d.GetTab);
        }));
        _disposable.Add(ShowTypeBtn_UIButtonEvt.Subscribe(_ =>
        {
            _view.PartnerList_GameObject.gameObject.SetActive(false);
            _view.ListBtnGroup_Transform.gameObject.SetActive(true);
        }));
        _disposable.Add(_strengthController.GetChangeTabHandler.Subscribe(d =>
        {
            _tabbtnManager.SetTabBtn((int)d.GetTab);
            ChangeTabPage((int)d.GetTab);
        }));

	    _func f = (btn, idx) =>
	    {
	        _disposable.Add(btn.AsObservable().Subscribe(_ => { SelectCrewType(idx); }));
	    };

        for (int i = 0; i < _view.ListBtnGroup_Transform.childCount; i++)
	    {
	        var btn = _view.ListBtnGroup_Transform.GetChild(i).GetComponent<UIButton>();
            var button = _view.BtnGroup_UIPanel.transform.GetChild(i).GetComponent<UIButton>();
            f(btn, i);
	        f(button, i);
	    }
	}

	protected override void RemoveCustomEvent ()
	{
	}
	
	protected override void OnDispose()
	{
        UICamera.onClick -= OnCameraClick;
        _disposable = _disposable.CloseOnceNull();
		base.OnDispose();
	}

	//在打开界面之前，初始化数据
	protected override void InitData()
	{
		_disposable = new CompositeDisposable();
	}

	// 业务逻辑数据刷新
	protected override void UpdateDataAndView(IQuartzData data)
	{
	    if (data == null || data.GetOrbmentDto == null) return;

        _data = data;
	    InitIconList();
        UpdateChildView();
        ChangeTabPage(_curPageIdx);
    }

	private void UpdateChildView()
	{
		switch (_data.CurTabPage)
		{
			case QuartzDataMgr.TabEnum.Info:
                if (_infoController != null)
                    _infoController.UpdateDataAndView(_data.QuartzInfoData);
                break;
			case QuartzDataMgr.TabEnum.Strength:
				if(_strengthController != null)
					_strengthController.UpdateDataAndView(_data.QuartzStrengthData);
                break;
			case QuartzDataMgr.TabEnum.Forge:
				if(_forgeController != null)
					_forgeController.UpdateDataAndView(_data.QuartzForgeData);
                break;
            case QuartzDataMgr.TabEnum.InfoMagic:
                if (_infoController != null)
                {
                    _infoController.UpdateDataAndView(_data.QuartzInfoData);
                    _infoController.ChangeTab(QuartzDataMgr.TabEnum.InfoMagic);
                }
                break;
		}
	}

	private void InitIconList()
	{
	    var datalist = _data.GetOrbmentByType(_curCrewType);
        for (int i = _view.PartnerList_GameObject.transform.childCount; i < datalist.Count(); i++)
		{
			var con = AddChild<CrewIconController, CrewIcon>(_view.PartnerList_GameObject.gameObject, CrewIcon.NAME);
            con.gameObject.AddComponent<UI_Contorl_ScrollFlowItem>().Index = i;
            _crewList.Add(con);
            con.UpdateDataAndView(datalist.TryGetValue(i));
            _disposable.Add(con.GetOrbmentEvt.Subscribe(dto => { _iconClickEvt.OnNext(dto);}));
        }

        if(!InitPartnerScrollView)
        {
            _disposable.Add(_view.UI_PartnerScrollView.GetClickEvt.Subscribe(d => { _iconScrollEvt.OnNext(d); }));
            InitPartnerScrollView = true;
            _view.UI_PartnerScrollView.Init(new Vector3(-23,178),0.2f);
        }

        for (int i = _view.TiledIconGrid_UIPageGrid.transform.childCount; i < datalist.Count(); i++)
        {
            var con = AddChild<CrewIconController, CrewIcon>(_view.TiledIconGrid_UIPageGrid.gameObject, CrewIcon.NAME);
            _expentCrewList.Add(con);
            con.UpdateDataAndView(datalist.TryGetValue(i));
            _disposable.Add(con.GetOrbmentEvt.Subscribe(dto =>
            {
                _iconClickEvt.OnNext(dto);
                _view.ExtendPanel.gameObject.SetActive(false);
            }));
        }

        _view.TiledIconGrid_UIPageGrid.Reposition();
        _view.TiledScrollView_UIScrollView.ResetPosition();
    }

	private void InitInfoGroup()
	{
        if (_infoController == null)
            _infoController = AddChild<QuartzInfoController, QuartzInfo>(_view.InfoGroup, QuartzInfo.NAME);
    }

	private void InitStrengthGroup()
	{
		_strengthController = AddChild<QuartzStrengthController, QuartzStrength>(
				_view.StrengthGroup, QuartzStrength.NAME);

        _view.StrengthGroup.SetActive(false);
    }

	private void InitForgeGroup()
	{
		if (_forgeController == null)
			_forgeController = AddChild<QuartzForgeController, QuartzForge>(
				_view.ForgeGroup, QuartzForge.NAME);

        _forgeController.UpdateDataAndView(_data.QuartzForgeData);
    }
	
	private void CreateTabInfo()
	{
		Func<int, ITabBtnController> func = i => AddChild<TabBtnWidgetController, TabBtnWidget>(
			_view.TabBtn_UIGrid.gameObject
			, TabbtnPrefabPath.TabBtnWidget.ToString()
			, "Tabbtn_" + i);

        _tabbtnManager = TabbtnManager.Create(TabInfo, func);

        _tabbtnManager.Stream.Subscribe(pageIdx =>
		{
			_changeTabEvt.OnNext((QuartzDataMgr.TabEnum)pageIdx);
			ChangeTabPage(pageIdx);
		});

        if (!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_72))
            _tabbtnManager.SetBtnHide((int)QuartzDataMgr.TabEnum.Forge);
        if(!FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_71))
            _tabbtnManager.SetBtnHide((int)QuartzDataMgr.TabEnum.Strength);
    }

	private void HideAllPage()
	{
	    HideContent();
        _view.ScrollView.SetActive(true);
        _view.ScrollView_UIScrollView.SetActive(true);
        _view.ShowTypeBtn_UIButton.gameObject.SetActive(true);
    }

    private void HideContent(bool b = false)
    {
        _view.InfoGroup.SetActive(b);
        _view.ForgeGroup.SetActive(b);
        _view.StrengthGroup.SetActive(b);
        _view.ScrollView_UIScrollView.SetActive(b);
        _view.ShowTypeBtn_UIButton.gameObject.SetActive(b);
    }
	
	private void ChangeTabPage(int pageIdx)
	{
		HideAllPage();
	    _curPageIdx = pageIdx;
        _view.PullBtn_UIButton.gameObject.SetActive(pageIdx != (int)QuartzDataMgr.TabEnum.Forge);
		switch ((QuartzDataMgr.TabEnum)pageIdx)
		{
			case QuartzDataMgr.TabEnum.Info:
				_view.InfoGroup.SetActive(true);
				InitInfoGroup();
		        _infoController.UpdateDataAndView(_data.QuartzInfoData);
                break;
			case QuartzDataMgr.TabEnum.Strength:
				_view.StrengthGroup.SetActive(true);
                _strengthController.UpdateDataAndView(_data.QuartzStrengthData);
                break;
			case QuartzDataMgr.TabEnum.Forge:
				_view.ForgeGroup.SetActive(true);
				_view.ScrollView.SetActive(false);
                _view.ShowTypeBtn_UIButton.gameObject.SetActive(false);
                InitForgeGroup();
				break;
		}
	}

    private void SelectCrewType(int crewType)
    {
        _curCrewType = crewType;
        var datalist = _data.GetOrbmentByType(crewType);
        _crewList.ForEachI((item, idx) =>
        {
            if (idx < datalist.Count())
            {
                item.gameObject.SetActive(true);
                item.UpdateDataAndView(datalist.TryGetValue(idx));
            }else
                item.gameObject.SetActive(false);
        });

        _expentCrewList.ForEachI((item, idx) =>
        {
            if (idx < datalist.Count())
            {
                item.gameObject.SetActive(true);
                item.UpdateDataAndView(datalist.TryGetValue(idx));
            }
            else
                item.gameObject.SetActive(false);
        });

        _view.PartnerList_GameObject.gameObject.SetActive(true);
        _view.TypeLabel_UILabel.text = _crewTypeList[crewType];
        _view.TypeLb_UILabel.text = _crewTypeList[crewType];
        _view.ListBtnGroup_Transform.gameObject.SetActive(false);
    }


    private void OnCameraClick(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (panel != _view.ListBtnGroup_UIPanel &&
            _view.ListBtnGroup_Transform.gameObject.activeSelf)
        {
            _view.ListBtnGroup_Transform.gameObject.SetActive(false);
            _view.PartnerList_GameObject.gameObject.SetActive(true);
        }

        if (_view.BtnGroup_UIPanel.gameObject.activeSelf &&
            panel != _view.BtnGroup_UIPanel)
        {
            _view.BtnGroup_UIPanel.gameObject.SetActive(false);
        }
    }

    #region 弧形滑动条需要方法
    /// <summary>
    /// 点击伙伴头像之后的回调方法
    /// </summary>
    /// <param name="id"></param>
    public void GetCrewInfoIdx(int id)
    {
        _crewList[id].OnClickHandler();
    }
    #endregion
}
