// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  CrewFormationCaseController.cs
// Author   : xush
// Created  : 7/24/2017 5:52:41 PM
// Porpuse  : 
// **********************************************************************

using System;
using AppDto;
using UniRx;

public interface IFormationCaseData
{
	int GetIdx { get; }
	int GetCaseId { get; }
	FormationCaseInfoDto GetDto { get; }
}

public class FormationCaseData:IFormationCaseData
{
	private int _idx;
	public int GetIdx{get { return _idx; }}

	private int _caseId;
	public int GetCaseId{get { return _caseId; }}

	private FormationCaseInfoDto _dto;
	public FormationCaseInfoDto GetDto{get { return _dto; }}
	 
	public static FormationCaseData Create(FormationCaseInfoDto dto, int idx, int caseId)
	{
		FormationCaseData data = new FormationCaseData();
		data._caseId = caseId;
		data._idx = idx;
		data._dto = dto;
		return data;
	}
}

public partial interface ICrewFormationCaseController
{
	void UpdateView(IFormationCaseData data, bool isSelect);
    UniRx.IObservable<IFormationCaseData> GetToggleClick { get; }
    UniRx.IObservable<IFormationCaseData> GetClickEvt { get; }
}

public partial class CrewFormationCaseController
{
	private CompositeDisposable _disposable;
	
	private Subject<IFormationCaseData> _toggleEvt = new Subject<IFormationCaseData>();
	public UniRx.IObservable<IFormationCaseData> GetToggleClick{get { return _toggleEvt; }}

	private Subject<IFormationCaseData> _clickEvt = new Subject<IFormationCaseData>();
	public UniRx.IObservable<IFormationCaseData> GetClickEvt {get { return _clickEvt; }}

    private IFormationCaseData _caseData;

    private string[] _nameList = {"阵容一", "阵容二", "阵容三"};
    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
	{
        //队形开放控制
        View.FormationBtn_UIButton.gameObject.SetActive(FunctionOpenHelper.isFuncOpen(FunctionOpen.FunctionOpenEnum.FUN_11));

        _disposable = new CompositeDisposable();

        var checkBox = AddController<GenericCheckBoxController, GenericCheckBox>(
            _view.GenericCheckBox);
        checkBox.UpdateView(GenericCheckBoxData.Create(string.Empty, false));

        checkBox.ClickStateHandler.Subscribe(isSelect =>
		{
            _toggleEvt.OnNext(_caseData);
        });
	}

	// 客户端自定义代码
	protected override void RegistCustomEvent ()
	{
		_disposable.Add(OnFormationBtn_UIButtonClick.Subscribe(_ =>
		{
            _clickEvt.OnNext(_caseData);
            ProxyFormation.OpenFormationView(FormationPosController.FormationType.Crew);
		}));

		_disposable.Add(CaseItemBtn_UIButtonEvt.Subscribe(_ => { _clickEvt.OnNext(_caseData); }));
	}

	protected override void RemoveCustomEvent ()
	{

	}
	
	protected override void OnDispose()
	{
		_disposable.Clear();
		_disposable = null;
		base.OnDispose();
	}

	public void UpdateView(IFormationCaseData data, bool isUse)
	{
        _caseData = data;
        Formation formation = DataCache.getDtoByCls<Formation>(data.GetDto.formationId);
	    _view.CaseName_UILabel.text = _nameList[data.GetIdx];
        _view.FormationName_UILabel.text = formation.name;
        IsUse = isUse;
	}

	private bool IsUse
	{
		get { return _view.Checkmark_UISprite.enabled; }
		set { _view.Checkmark_UISprite.enabled = value; }
	}

    public bool IsSelect
    {
        get { return _view.Select; }
        set { _view.Select.SetActive(value);}
    }

    public IFormationCaseData CaseDate { get { return _caseData; } }
}
