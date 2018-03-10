// **********************************************************************
// Copyright (c)  Baoyugame. All rights reserved.
// File     :  TeamFormationTabContentViewController.cs
// Author   : 
// Created  : $timeDecls$
// Porpuse  : 
// **********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using AppDto;
using UniRx;
using UnityEngine;

public partial interface ITeamFormationController
{
    UniRx.IObservable<int> GetLearnFormationHandler { get; }
    UniRx.IObservable<int> GetUpGradeHandler { get; }
}

public partial class TeamFormationController
{
    private ITeamFormationData _data;
    private FormationPosController.FormationType _type;
    private List<FormationInfoItemController> _formationItemList = new List<FormationInfoItemController>(); 
    private PageTurnViewController pageTurn;
    private FormationPosController _posController;
    private int _curFormationId = -1;

    private List<UISprite> _restrainList = new List<UISprite>();
    private List<UISprite> _unRestrainList = new List<UISprite>();
    private List<FormationGrade> _formationGradeList = new List<FormationGrade>(); 
    private List<Formation> _formationList = new List<Formation>();
    private string[] _rankStr = {"前排", "中排", "后排"};
    #region Subject
    private Subject<int> _useFormationEvt = new Subject<int>();
    public UniRx.IObservable<int> GetUseBtnHandler { get { return _useFormationEvt; } }  

    private Subject<Unit> _noUseFormationEvt = new Subject<Unit>();
    public UniRx.IObservable<Unit> GetNoUseFormationHandler { get { return _noUseFormationEvt; } } 
    
    private Subject<int> _learnFormationEvt = new Subject<int>();
    public UniRx.IObservable<int> GetLearnFormationHandler { get { return _learnFormationEvt; } }   

    private Subject<int> _upGradeEvt = new Subject<int>();
    public UniRx.IObservable<int> GetUpGradeHandler { get { return _upGradeEvt; } }  
    #endregion

    // 界面初始化完成之后的一些后续初始化工作
    protected override void AfterInitView ()
    {
        InitSpriteList();
        InitPosGroup();
    }

    protected override void UpdateDataAndView(ITeamFormationData data)
    {
        _data = data;
        FormationState state;
        if (data.ActiveFormationId == (int) Formation.FormationType.Regular)
        {
            var fid = _curFormationId == -1 ? data.GetFormationByIdx(0).id : _curFormationId;
            state = data.GetFormationState(fid);
            _curFormationId = fid;
        }
        else
        {
            var fid = _curFormationId == -1 ? data.ActiveFormationId : _curFormationId;
            state = data.GetFormationState(fid);
            _curFormationId = fid;
        }

        ShowOrHideLearnBtn(state != FormationState.Learned);

        UpdateInfoItemList();
        UpdatePosGroup();
        var lv = _data.GetFormationLevel(_curFormationId);
        _view.FormationLv_UILabel.text = lv == 0 ? "" : string.Format("Lv.{0}", lv);
        var f = _formationList.Find(d => d.id == _curFormationId);
        _view.FormationNameLb_UILabel.text = f == null ? "" : f.name;

    }

    private void ShowOrHideLearnBtn(bool b)
    {
        View.LearnBtn_UIButton.gameObject.SetActive(b);
        View.UpGradeBtn_UIButton.gameObject.SetActive(!b);
    }

    private void ShowFormationDesc(bool b)
    {
        _view.FormationDesc_UILabel.gameObject.SetActive(b);
        _view.RestrainBtn_UIButton.gameObject.SetActive(b);
        _view.RestrainGroup.SetActive(!b);
    }

    public void InitFormationList(ITeamFormationData data, FormationPosController.FormationType type)
    {
        _data = data;
        _type = type;
        var allFormationList = _data.GetAllFormationInfo();
        allFormationList.ForEachI((formation,i)=> 
        {
            var com = AddChild<FormationInfoItemController, FormationInfoItem>(
                View.LeftItemGrid_UIGrid.gameObject
                , FormationInfoItem.NAME
                , string.Format("{0}_{1}", FormationInfoItem.NAME, i));

            _disposable.Add(com.OnFormationInfoItem_UIButtonClick.Subscribe(_ =>
            {
                var f = data.GetFormationByIdx(i);
                _curFormationId = f.id;
                SelectFormationItem(f.id, i);
                com.SetCloseBtnState(f.id == data.ActiveFormationId);
                com.SetOpenBtnState(f.id != data.ActiveFormationId);
                var lv = _data.GetFormationLevel(f.id);
                _view.FormationLv_UILabel.text = lv == 0 ? "" : string.Format("Lv.{0}", lv);
                _view.FormationNameLb_UILabel.text = f.name;
            }));
            _disposable.Add(com.GetCloseHandler.Subscribe(_ => { _noUseFormationEvt.OnNext(new Unit());}));
            _disposable.Add(com.GetOpenHandler.Subscribe(_ =>
            {
                var f = data.GetFormationByIdx(i);
                _useFormationEvt.OnNext(f.id);
            }));

            _formationItemList.Add(com);
            UpdateFormationInfoItem(com, formation);
        });

        if (_data.ActiveFormationId == (int) Formation.FormationType.Regular)
            SelectFormationItem(allFormationList.TryGetValue(0).id, 0);
        else
        {
            var idx = allFormationList.FindElementIdx(d => d.id == _data.ActiveFormationId);
            SelectFormationItem(_data.ActiveFormationId, idx);
        }
    }

    private void SelectFormationItem(int formationId, int idx)
    {
        UpdatePosGroup();
        var state = _data.GetFormationState(formationId);
        ShowOrHideLearnBtn(state != FormationState.Learned);
        DebuffTargetIds(formationId);
        UpdateRankInfo(formationId);
        OnFormationClick(idx);
        var lv = _data.GetFormationLevel(formationId);
        _view.FormationLv_UILabel.text = lv == 0 ? "" : string.Format("Lv.{0}", lv);
        var f = _formationList.Find(d => d.id == formationId);
        _view.FormationNameLb_UILabel.text = f == null ? "" : f.name;
    }

    private void OnFormationClick(int idx)
    {
        _formationItemList.ForEachI((item, i) =>
        {
            item.SetCloseBtnState(false);
            item.SetOpenBtnState(false);
            item.SetSelect(idx == i);
        });
    }
    
    private void UpdateFormationInfoItem(FormationInfoItemController com, Formation formation)
    {
        var state = _data.GetFormationState(formation.id);
        bool select = false;
        if (_data.ActiveFormationId == (int) Formation.FormationType.Regular)
            select = _curFormationId == -1
                ? formation.id == _data.GetFormationByIdx(0).id
                : formation.id == _curFormationId;
        else
            select = _curFormationId == -1 ? _data.ActiveFormationId == formation.id : _curFormationId == formation.id;

        var level = _data.GetFormationLevel(formation.id);
        if (select)
        {
            var f = _data.GetFormationDtoById(formation.id);
            level = f == null ? 1 :f.level;
        }

        
        bool flagState;
        if (_type == FormationPosController.FormationType.Team)
        {
            var team = TeamDataMgr.DataMgr.GetSelfTeamDto();
            flagState = team == null
                ? _data.ActiveFormationId == formation.id
                : team.formation.formationId == formation.id;
        }
        else
            flagState = _data.ActiveFormationId == formation.id;

        com.UpdateView(
            formation
            , state
            , level
            , select
            , flagState);
    }
    
    // 客户端自定义代码
    protected override void RegistCustomEvent ()
    {
        _disposable.Add(DescBtn_UIButtonEvt.Subscribe(_ => { ShowFormationDesc(true); }));
        _disposable.Add(RestrainBtn_UIButtonEvt.Subscribe(_ => { ShowFormationDesc(false);}));
        _disposable.Add(LearnBtn_UIButtonEvt.Subscribe(_=> {_learnFormationEvt.OnNext(_curFormationId);}));
        _disposable.Add(UpGradeBtn_UIButtonEvt.Subscribe(_ => { _upGradeEvt.OnNext(_curFormationId);}));
    }

    protected override void OnDispose()
    {
        _disposable = _disposable.CloseOnceNull();
        base.OnDispose();
    }

    //在打开界面之前，初始化数据
    protected override void InitData()
    {
        _formationGradeList = DataCache.getArrayByCls<FormationGrade>();
        _formationList = DataCache.getArrayByCls<Formation>();
    }

    private void InitSpriteList()
    {
        var max = 3;
        for (int i = 0; i < max; i++)
        {
            UISprite restrain = _view.RestrainGrid_UIGrid.gameObject.FindGameObject("Sprite_" + i).GetMissingComponent<UISprite>();
            _restrainList.Add(restrain);

            UISprite unRestrain = _view.UnRestrainGrid_UIGrid.gameObject.FindGameObject("Sprite_" + i).GetMissingComponent<UISprite>();
            _unRestrainList.Add(unRestrain);
        }
    }

    private void UpdateInfoItemList()
    {
        _formationItemList.ForEachI(delegate(FormationInfoItemController item , int i)
        {
            var dto = _data.GetFormationByIdx(i);
            UpdateFormationInfoItem(item, dto);
            item.SetCloseBtnState(false);
            item.SetOpenBtnState(false);
        });
    }

    public void InitPosGroup()
    {
        _posController = AddChild<FormationPosController, FormationPosView>(
            View.FormationPosGroup.gameObject
            , FormationPosView.NAME);
    }

    private void UpdatePosGroup()
    {
        if (TeamDataMgr.DataMgr.HasTeam())
            _posController.UpdateTeamPos(_data.GetTeamDto.members, 
                FormationPosController.FormationType.Team);
        else
        {
            if (_data.AllCaseInfoDto == null||
                _data.AllCaseInfoDto.caseInfoDtos == null||
                _data.AllCaseInfoDto.caseInfoDtos.Count == 0)
                return;
            _posController.UpdateCrewPos(_data.AllCaseInfoDto.caseInfoDtos.Find(d=>d.caseId == _data.GetCurCaseIdx), 
                FormationPosController.FormationType.Crew);
        }
    }

    private void UpdateRankInfo(int formationId)
    {
        if (formationId == (int)Formation.FormationType.Regular) return;

        var formationDto = _data.acquiredFormation.Find(d => d.formationId == formationId);
        var formation = DataCache.getDtoByCls<Formation>(formationId);
        int lv = formationDto == null ? 0 : formationDto.level - 1;
        
        var info = formation.description.Split(';');
        var max = formation.descMaxNum.Split(';');
        StringBuilder sb = new StringBuilder();
        info.ForEachI((f,i)=>
        {
            string str = "";
            var txt = f.Split(',');
            txt.ForEachI((s, idx) =>
            {
                if(idx < txt.Length - 1)
                    str += string.Format(s+",", GetFormationDescValue(lv, float.Parse(max[i].Split(',')[idx])));
                else
                    str += string.Format(s, GetFormationDescValue(lv, float.Parse(max[i].Split(',')[idx])));
            });
            sb.AppendLine(_rankStr[i] + ":" + str);
        });
        _view.FormationDesc_UILabel.text = sb.ToString();
    }

    private string GetFormationDescValue(int lv, float maxValue)
    {
        var formationGrade = _formationGradeList[lv];
       
        if (formationGrade == null)
        {
            GameDebuger.LogError(string.Format("FormationGrade表找不到{0}级相关的数据", lv));
            return "";
        }
        return string.Format("{0}%", maxValue*formationGrade.effect).WrapColor(ColorConstantV3.Color_Green_Str);
    }

    public void DebuffTargetIds(int id)
    {
        var formation = _formationList.Find(d => d.id == id);
        if (formation == null)
        {
            GameDebuger.LogError(string.Format("Formation表获取不到{0}内容", id));
            return;
        }

        if (formation.debuffTargetIds.Count == 0)
        {
            _restrainList.ForEach(item => { item.gameObject.SetActive(false); });
            return;
        }

        _restrainList.ForEachI((sprite, idx) =>
        {
            sprite.gameObject.SetActive(true);
            var f = _formationList.Find(d => d.id == formation.debuffTargetIds[idx]);
            sprite.spriteName = f == null ? "formationicon_" : string.Format("formationicon_{0}", f.id);
        });

        if (formation.targetIds.Count == 0)
        {
            _unRestrainList.ForEach(item => { item.gameObject.SetActive(false); });
            return;
        }

        _unRestrainList.ForEachI((sprite, idx) =>
        {
            sprite.gameObject.SetActive(true);
            var f = _formationList.Find(d => d.id == formation.targetIds[idx]);
            sprite.spriteName = f == null ? "formationicon_" : string.Format("formationicon_{0}", f.id);
        });
    }
}