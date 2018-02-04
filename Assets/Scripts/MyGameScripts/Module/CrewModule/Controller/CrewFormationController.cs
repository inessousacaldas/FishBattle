// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  PartnerFormationController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AppDto;
using UniRx;
using UnityEngine;

public interface ICrewIconData
{
    PropertyType GetPropertyType { get; }
    GameObject GetGo { get; }
    int ItemIdx { get; }
    int DataIdx { get; }
}

public class CrewIconData:ICrewIconData
{
    private PropertyType _propertyType;
    public PropertyType GetPropertyType { get { return _propertyType; } }
    
    private GameObject _go;
    public GameObject GetGo{get { return _go; }}

    private int _itemIdx;
    public int ItemIdx{get { return _itemIdx; }}

    private int _dataIdx;
    public int DataIdx{get { return _dataIdx; }}

    public static CrewIconData Create(GameObject go, int itemIdx, int dataIdx, PropertyType type = PropertyType.All)
    {
        CrewIconData data = new CrewIconData();
        data._go = go;
        data._itemIdx = itemIdx;
        data._dataIdx = dataIdx;
        data._propertyType = type;
        return data;
    }
}

public partial interface ICrewFormationController
{
    void UpdatePosGroup(FormationCaseInfoDto data);
    UniRx.IObservable<Tuple<CrewFormationController.Option, long>> GetOptionClickHandler { get; }
    UniRx.IObservable<IFormationCaseData> GetToggleFormationHandler { get; }
    UniRx.IObservable<IFormationCaseData> GetFormationCaseHandler { get; }
    void InitData(int useIdx);
}

public partial class CrewFormationController
{
    #region Subject

    private Subject<IFormationCaseData> _toggleFormatinEvt = new Subject<IFormationCaseData>();
    public UniRx.IObservable<IFormationCaseData> GetToggleFormationHandler { get { return _toggleFormatinEvt; } }

    private Subject<IFormationCaseData> _formationCaseClickEvt = new Subject<IFormationCaseData>();
    public UniRx.IObservable<IFormationCaseData> GetFormationCaseHandler { get { return _formationCaseClickEvt; } }

    private Subject<Tuple<Option, long>> _optionClickEvt = new Subject<Tuple<Option, long>>();
    public UniRx.IObservable<Tuple<Option, long>> GetOptionClickHandler { get { return _optionClickEvt; } }
    #endregion

    private readonly int _listMaxNum = 7;        //列表最多实例7个item
    private readonly int _rankItemNum = 3;       //每排3个
    private int _curCaseId = 0;        //选中第几套方案
    private int _useCaseIdx = 1;             //使用第几套方案
    private long _curPartnerId;
    

    private IEnumerable<CrewInfoDto> _allCrewList = new List<CrewInfoDto>();
    private List<CrewFormationCaseController> _formationCaseList = new List<CrewFormationCaseController>(); 
    private List<CrewIconItemController> _crewTiledList = new List<CrewIconItemController>();
    private List<CrewItemController> _crewList = new List<CrewItemController>();
    private FormationPosController _posController;
    private FormationCaseData _caseData;
    private ITeamFormationData _data;

    private string[] _typeStr = {"全部", "力量", "魔法", "控制", "辅助"};

    private S3PopupListController teamPopupListCtrl;
    private List<PopUpItemInfo> nameList = new List<PopUpItemInfo>();
    private const int ButtonHigth = 52;

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        _posController = AddChild<FormationPosController, FormationPosView>(
            View.FormationPosGroup.gameObject
            , FormationPosView.NAME);
        
        InitPopupList();
    }

    public void InitData(int useCaseIdx)
    {
        _curCaseId = useCaseIdx;
    }
    
    // 客户端自定义事件
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(TiledBtn_UIButtonEvt.Subscribe(_ => SetItemShowTyep(false)));
        _disposable.Add(ListBtn_UIButtonEvt.Subscribe(_ => SetItemShowTyep(true)));
        _disposable.Add(TypeBtn_UIButtonEvt.Subscribe(_ =>
        {
            _view.ButtonArrow_UISprite.flip = UIBasicSprite.Flip.Horizontally;
            _view.BtnList_UIPanel.gameObject.SetActive(true);
        }));

        _disposable.Add(OnMagicBtn_UIButtonClick.Subscribe(_ => ShowSameTypePartner(PropertyType.Magic)));
        _disposable.Add(OnAllTypeBtn_UIButtonClick.Subscribe(_ => ShowSameTypePartner()));
        _disposable.Add(OnControlBtn_UIButtonClick.Subscribe(_ => ShowSameTypePartner(PropertyType.Control)));
        _disposable.Add(OnTreatBtn_UIButtonClick.Subscribe(_ => ShowSameTypePartner(PropertyType.Treat)));
        _disposable.Add(OnPowerBtn_UIButtonClick.Subscribe(_ => ShowSameTypePartner(PropertyType.Power)));
        UICamera.onClick += OnCameraClick;
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        teamPopupListCtrl.OnClickOtherEvt -= TeamPopupListCtrl_OnClickOtherEvt;
        teamPopupListCtrl = null;
        UICamera.onClick -= OnCameraClick;
        base.OnDispose();
    }

    // 如果自定义客户端交互使用了事件流，还是需要remove的
    protected override void RemoveCustomEvent ()
    {
        
    }

    protected override void UpdateDataAndView(ITeamFormationData data)
    {
        if (data == null || data.CrewFormationData == null) return;

        _data = data;
        _useCaseIdx = data.CurFormationType == 
            TeamFormationDataMgr.TeamFormationData.FormationType.ArenaFormation ? 
            data.CrewFormationData.GetArenaCaseId:
            data.CrewFormationData.GetUseCaseIdx;

        if (data.CrewFormationData.GetCrewFormationList.Count > 0)
        {
            InitFormationItem(data.CrewFormationData.GetCrewFormationList);
            InitPartnerData(data.CrewFormationData);
            UpdatePosGroup(data.CrewFormationData.GetCrewFormationList.Find(d=>d.caseId == _curCaseId));
        }
    }
    
    private void InitPartnerData(ICrewFormationData data)
    {
        _allCrewList = data.GetSelfCrew();
        InitListTable();
        InitTiledList();
        _view.Scrollview_UIScrollView.ResetPosition();
    }

    //平铺
    private void InitTiledList()
    {
        for (int i = _view.TiledGrid_UIGrid.transform.childCount; i < _allCrewList.Count(); i++)
        {
            var controller = AddChild<CrewIconItemController, CrewIconItem>(_view.TiledGrid_UIGrid.gameObject,
                CrewIconItem.NAME);
            var dto = _allCrewList.TryGetValue(i);
            if (dto == null) return;

            controller.SetCrewInfo(dto, i, GetCrewState(dto.id));
            _crewTiledList.Add(controller);
            _disposable.Add(controller.GetClickHandler.Subscribe(data => 
            {
                SelectCrewItem(data.GetIdx);
                _curPartnerId = data.GetCrewId;
                OnSelectPopupList(controller.GetAnchorPos, data);
            }));
        }
        _view.TiledGrid_UIGrid.Reposition();
    }

    //列表
    private void InitListTable()
    {
        for (int i = _view.ListTable_UITable.transform.childCount; i < _allCrewList.Count(); i++)
        {
            var controller = AddChild<CrewItemController, CrewItem>(_view.ListTable_UITable.gameObject,
                CrewItem.NAME);
            var dto = _allCrewList.TryGetValue(i);
            if (dto == null) return;

            controller.SetItemInfo(_allCrewList.TryGetValue(i), i, GetCrewState(dto.id));
            _crewList.Add(controller);
            _disposable.Add(controller.GetClickHandler.Subscribe(data =>
            {
                SelectCrewItem(data.GetIdx);
                _curPartnerId = data.GetCrewId;
                OnSelectPopupList(controller.GetAnchorPos, data);
            }));
        }

        _view.ListTable_UITable.Reposition();
    }

    private void SelectCrewItem(int idx)
    {
        _crewList.ForEachI((item, i) => { item.Select = i == idx; });
        _crewTiledList.ForEachI((item, i) => { item.Selected = i == idx; });
    }
    
    //阵法设置
    public void InitFormationItem(List<FormationCaseInfoDto> data)
    {
        if (_formationCaseList.Count != 0)
        {
            _formationCaseList.ForEachI((item, idx) =>
            {
                var caseinfo = data.TryGetValue(idx);
                item.gameObject.SetActive(caseinfo != null);
                if (caseinfo != null)
                {
                    var casedata = FormationCaseData.Create(data[idx], idx, data[idx].caseId);
                    item.UpdateView(casedata, _useCaseIdx == data[idx].caseId);
                }
            });
            return;
        }

        for (int i = 0; i < _view.FormationCaseGroup_Transform.childCount; i++)
        {
            var item = _view.FormationCaseGroup_Transform.GetChild(i);
            var controller = AddController<CrewFormationCaseController, CrewFormationCaseItem>(item.gameObject);
            var caseinfo = data.TryGetValue(i);
            controller.gameObject.SetActive(caseinfo != null);
            if (caseinfo != null)
            {
                _caseData = FormationCaseData.Create(data[i], i, data[i].caseId);
                controller.UpdateView(_caseData, _useCaseIdx == data[i].caseId);
            }
            _formationCaseList.Add(controller);
            _disposable.Add(controller.GetToggleClick.Subscribe(d =>
            {
                _toggleFormatinEvt.OnNext(d);
            }));
            _disposable.Add(controller.GetClickEvt.Subscribe(d => 
            {
                _posController.EnterBtnClickHandler();
                _curCaseId = d.GetCaseId;
                SetSelectCase();
                _formationCaseClickEvt.OnNext(d);
            }));
        }
        SetSelectCase();
    }

    private void SetSelectCase()
    {
        _formationCaseList.ForEachI((item, idx) =>
        {
            item.IsSelect = item.CaseDate == null ? false :item.CaseDate.GetCaseId == _curCaseId;
        });
    }

    public void UpdatePosGroup(FormationCaseInfoDto data)
    {
        _posController.UpdateCrewPos(data, FormationPosController.FormationType.Crew);

        _crewList.ForEachI((item, idx) =>
        {
            var dto = _allCrewList.TryGetValue(idx);
            if (dto == null)
                return;
            
            item.SetItemInfo(_allCrewList.TryGetValue(idx), idx, GetCrewState(dto.id));
        });
        _crewTiledList.ForEachI((item, idx) =>
        {
            var dto = _allCrewList.TryGetValue(idx);
            if (dto == null)
                return;
            item.SetCrewInfo(_allCrewList.TryGetValue(idx), idx, GetCrewState(dto.id));
        });
    }

    private Crew.BattleCrewType GetCrewState(long id)
    {
        Crew.BattleCrewType state;
        var caseInfo = _data.CrewFormationData.GetCrewFormationList.Find(d => d.caseId == _curCaseId);
        if (id == _data.CrewFormationData.GetMainCrewId)
            state = Crew.BattleCrewType.MainCrew;
        else if (caseInfo != null && caseInfo.casePositions.Find(d => d.crewId == id) != null)
            state = Crew.BattleCrewType.AssistCrew;
        else
            state = Crew.BattleCrewType.Default;

        return state;
    }

    public void SetItemShowTyep(bool b)
    {
        _view.ListBtn_UIButton.gameObject.SetActive(!b);
        _view.TiledBtn_UIButton.gameObject.SetActive(b);
        _view.ListTable_UITable.gameObject.SetActive(!b);
        _view.TiledGrid_UIGrid.gameObject.SetActive(b);
        _view.Scrollview_UIScrollView.ResetPosition();
    }

    //显示某种类型的伙伴
    private void ShowSameTypePartner(PropertyType type = PropertyType.All)
    {
        _view.CrewTypeLb_UILabel.text = _typeStr[(int) type];
        _view.BtnList_UIPanel.gameObject.SetActive(false);
        _view.ButtonArrow_UISprite.flip = UIBasicSprite.Flip.Vertically;
        var list = _data.CrewFormationData.GetCrewInfoByType(type);
        UpdateCrewItemList(list);
        UpdateCrewTiledList(list);
        SetBtnSprite(type);
        _view.Scrollview_UIScrollView.ResetPosition();
    }

    private void UpdateCrewItemList(IEnumerable<CrewInfoDto> list)
    {
        _crewList.ForEachI((item, idx) =>
        {
            var dto = list.TryGetValue(idx);
            if (dto == null)
                item.gameObject.SetActive(false);
            else
            {
                item.gameObject.SetActive(true);
                item.SetItemInfo(dto, idx, GetCrewState(dto.id));
            }
        });
    }

    private void UpdateCrewTiledList(IEnumerable<CrewInfoDto> list)
    {
        _crewTiledList.ForEachI((item, idx) =>
        {
            var dto = list.TryGetValue(idx);
            if (dto == null)
                item.gameObject.SetActive(false);
            else
            {
                item.gameObject.SetActive(true);
                item.SetCrewInfo(dto, idx, GetCrewState(dto.id));
            }
        });
    }

    private void SetBtnSprite(PropertyType type = PropertyType.All)
    {
        switch (type)
        {
            case PropertyType.All:
                _view.TypeSprite_UISprite.spriteName = "quanbuxiaotuxing";
                break;
            case PropertyType.Power:
                _view.TypeSprite_UISprite.spriteName = "typeIcon_1";
                break;
            case PropertyType.Control:
                _view.TypeSprite_UISprite.spriteName = "typeIcon_4";
                break;
            case PropertyType.Magic:
                _view.TypeSprite_UISprite.spriteName = "typeIcon_2";
                break;
            case PropertyType.Treat:
                _view.TypeSprite_UISprite.spriteName = "typeIcon_3";
                break;
        }
    } 

    public void OnCameraClick(GameObject go)
    {
        UIPanel panel = UIPanel.Find(go.transform);
        if (_view.BtnList_UIPanel.gameObject.active && panel != _view.BtnList_UIPanel)
        {
            _view.ButtonArrow_UISprite.flip = UIBasicSprite.Flip.Vertically;
            _view.BtnList_UIPanel.gameObject.SetActive(false);
        }
    }

    public enum Option
    {
        MainWar = 0,   //设为主战
        Up = 1,        //上阵
        Down = 2,      //下阵
    }

    #region PopupList
    private void TeamPopupListCtrl_OnClickOtherEvt()
    {
        teamPopupListCtrl.Hide();
    }

    private void InitPopupList()
    {
        teamPopupListCtrl = AddChild<S3PopupListController, S3PopupList>(View.gameObject, S3PopupList.PREFAB_WAREHOUSE);
        teamPopupListCtrl.InitData(S3PopupItem.PREFAB_TEAMBTN, nameList,
            isClickHide: false, isShowList: true, isShowBg: false);
        teamPopupListCtrl.Hide();
        teamPopupListCtrl.OnClickOtherEvt += TeamPopupListCtrl_OnClickOtherEvt;
        teamPopupListCtrl.OnChoiceIndexStream.Subscribe(item =>
        {
            switch ((Option)item.EnumValue)
            {
                case Option.MainWar:
                    TeamFormationDataMgr.TeamFormationNetMsg.Formation_ChangeMainCrew(_curCaseId, _curPartnerId);
                    break;
                case Option.Up:
                    TeamFormationDataMgr.TeamFormationNetMsg.Formation_AddPosition(_curCaseId, _curPartnerId);
                    break;
                case Option.Down:
                    TeamFormationDataMgr.TeamFormationNetMsg.Formation_RemovePosition(_curCaseId, _curPartnerId);
                    break;
            }
            TeamPopupListCtrl_OnClickOtherEvt();
        });
    }

    private void OnSelectPopupList(Vector3 pos, ICrewItemData data)
    {
        nameList.Clear();

        if(data.GetCrewId != _data.CrewFormationData.GetMainCrewId)
            nameList.Add(new PopUpItemInfo("设为主战", (int)Option.MainWar));
        if(_data.CrewFormationData.CrewIsFigthing(_curCaseId, data.GetCrewId))
            nameList.Add(new PopUpItemInfo("下阵", (int)Option.Down));
        else
            nameList.Add(new PopUpItemInfo("上阵", (int)Option.Up));

        teamPopupListCtrl.UpdateView(nameList);
        teamPopupListCtrl.Show();
        teamPopupListCtrl.SetPos(pos);
        teamPopupListCtrl.SetListBgDimensions(126, ButtonHigth * nameList.Count + 5);
    }
    #endregion
}
